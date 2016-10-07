// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using PInvoke;

using ThreadState = System.Threading.ThreadState;

namespace WinApi.Console
{
    public static class ConsoleExtensions
    {
        /// _CALL_REPORTFAULT -> 0x2
        private const int _CALL_REPORTFAULT = 2;

        /// _ONEXIT_T_DEFINED ->
        /// Error generating expression: Der Wert darf nicht NULL sein.
        ///Parametername: node
        private const string _ONEXIT_T_DEFINED = "";

        /// _WRITE_ABORT_MSG -> 0x1
        private const int _WRITE_ABORT_MSG = 1;

        /// CREATE_NEW_CONSOLE -> 0x00000010
        private const int CREATE_NEW_CONSOLE = 16;

        /// EVENT_CONSOLE_CARET -> 0x4001
        private const int EVENT_CONSOLE_CARET = 16385;

        /// EVENT_CONSOLE_LAYOUT -> 0x4005
        private const int EVENT_CONSOLE_LAYOUT = 16389;

        /// EVENT_CONSOLE_START_APPLICATION -> 0x4006
        private const int EVENT_CONSOLE_START_APPLICATION = 16390;

        /// EVENT_CONSOLE_UPDATE_REGION -> 0x4002
        private const int EVENT_CONSOLE_UPDATE_REGION = 16386;

        /// EVENT_CONSOLE_UPDATE_SCROLL -> 0x4004
        private const int EVENT_CONSOLE_UPDATE_SCROLL = 16388;

        /// EVENT_CONSOLE_UPDATE_SIMPLE -> 0x4003
        private const int EVENT_CONSOLE_UPDATE_SIMPLE = 16387;

        /// SEE_MASK_NO_CONSOLE -> 0x00008000
        private const int SEE_MASK_NO_CONSOLE = 32768;

        private const string WAIT_PROMPT = "\n\nPress any key to quit ...";

        private static readonly char[] spinner = { '-', '\\', '|', '/' };
        private static Kernel32.HandlerRoutine consoleCtrlHandler;
        private static volatile bool hasFaulted ;
        private static volatile bool hasHandledOnExit;
        private static bool isCapturingFirstChanceException;

        private volatile static bool marqueeRunning;

        private static Thread marqueeThread;

        private volatile static bool marqueeWithProgress = true;

        private volatile static float pctComplete;

        private static bool shouldContinueOnBreak;

        private static bool shouldPrompt = true;

        private static int spinnerPos;

        static ConsoleExtensions()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;

            var currentProcess = Process.GetCurrentProcess();
            currentProcess.Exited += Process_Exited;
            currentProcess.Disposed += Process_Disposed;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            System.Console.CancelKeyPress += Console_CancelKeyPress;

            consoleCtrlHandler = new Kernel32.HandlerRoutine(Console_CtrlEvent);
            Kernel32.SetConsoleCtrlHandler(consoleCtrlHandler, true);

            SystemEvents.EventsThreadShutdown += SystemEvents_EventsThreadShutdown;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnded += SystemEvents_SessionEnded;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        /// Return Type: int
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int _onexit_t();

        /// Return Type: void
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void atexit_function();

