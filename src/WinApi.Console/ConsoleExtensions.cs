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
        private static PInvoke.Kernel32.PHANDLER_ROUTINE consoleCtrlHandler;
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

            consoleCtrlHandler = new PInvoke.Kernel32.PHANDLER_ROUTINE(Console_CtrlEvent);
            PInvoke.Kernel32.SetConsoleCtrlHandler(consoleCtrlHandler, true);

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
        private delegate void Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375();

        ///// <summary>
        ///// The PHANDLER_ROUTINE type defines a pointer a callback function to handle control signals
        ///// received by the process. When the signal is received, the system creates a new thread in
        ///// the process to execute the function.
        ///// <para>
        ///// Because the system creates a new thread in the process to execute the handler function,
        ///// it is possible that the handler function will be terminated by another thread in the
        ///// process. Be sure to synchronize threads in the process with the thread for the handler function.
        ///// </para>
        ///// <para>
        ///// Each console process has its own list of HandlerRoutine functions. Initially, this list
        ///// contains only a default handler function that calls ExitProcess. A console process adds
        ///// or removes additional handler functions by calling the SetConsoleCtrlHandler function,
        ///// which does not affect the list of handler functions for other processes. When a console
        ///// process receives any of the control signals, its handler functions are called on a
        ///// last-registered, first-called basis until one of the handlers returns TRUE. If none of
        ///// the handlers returns TRUE, the default handler is called.
        ///// </para>
        ///// </summary>
        ///// <param name="dwCtrlType">
        ///// The type of control signal received by the handler.
        ///// <para>
        ///// The CTRL_CLOSE_EVENT, CTRL_LOGOFF_EVENT, and CTRL_SHUTDOWN_EVENT signals give the process
        ///// an opportunity to clean up before termination. A HandlerRoutine can perform any necessary
        ///// cleanup, then take one of the following actions:
        ///// <list>
        ///// <item>Call the ExitProcess function to terminate the process</item>
        ///// <item>
        ///// Return FALSE.If none of the registered handler functions returns TRUE, the default
        ///// handler terminates the process
        ///// </item>
        ///// <item>
        ///// Return TRUE. In this case, no other handler functions are called and the system
        ///// terminates the process
        ///// </item>
        ///// </list>
        ///// </para>
        ///// <para>
        ///// A process can use the SetProcessShutdownParameters function to prevent the system from
        ///// displaying a dialog box to the user during logoff or shutdown. In this case, the system
        ///// terminates the process when HandlerRoutine returns TRUE or when the time-out period elapses.
        ///// </para>
        ///// </param>
        ///// <returns>
        ///// If the function handles the control signal, it should return TRUE. If it returns FALSE,
        ///// the next handler function in the list of handlers for this process is used.
        ///// </returns>
        ///// <remarks>
        ///// Note that a third-party library or DLL can install a console control handler for your
        ///// application. If it does, this handler overrides the default handler, and can cause the
        ///// application to exit when the user logs off.
        ///// <para>
        ///// Windows 7, Windows 8, Windows 8.1 and Windows 10: If a console application loads the
        ///// gdi32.dll or user32.dll library, the HandlerRoutine function that you specify when you
        ///// call SetConsoleCtrlHandler does not get called for the CTRL_LOGOFF_EVENT and
        ///// CTRL_SHUTDOWN_EVENT events.
        ///// </para>
        ///// <para>
        ///// The operating system recognizes processes that load gdi32.dll or user32.dll as Windows
        ///// applications rather than console applications. This behavior also occurs for console
        ///// applications that do not call functions in gdi32.dll or user32.dll directly, but do call
        ///// functions such as Shell functions that do in turn call functions in gdi32.dll or user32.dll.
        ///// </para>
        ///// <para>
        ///// To receive events when a user signs out or the device shuts down in these circumstances,
        ///// create a hidden window in your console application, and then handle the
        ///// WM_QUERYENDSESSION and WM_ENDSESSION window messages that the hidden window receives. You
        ///// can create a hidden window by calling the CreateWindowEx method with the dwExStyle
        ///// parameter set to 0.
        ///// </para>
        ///// <para>
        ///// When a console application is run as a service, it receives a modified default console
        ///// control handler. This modified handler does not call ExitProcess when processing the
        ///// CTRL_LOGOFF_EVENT and CTRL_SHUTDOWN_EVENT signals. This allows the service to continue
        ///// running after the user logs off. If the service installs its own console control handler,
        ///// this handler is called before the default handler. If the installed handler calls
        ///// ExitProcess when processing the CTRL_LOGOFF_EVENT signal, the service exits when the user
        ///// logs off.
        ///// </para>
        ///// </remarks>
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private delegate bool PHANDLER_ROUTINE(CtrlType dwCtrlType);

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

        /// Return Type: int
        ///param0: Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375
        [DllImport("msvcr80.dll", EntryPoint = "atexit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int atexit(Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375 param0);

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

        private static bool Console_CtrlEvent(PInvoke.Kernel32.ControlType sig)
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

            PInvoke.Kernel32.SetConsoleCtrlHandler(consoleCtrlHandler, false);
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

        [StructLayout(LayoutKind.Sequential)]
        private struct CHAR_INFO
        {
            /// Anonymous_f3630dcb_df39_4f30_a593_48e610e9363d
            public EventCharacterUnion Char;

            /// WORD->unsigned short
            public ushort Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_FONT_INFO
        {
            /// DWORD->unsigned int
            public uint nFont;

            /// COORD->_COORD
            public COORD dwFontSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SCREEN_BUFFER_INFO
        {
            /// COORD->_COORD
            public COORD dwSize;

            /// COORD->_COORD
            public COORD dwCursorPosition;

            /// WORD->unsigned short
            public ushort wAttributes;

            /// SMALL_RECT->_SMALL_RECT
            public SMALL_RECT srWindow;

            /// COORD->_COORD
            public COORD dwMaximumWindowSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CONSOLE_SELECTION_INFO
        {
            /// DWORD->unsigned int
            public uint dwFlags;

            /// COORD->_COORD
            public COORD dwSelectionAnchor;

            /// SMALL_RECT->_SMALL_RECT
            public SMALL_RECT srSelection;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ConsoleEvents
        {
            /// KEY_EVENT_RECORD->_KEY_EVENT_RECORD
            [FieldOffset(0)]
            public KEY_EVENT_RECORD KeyEvent;

            /// MOUSE_EVENT_RECORD->_MOUSE_EVENT_RECORD
            [FieldOffset(0)]
            public MOUSE_EVENT_RECORD MouseEvent;

            /// WINDOW_BUFFER_SIZE_RECORD->_WINDOW_BUFFER_SIZE_RECORD
            [FieldOffset(0)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;

            /// MENU_EVENT_RECORD->_MENU_EVENT_RECORD
            [FieldOffset(0)]
            public MENU_EVENT_RECORD MenuEvent;

            /// FOCUS_EVENT_RECORD->_FOCUS_EVENT_RECORD
            [FieldOffset(0)]
            public FOCUS_EVENT_RECORD FocusEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            /// SHORT->short
            public short X;

            /// SHORT->short
            public short Y;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct EventCharacterUnion
        {
            /// WCHAR->wchar_t->unsigned short
            [FieldOffset(0)]
            public ushort UnicodeChar;

            /// CHAR->char
            [FieldOffset(0)]
            public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FOCUS_EVENT_RECORD
        {
            /// BOOL->int
            [MarshalAs(UnmanagedType.Bool)]
            public bool bSetFocus;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT_RECORD
        {
            /// WORD->unsigned short
            public ushort EventType;

            /// Anonymous_79fe9041_6876_475e_b93a_ffb0d7822836
            public ConsoleEvents Event;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEY_EVENT_RECORD
        {
            /// BOOL->int
            [MarshalAs(UnmanagedType.Bool)]
            public bool bKeyDown;

            /// WORD->unsigned short
            public ushort wRepeatCount;

            /// WORD->unsigned short
            public ushort wVirtualKeyCode;

            /// WORD->unsigned short
            public ushort wVirtualScanCode;

            /// Anonymous_ee4ad878_dde2_4d9b_b7de_b1588db350c7
            public EventCharacterUnion uChar;

            /// DWORD->unsigned int
            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MENU_EVENT_RECORD
        {
            /// UINT->unsigned int
            public uint dwCommandId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            /// COORD->_COORD
            public COORD dwMousePosition;

            /// DWORD->unsigned int
            public uint dwButtonState;

            /// DWORD->unsigned int
            public uint dwControlKeyState;

            /// DWORD->unsigned int
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            /// DWORD->unsigned int
            public uint nLength;

            /// LPVOID->void*
            public IntPtr lpSecurityDescriptor;

            /// BOOL->int
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SMALL_RECT
        {
            /// SHORT->short
            public short Left;

            /// SHORT->short
            public short Top;

            /// SHORT->short
            public short Right;

            /// SHORT->short
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOW_BUFFER_SIZE_RECORD
        {
            /// COORD->_COORD
            public COORD dwSize;
        }
    }
}