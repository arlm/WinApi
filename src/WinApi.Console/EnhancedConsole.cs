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
    public sealed class EnhancedConsole : IDisposable
    {
        private const string WAIT_PROMPT = "\n\nPress any key to quit ...";
        private static readonly char[] spinner = { '-', '\\', '|', '/' };

        private Kernel32.HandlerRoutine consoleCtrlHandler;
        private volatile bool hasFaulted ;
        private volatile bool hasHandledOnExit;
        private bool isCapturingFirstChanceException;
        private bool disposedValue;
        
        private volatile bool marqueeRunning;

        private Thread marqueeThread;

        private volatile bool marqueeWithProgress = true;

        private volatile float pctComplete;

        private bool shouldContinueOnBreak;

        private bool shouldPrompt = true;

        private bool consoleAlloced = false;

        private int spinnerPos;

        public EnhancedConsole()
        {
            if (System.Console.LargestWindowHeight == 0 && System.Console.LargestWindowWidth == 0)
            {
                consoleAlloced = AttachOrAllocConsole();
            }

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

        ~EnhancedConsole()
        {
            Dispose(false);
        }

        public event EventHandler<ConsoleCancelEventArgs> OnCancelKeyPressExit;

        public event EventHandler<EventArgs> OnProcessExit;

        public event EventHandler<UnhandledExceptionEventArgs> OnUnhandledException;

        public bool CaptureFirstChanceException
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

        public float Progress
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

        public bool ShouldExitOnErrors
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

        public bool ShowProgress
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

        public bool Unattended
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetTitle()
        {
            var assembly = Assembly.GetCallingAssembly();
            var company = assembly.GetAttribute<AssemblyCompanyAttribute>();
            var name = assembly.GetAttribute<AssemblyTitleAttribute>();

            System.Console.Title = $"{company?.Company ?? "Console"} | {name?.Title ?? "Application"}";
        }

        public void StartMarquee(bool showProgress = true)
        {          
            StopMarquee();

            marqueeWithProgress = showProgress;
            marqueeRunning = true;
            marqueeThread = new Thread(MarqueeThread);
            marqueeThread.Start();
        }

        public void StopMarquee()
        {
            marqueeRunning = false;
            var threadState = marqueeThread?.ThreadState ?? ThreadState.Unstarted;

            if (threadState != ThreadState.Aborted && threadState != ThreadState.Unstarted && threadState != ThreadState.Stopped)
            {
                if (!marqueeThread?.Join(750) ?? false)
                {
                    try
                    {
                        marqueeThread.Abort();
                    }
                    catch (ThreadAbortException)
                    {
                    }
                }
            }

            marqueeThread = null;
        }

        private bool AttachConsole()
        {
            const int ParentProcess = unchecked((int)0xFFFFFFFF);
            if (!Kernel32.AttachConsole(ParentProcess))
            {
                return false;
            }

            System.Console.Clear();
            return true;
        }

        private bool AttachOrAllocConsole()
        {
            if (!AttachConsole())
            {
                return Kernel32.AllocConsole();
            }

            return false;
        }

        private bool FreeConsole()
        {
            return Kernel32.FreeConsole();
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void AppDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
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

                if (shouldPrompt)
                {
                    ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                }

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

        private void AppDomain_ProcessExit(object sender, EventArgs e)
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

                        if (shouldPrompt)
                        {
                            ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                        }
                    }

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void AppDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
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

                if (shouldPrompt)
                {
                    ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                }

                var args = new UnhandledExceptionEventArgs(e);
                OnUnhandledException?.Invoke(sender, args);

                Dispose();
                Environment.Exit(args.ExitCode);
            }
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
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

        private bool Console_CtrlEvent(Kernel32.ControlType sig)
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

        private void MarqueeThread()
        {
            while (marqueeRunning && pctComplete <= 100)
            {
                UpdateProgress(pctComplete);

                Thread.Sleep(250);
            }

            System.Console.Out.WriteLine(Environment.NewLine);
        }

        private void Process_Disposed(object sender, EventArgs e)
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

                    if (shouldPrompt)
                    {
                        ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                    }

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
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

                    if (shouldPrompt)
                    {
                        ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                    }

                    OnProcessExit?.Invoke(sender, e);

                    Dispose();
                }
            }
        }

        private void SystemEvents_EventsThreadShutdown(object sender, EventArgs e)
        {
            Dispose();
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            Dispose();
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Dispose();
            e.Cancel = false;
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            Dispose();
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
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

                if (shouldPrompt)
                {
                    ConsoleExtensions.WaitIfDebugging(WAIT_PROMPT);
                }

                var args = new UnhandledExceptionEventArgs(e);
                OnUnhandledException?.Invoke(sender, args);
            }
        }

        private void UpdateProgress(float completion)
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

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopMarquee();

                    CaptureFirstChanceException = false;

                    SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                    SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
                    SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
                    SystemEvents.EventsThreadShutdown -= SystemEvents_EventsThreadShutdown;
                    System.Console.CancelKeyPress -= Console_CancelKeyPress;

                    var currentProcess = Process.GetCurrentProcess();
                    currentProcess.Disposed -= Process_Disposed;
                    currentProcess.Exited -= Process_Exited;

                    TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
                }

                Kernel32.SetConsoleCtrlHandler(consoleCtrlHandler, false);
                consoleCtrlHandler = null;

                if (consoleAlloced)
                {
                    FreeConsole();
                }

                AppDomain.CurrentDomain.ProcessExit -= AppDomain_ProcessExit;
                AppDomain.CurrentDomain.UnhandledException -= AppDomain_UnhandledException;

                disposedValue = true;
            }
        }
    }
}