        /// Return Type: int
        ///param0: Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375
        [DllImport("msvcr80.dll", EntryPoint = "atexit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int atexit(atexit_function param0);

        /// Return Type: terminate_function
        [DllImport("msvcr80.dll", EntryPoint = "_get_terminate")]
        private static extern terminate_function _get_terminate();

        /// Return Type: unexpected_function
        [DllImport("msvcr80.dll", EntryPoint = "_get_unexpected")]
        private static extern unexpected_function _get_unexpected();

        /// Return Type: _onexit_t
        ///_Func: _onexit_t
        [DllImport("msvcr80.dll", EntryPoint = "_onexit", CallingConvention = CallingConvention.Cdecl)]
        private static extern _onexit_t _onexit(_onexit_t _Func);

        /// Return Type: unsigned int
        ///_Flags: unsigned int
        ///_Mask: unsigned int
        [DllImport("msvcr80.dll", EntryPoint = "_set_abort_behavior", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint _set_abort_behavior(uint _Flags, uint _Mask);

        /// Return Type: void
        ///param0: terminate_function
        [DllImport("msvcr80.dll", EntryPoint = "set_terminate", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_terminate(terminate_function param0);

        /// Return Type: void
        ///param0: int
        [DllImport("msvcr80.dll", EntryPoint = "set_unexpected", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_unexpected(int param0);

        /// Return Type: int
        ///sig: int
        ///param1: signal_function
        [DllImport("msvcr80.dll", EntryPoint = "signal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int signal(int sig, signal_function param1);

        /// Return Type: void
        ///param0: int[]
        private delegate void signal_function(int[] param0);

        /// Return Type: void
        private delegate void terminate_function();

        /// Return Type: void
        private delegate void unexpected_function();

        public static event EventHandler<ConsoleCancelEventArgs> OnCancelKeyPressExit;

        public static event EventHandler<EventArgs> OnProcessExit;

        public static event EventHandler<UnhandledExceptionEventArgs> OnUnhandledException;

        public static bool CaptureFirstChanceException
        {
            get
            {
                return isCapturingFirstChanceException;
            }

            set
            {
                if (!value && isCapturingFirstChanceException)
                {
                    AppDomain.CurrentDomain.FirstChanceException -= AppDomain_FirstChanceException;
                }

                isCapturingFirstChanceException = value;

                if (isCapturingFirstChanceException)
                {
                    AppDomain.CurrentDomain.FirstChanceException += AppDomain_FirstChanceException;
                }
            }
        }

        public static float Progress
        {
            get
            {
                return pctComplete;
            }

            set
            {
                if (value >= 100f)
                {
                    pctComplete = 100;

                    if (marqueeRunning)
                    {
                        StopMarquee();
                    }
                }
                else if (value <= 0)
                {
                    pctComplete = 0;
                }

                UpdateProgress(pctComplete);
            }
        }

        public static bool ShouldExitOnErrors
        {
            get
            {
                return !shouldContinueOnBreak;
            }

            set
            {
                shouldContinueOnBreak = !value;
            }
        }

        public static bool ShowProgress
        {
            get
            {
                return marqueeWithProgress;
            }
            set
            {
                marqueeWithProgress = value;
            }
        }

        public static bool Unattended
        {
            get
            {
                return !shouldPrompt;
            }

            set
            {
                shouldPrompt = !value;
            }
        }

        public static void SetTitle()
        {
            var assembly = Assembly.GetCallingAssembly();
            var company = assembly.GetAttribute<AssemblyCompanyAttribute>();
            var name = assembly.GetAttribute<AssemblyTitleAttribute>();

            System.Console.Title = $"{company?.Company ?? "Console"} | {name?.Title ?? "Application"}";
        }

        public static void StartMarquee(bool showProgress = true)
        {
            marqueeRunning = false;
            marqueeThread.Dispose();
            marqueeThread = null;

            marqueeWithProgress = showProgress;
            marqueeRunning = true;
            marqueeThread = new Thread(MarqueeThread);
            marqueeThread.Start();
        }

        public static void StopMarquee()
        {
            marqueeRunning = false;
            marqueeThread.Dispose();
            marqueeThread = null;

            marqueeThread = null;
        }

        public static void WaitIfDebugging(string message)
        {
            if (shouldPrompt)
            {
                var parentProcess = Process.GetCurrentProcess().Parent();

                if ((parentProcess != null && parentProcess.ProcessName != "cmd") ||
                    Debugger.IsAttached)
                {
                    System.Console.Out.WriteLine(message);
                    System.Console.ReadKey();
                }
            }
        }

        public static void WaitIfNotDebugging(string message)
        {
            if (shouldPrompt)
            {
                var parentProcess = Process.GetCurrentProcess().Parent();

                if ((parentProcess != null && parentProcess.ProcessName == "cmd") &&
                    !Debugger.IsAttached)
                {
                    System.Console.Out.WriteLine(message);
                    System.Console.ReadKey();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Global exception handlers should not throw exceptions")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void AppDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            hasFaulted = true;

            var message = "\nFirst Chance Exception";

            try
            {
                var stacktrace = new StackTrace(e.Exception, true);
                var thisStacktrace = new StackTrace(1 + stacktrace.FrameCount, true);

                StopMarquee();

                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = $"\nFirst Chance Exception ({e.Exception.GetType().FullName}) in {assemblyName.Name} v{assemblyName.Version}: {e.Exception.Flatten()}{thisStacktrace.ToString()}";
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"\nError in First Chance Exception handler: {ex.Flatten()}");
            }
            finally
            {
                System.Console.Error.WriteLine(message);

                WaitIfDebugging(WAIT_PROMPT);

                var args = new UnhandledExceptionEventArgs(e);
                OnUnhandledException?.Invoke(sender, args);

                if (e.Exception is SEHException ||
                    e.Exception is AccessViolationException ||
                    e.Exception is StackOverflowException)
                {
                    Dispose();
                    Environment.Exit(args.ExitCode);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not throw exceptions when the AppDomain process exits")]
        private static void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            if (!hasHandledOnExit)
            {
                hasHandledOnExit = true;

                var message = "\nProcess Exit";

                try
                {
                    StopMarquee();

                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    message = $"\nProcess ({Process.GetCurrentProcess()?.Id.ToString() ?? "unknown"}) Exit in {assemblyName.Name} v{assemblyName.Version}";
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine($"\nError in Process Exit handler: {ex.Flatten()}");
                }
                finally
                {
                    if (hasFaulted)
                    {
                        System.Console.Error.WriteLine(message);

                        WaitIfDebugging(WAIT_PROMPT);
                    }

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Global exception handlers should not throw exceptions")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            hasFaulted = true;

            var message = "\nUnhandled Exception";

            try
            {
                var exception = (e.ExceptionObject as Exception);
                if (exception != null)
                {
                    var stacktrace = new StackTrace(exception, true);
                    var thisStacktrace = new StackTrace(1 + stacktrace.FrameCount, true);

                    StopMarquee();

                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    message = $"\nUnhandled Exception ({e.ExceptionObject.GetType().FullName}) in {assemblyName.Name} v{assemblyName.Version}: {exception.Flatten()}{thisStacktrace.ToString()}";
                }
                else
                {
                    throw new ArgumentException($"parameter is not of the Exception type ({e.ExceptionObject.GetType().FullName})", nameof(e.ExceptionObject));
                }
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"\nError in Unhandled Exception handler: {ex.Flatten()}");
            }
            finally
            {
                System.Console.Error.WriteLine(message);

                WaitIfDebugging(WAIT_PROMPT);

                var args = new UnhandledExceptionEventArgs(e);
                OnUnhandledException?.Invoke(sender, args);

                Dispose();
                Environment.Exit(args.ExitCode);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not throw exceptions during CancelKeyPress handler")]
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            hasFaulted = true;

            var message = "\nCancel Key Press";

            try
            {
                StopMarquee();

                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = $"\nCatched Cancel Key Press ({e.SpecialKey.ToString("G")}) in {assemblyName.Name} v{assemblyName.Version}";
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"\nError in Cancel Key Press handler: {ex.Flatten()}");
            }
            finally
            {
                System.Console.Error.WriteLine(message);

                e.Cancel = shouldContinueOnBreak;

                if (!shouldContinueOnBreak)
                {
                    OnCancelKeyPressExit?.Invoke(sender, e);
                }

                Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not throw exceptions during CtrlEvent handler")]
        private static bool Console_CtrlEvent(Kernel32.ControlType sig)
        {
            hasFaulted = true;

            var message = "\nExiting system due to external CTRL-C, process kill or shutdown";

            try
            {
                StopMarquee();

                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = $"\nCatched Console Ctrl Event ({sig.ToString("G")}) in {assemblyName.Name} v{assemblyName.Version}";
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"\nError in Console Ctrl Event handler: {ex.Flatten()}");
            }
            finally
            {
                System.Console.Error.WriteLine(message);

                if (!shouldContinueOnBreak)
                {
                    OnCancelKeyPressExit?.Invoke(Process.GetCurrentProcess(), null);

                    Dispose();
                    Environment.Exit(int.MinValue);
                }
            }

            return true;
        }

        private static void Dispose()
        {
            StopMarquee();

            CaptureFirstChanceException = false;

            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.EventsThreadShutdown -= SystemEvents_EventsThreadShutdown;

            Kernel32.SetConsoleCtrlHandler(consoleCtrlHandler, false);
            consoleCtrlHandler = null;

            System.Console.CancelKeyPress -= Console_CancelKeyPress;

            TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;

            var currentProcess = Process.GetCurrentProcess();
            currentProcess.Disposed -= Process_Disposed;
            currentProcess.Exited -= Process_Exited;

            AppDomain.CurrentDomain.ProcessExit -= AppDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;
        }

        private static void Dispose(this Thread thread)
        {
            try
            {
                var threadState = thread?.ThreadState ?? ThreadState.Unstarted;
                if (threadState != ThreadState.Suspended && threadState != ThreadState.Background && threadState != ThreadState.Running)
                {
                    return;
                }

                if (!thread.Join(1500))
                {
                    thread.Abort();
                }
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                marqueeThread = null;
            }
        }

        private static void MarqueeThread()
        {
            while (marqueeRunning && pctComplete <= 100)
            {
                UpdateProgress(pctComplete);

                Thread.Sleep(250);
            }

            System.Console.Out.WriteLine(Environment.NewLine);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not throw exceptions during Process.Disposed handler")]
        private static void Process_Disposed(object sender, EventArgs e)
        {
            if (!hasHandledOnExit)
            {
                hasHandledOnExit = true;

                var message = "\nProcess Disposed";

                try
                {
                    StopMarquee();

                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    message = $"\nProcess ({Process.GetCurrentProcess()?.Id.ToString() ?? "unknown"}) Disposed in {assemblyName.Name} v{assemblyName.Version}";
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine($"\nError in Process Disposed handler: {ex.Flatten()}");
                }
                finally
                {
                    System.Console.Error.WriteLine(message);

                    WaitIfDebugging(WAIT_PROMPT);

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should not throw exceptions during Process.Exited handler")]
        private static void Process_Exited(object sender, EventArgs e)
        {
            if (!hasHandledOnExit)
            {
                hasHandledOnExit = true;

                var message = "\nProcess Exited";

                try
                {
                    StopMarquee();

                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    message = $"\nProcess ({Process.GetCurrentProcess()?.Id.ToString() ?? "unknown"}) Exited in {assemblyName.Name} v{assemblyName.Version}";
                }
                catch (Exception ex)
                {
                    System.Console.Error.WriteLine($"\nError in Process Exited handler: {ex.Flatten()}");
                }
                finally
                {
                    System.Console.Error.WriteLine(message);

                    WaitIfDebugging(WAIT_PROMPT);

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        private static void SystemEvents_EventsThreadShutdown(object sender, EventArgs e)
        {
            Dispose();
        }

        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
        }

        private static void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            Dispose();
        }

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Dispose();
            e.Cancel = false;
        }

        private static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Dispose();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Global exception handlers should not throw exceptions")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            hasFaulted = true;

            var message = "\nUnobserved Task Exception";

            try
            {
                StopMarquee();

                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = $"\nUnobserved Task Exception ({e.Exception.GetType().FullName}) in {assemblyName.Name} v{assemblyName.Version}: {e.Exception.ToFlattenedString()}";
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"\nError in Unobserved Task Exception handler: {ex.Flatten()}");
            }
            finally
            {
                System.Console.Error.WriteLine(message);

                WaitIfDebugging(WAIT_PROMPT);

                var args = new UnhandledExceptionEventArgs(e);
                OnUnhandledException?.Invoke(sender, args);
            }
        }

        private static void UpdateProgress(float completion)
        {
            if (completion >= 100f)
            {
                System.Console.Out.WriteLine("\rProcess Complete!".PadRight(System.Console.BufferWidth));
            }
            else
            {
                string marquee;

                if (marqueeWithProgress)
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]} - {completion:0.0}%";
                }
                else
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]}";
                }

                System.Console.Out.Write(marquee.PadRight(System.Console.BufferWidth));
                spinnerPos = (spinnerPos >= 3) ? 0 : spinnerPos + 1;
            }
        }
    }
}