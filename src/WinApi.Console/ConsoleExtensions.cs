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
using CLR.Extensions;
using ExceptionHandling.Extensions;
using Microsoft.Win32;
using Windows.Core.Extensions;
using ThreadState = System.Threading.ThreadState;

namespace WinApi.Console
{
    public static class ConsoleExtensions
    {
        #region Private Fields

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
        private static PHANDLER_ROUTINE consoleCtrlHandler;
        private static volatile bool hasFaulted = false;
        private static volatile bool hasHandledOnExit = false;
        private static bool isCapturingFirstChanceException = false;

        private volatile static bool marqueeRunning = false;

        private static Thread marqueeThread = null;

        private volatile static bool marqueeWithProgress = true;

        private volatile static float pctComplete = 0;

        private static bool shouldContinueOnBreak = false;

        private static bool shouldPrompt = true;

        private static int spinnerPos = 0;

        #endregion Private Fields

        #region Public Constructors

        static ConsoleExtensions()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;

            var currentProcess = Process.GetCurrentProcess();
            currentProcess.Exited += Process_Exited;
            currentProcess.Disposed += Process_Disposed;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            System.Console.CancelKeyPress += Console_CancelKeyPress;

            consoleCtrlHandler = new PHANDLER_ROUTINE(Console_CtrlEvent);
            SetConsoleCtrlHandler(consoleCtrlHandler, true);

            SystemEvents.EventsThreadShutdown += SystemEvents_EventsThreadShutdown;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            SystemEvents.SessionEnded += SystemEvents_SessionEnded;
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;
            SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
        }

        #endregion Public Constructors

        #region Private Delegates

        /// Return Type: int
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int _onexit_t();

        /// Return Type: void
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375();

        /// <summary>
        /// The PHANDLER_ROUTINE type defines a pointer a callback function to handle control signals
        /// received by the process. When the signal is received, the system creates a new thread in
        /// the process to execute the function.
        /// <para>
        /// Because the system creates a new thread in the process to execute the handler function,
        /// it is possible that the handler function will be terminated by another thread in the
        /// process. Be sure to synchronize threads in the process with the thread for the handler function.
        /// </para>
        /// <para>
        /// Each console process has its own list of HandlerRoutine functions. Initially, this list
        /// contains only a default handler function that calls ExitProcess. A console process adds
        /// or removes additional handler functions by calling the SetConsoleCtrlHandler function,
        /// which does not affect the list of handler functions for other processes. When a console
        /// process receives any of the control signals, its handler functions are called on a
        /// last-registered, first-called basis until one of the handlers returns TRUE. If none of
        /// the handlers returns TRUE, the default handler is called.
        /// </para>
        /// </summary>
        /// <param name="dwCtrlType">
        /// The type of control signal received by the handler.
        /// <para>
        /// The CTRL_CLOSE_EVENT, CTRL_LOGOFF_EVENT, and CTRL_SHUTDOWN_EVENT signals give the process
        /// an opportunity to clean up before termination. A HandlerRoutine can perform any necessary
        /// cleanup, then take one of the following actions:
        /// <list>
        /// <item>Call the ExitProcess function to terminate the process</item>
        /// <item>
        /// Return FALSE.If none of the registered handler functions returns TRUE, the default
        /// handler terminates the process
        /// </item>
        /// <item>
        /// Return TRUE. In this case, no other handler functions are called and the system
        /// terminates the process
        /// </item>
        /// </list>
        /// </para>
        /// <para>
        /// A process can use the SetProcessShutdownParameters function to prevent the system from
        /// displaying a dialog box to the user during logoff or shutdown. In this case, the system
        /// terminates the process when HandlerRoutine returns TRUE or when the time-out period elapses.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function handles the control signal, it should return TRUE. If it returns FALSE,
        /// the next handler function in the list of handlers for this process is used.
        /// </returns>
        /// <remarks>
        /// Note that a third-party library or DLL can install a console control handler for your
        /// application. If it does, this handler overrides the default handler, and can cause the
        /// application to exit when the user logs off.
        /// <para>
        /// Windows 7, Windows 8, Windows 8.1 and Windows 10: If a console application loads the
        /// gdi32.dll or user32.dll library, the HandlerRoutine function that you specify when you
        /// call SetConsoleCtrlHandler does not get called for the CTRL_LOGOFF_EVENT and
        /// CTRL_SHUTDOWN_EVENT events.
        /// </para>
        /// <para>
        /// The operating system recognizes processes that load gdi32.dll or user32.dll as Windows
        /// applications rather than console applications. This behavior also occurs for console
        /// applications that do not call functions in gdi32.dll or user32.dll directly, but do call
        /// functions such as Shell functions that do in turn call functions in gdi32.dll or user32.dll.
        /// </para>
        /// <para>
        /// To receive events when a user signs out or the device shuts down in these circumstances,
        /// create a hidden window in your console application, and then handle the
        /// WM_QUERYENDSESSION and WM_ENDSESSION window messages that the hidden window receives. You
        /// can create a hidden window by calling the CreateWindowEx method with the dwExStyle
        /// parameter set to 0.
        /// </para>
        /// <para>
        /// When a console application is run as a service, it receives a modified default console
        /// control handler. This modified handler does not call ExitProcess when processing the
        /// CTRL_LOGOFF_EVENT and CTRL_SHUTDOWN_EVENT signals. This allows the service to continue
        /// running after the user logs off. If the service installs its own console control handler,
        /// this handler is called before the default handler. If the installed handler calls
        /// ExitProcess when processing the CTRL_LOGOFF_EVENT signal, the service exits when the user
        /// logs off.
        /// </para>
        /// </remarks>
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool PHANDLER_ROUTINE(CtrlType dwCtrlType);

        /// Return Type: void
        ///param0: int[]
        private delegate void signal_function(int[] param0);

        /// Return Type: void
        private delegate void terminate_function();

        /// Return Type: void
        private delegate void unexpected_function();

        #endregion Private Delegates

        #region Public Events

        public static event EventHandler<ConsoleCancelEventArgs> OnCancelKeyPressExit;

        public static event EventHandler<EventArgs> OnProcessExit;

        public static event EventHandler<UnhandledExceptionEventArgs> OnUnhandledException;

        #endregion Public Events

        #region Public Properties

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

        #endregion Public Properties

        #region Public Methods

        public static void SetTitle()
        {
            var assembly = Assembly.GetExecutingAssembly();
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

        #endregion Public Methods

        #region Private Methods

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

        /// Return Type: BOOL->int
        ///Source: LPWSTR->WCHAR*
        ///Target: LPWSTR->WCHAR*
        ///ExeName: LPWSTR->WCHAR*
        [DllImport("kernel32.dll", EntryPoint = "AddConsoleAliasW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddConsoleAliasW([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder Source, [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder Target, [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder ExeName);

        /// Return Type: BOOL->int
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

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

        /// Return Type: BOOL->int
        ///dwProcessId: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "AttachConsole")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AttachConsole(uint dwProcessId);

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

        private static bool Console_CtrlEvent(CtrlType sig)
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

        /// Return Type: HANDLE->void*
        ///dwDesiredAccess: DWORD->unsigned int
        ///dwShareMode: DWORD->unsigned int
        ///lpSecurityAttributes: SECURITY_ATTRIBUTES*
        ///dwFlags: DWORD->unsigned int
        ///lpScreenBufferData: LPVOID->void*
        [DllImport("kernel32.dll", EntryPoint = "CreateConsoleScreenBuffer")]
        private static extern IntPtr CreateConsoleScreenBuffer(uint dwDesiredAccess, uint dwShareMode, ref SECURITY_ATTRIBUTES lpSecurityAttributes, uint dwFlags, IntPtr lpScreenBufferData);

        private static void Dispose()
        {
            StopMarquee();

            CaptureFirstChanceException = false;

            SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            SystemEvents.EventsThreadShutdown -= SystemEvents_EventsThreadShutdown;

            SetConsoleCtrlHandler(consoleCtrlHandler, false);
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

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        [DllImport("kernel32.dll", EntryPoint = "FlushConsoleInputBuffer")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlushConsoleInputBuffer(IntPtr hConsoleInput);

        /// Return Type: BOOL->int
        [DllImport("kernel32.dll", EntryPoint = "FreeConsole")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        /// Return Type: BOOL->int
        ///dwCtrlEvent: DWORD->unsigned int
        ///dwProcessGroupId: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GenerateConsoleCtrlEvent")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        /// Return Type: DWORD->unsigned int
        ///AliasBuffer: LPWSTR->WCHAR*
        ///AliasBufferLength: DWORD->unsigned int
        ///ExeName: LPWSTR->WCHAR*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleAliasesW")]
        private static extern uint GetConsoleAliasesW([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder AliasBuffer, uint AliasBufferLength, [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder ExeName);

        /// Return Type: DWORD->unsigned int
        ///ExeNameBuffer: LPWSTR->WCHAR*
        ///ExeNameBufferLength: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleAliasExesW")]
        private static extern uint GetConsoleAliasExesW([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder ExeNameBuffer, uint ExeNameBufferLength);

        /// Return Type: DWORD->unsigned int
        ///Source: LPWSTR->WCHAR*
        ///TargetBuffer: LPWSTR->WCHAR*
        ///TargetBufferLength: DWORD->unsigned int
        ///ExeName: LPWSTR->WCHAR*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleAliasW")]
        private static extern uint GetConsoleAliasW([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder Source, [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder TargetBuffer, uint TargetBufferLength, [MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder ExeName);

        /// Return Type: UINT->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleCP")]
        private static extern uint GetConsoleCP();

        /// Return Type: BOOL->int
        ///lpModeFlags: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleDisplayMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleDisplayMode(ref uint lpModeFlags);

        /// Return Type: COORD->_COORD
        ///hConsoleOutput: HANDLE->void*
        ///nFont: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleFontSize")]
        private static extern COORD GetConsoleFontSize(IntPtr hConsoleOutput, uint nFont);

        /// Return Type: BOOL->int
        ///hConsoleHandle: HANDLE->void*
        ///lpMode: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, ref uint lpMode);

        /// Return Type: UINT->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleOutputCP")]
        private static extern uint GetConsoleOutputCP();

        /// Return Type: DWORD->unsigned int
        ///lpdwProcessList: LPDWORD->DWORD*
        ///dwProcessCount: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleProcessList")]
        private static extern uint GetConsoleProcessList(ref uint lpdwProcessList, uint dwProcessCount);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///lpConsoleScreenBufferInfo: PCONSOLE_SCREEN_BUFFER_INFO->_CONSOLE_SCREEN_BUFFER_INFO*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

        /// Return Type: BOOL->int
        ///lpConsoleSelectionInfo: PCONSOLE_SELECTION_INFO->_CONSOLE_SELECTION_INFO*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleSelectionInfo")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleSelectionInfo(ref CONSOLE_SELECTION_INFO lpConsoleSelectionInfo);

        /// Return Type: DWORD->unsigned int
        ///lpConsoleTitle: LPWSTR->WCHAR*
        ///nSize: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleTitleW")]
        private static extern uint GetConsoleTitleW([MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder lpConsoleTitle, uint nSize);

        /// Return Type: HWND->HWND__*
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleWindow")]
        private static extern IntPtr GetConsoleWindow();

        /// Return Type: COORD->_COORD
        ///hConsoleOutput: HANDLE->void*
        [DllImport("kernel32.dll", EntryPoint = "GetLargestConsoleWindowSize")]
        private static extern COORD GetLargestConsoleWindowSize(IntPtr hConsoleOutput);

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        ///lpNumberOfEvents: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "GetNumberOfConsoleInputEvents")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, ref uint lpNumberOfEvents);

        private static void MarqueeThread()
        {
            while (marqueeRunning && pctComplete <= 100)
            {
                UpdateProgress(pctComplete);

                Thread.Sleep(250);
            }

            System.Console.Out.WriteLine(Environment.NewLine);
        }

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        ///lpBuffer: PINPUT_RECORD->_INPUT_RECORD*
        ///nLength: DWORD->unsigned int
        ///lpNumberOfEventsRead: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "PeekConsoleInputW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekConsoleInputW(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

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

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        ///lpBuffer: PINPUT_RECORD->_INPUT_RECORD*
        ///nLength: DWORD->unsigned int
        ///lpNumberOfEventsRead: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadConsoleInputW(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///lpBuffer: PCHAR_INFO->_CHAR_INFO*
        ///dwBufferSize: COORD->_COORD
        ///dwBufferCoord: COORD->_COORD
        ///lpReadRegion: PSMALL_RECT->_SMALL_RECT*
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleOutputW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadConsoleOutputW(IntPtr hConsoleOutput, ref CHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpReadRegion);

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        ///lpBuffer: LPVOID->void*
        ///nNumberOfCharsToRead: DWORD->unsigned int
        ///lpNumberOfCharsRead: LPDWORD->DWORD*
        ///lpReserved: LPVOID->void*
        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ReadConsoleW(IntPtr hConsoleInput, IntPtr lpBuffer, uint nNumberOfCharsToRead, ref uint lpNumberOfCharsRead, IntPtr lpReserved);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///lpScrollRectangle: SMALL_RECT*
        ///lpClipRectangle: SMALL_RECT*
        ///dwDestinationOrigin: COORD->_COORD
        ///lpFill: CHAR_INFO*
        [DllImport("kernel32.dll", EntryPoint = "ScrollConsoleScreenBufferW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ScrollConsoleScreenBufferW(IntPtr hConsoleOutput, ref SMALL_RECT lpScrollRectangle, ref SMALL_RECT lpClipRectangle, COORD dwDestinationOrigin, ref CHAR_INFO lpFill);

        /// Return Type: void
        ///param0: terminate_function
        [DllImport("msvcr80.dll", EntryPoint = "set_terminate", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_terminate(terminate_function param0);

        /// Return Type: void
        ///param0: int
        [DllImport("msvcr80.dll", EntryPoint = "set_unexpected", CallingConvention = CallingConvention.Cdecl)]
        private static extern void set_unexpected(int param0);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleActiveScreenBuffer")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

        /// Return Type: BOOL->int
        ///wCodePageID: UINT->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleCP")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleCP(uint wCodePageID);

        /// <summary>
        /// Adds or removes an application-defined HandlerRoutine function from the list of handler
        /// functions for the calling process. If no handler function is specified, the function sets
        /// an inheritable attribute that determines whether the calling process ignores CTRL+C signals.
        /// </summary>
        /// <param name="handlerRoutine">
        /// A pointer to the application-defined HandlerRoutine function to be added or removed. This
        /// parameter can be NULL.
        /// </param>
        /// <param name="add">
        /// <para>
        /// If this parameter is TRUE, the handler is added; if it is FALSE, the handler is removed.
        /// </para>
        /// <para>
        /// If the HandlerRoutine parameter is NULL, a TRUE value causes the calling process to
        /// ignore CTRL+C input, and a FALSE value restores normal processing of CTRL+C input. This
        /// attribute of ignoring or processing CTRL+C is inherited by child processes.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return
        /// value is zero.To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This function provides a similar notification for console application and services that
        /// WM_QUERYENDSESSION provides for graphical applications with a message pump. You could
        /// also use this function from a graphical application, but there is no guarantee it would
        /// arrive before the notification from WM_QUERYENDSESSION.
        /// </para>
        /// <para>
        /// Each console process has its own list of application-defined HandlerRoutine functions
        /// that handle CTRL+C and CTRL+BREAK signals. The handler functions also handle signals
        /// generated by the system when the user closes the console, logs off, or shuts down the
        /// system. Initially, the handler list for each process contains only a default handler
        /// function that calls the ExitProcess function. A console process adds or removes
        /// additional handler functions by calling the SetConsoleCtrlHandler function, which does
        /// not affect the list of handler functions for other processes. When a console process
        /// receives any of the control signals, its handler functions are called on a
        /// last-registered, first-called basis until one of the handlers returns TRUE. If none of
        /// the handlers returns TRUE, the default handler is called.
        /// </para>
        /// </remarks>
        [DllImport("Kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleCtrlHandler(PHANDLER_ROUTINE handlerRoutine, [MarshalAs(UnmanagedType.Bool)] bool add);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///dwCursorPosition: COORD->_COORD
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleCursorPosition")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        /// Return Type: BOOL->int
        ///hConsoleHandle: HANDLE->void*
        ///dwMode: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// Return Type: BOOL->int
        ///wCodePageID: UINT->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleOutputCP")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///wAttributes: WORD->unsigned short
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleTextAttribute")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, ushort wAttributes);

        /// Return Type: BOOL->int
        ///lpConsoleTitle: LPCWSTR->WCHAR*
        [DllImport("kernel32.dll", EntryPoint = "SetConsoleTitleW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleTitleW([MarshalAs(UnmanagedType.LPWStr)] string lpConsoleTitle);

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

        private static void UpdateProgress(float pctComplete)
        {
            if (pctComplete >= 100f)
            {
                System.Console.Out.WriteLine("\rProcess Complete!".PadRight(System.Console.BufferWidth));
            }
            else
            {
                string marquee;

                if (marqueeWithProgress)
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]} - {pctComplete:0.0}%";
                }
                else
                {
                    marquee = $"\rWorking... {spinner[spinnerPos]}";
                }

                System.Console.Out.Write(marquee.PadRight(System.Console.BufferWidth));
                spinnerPos = (spinnerPos >= 3) ? 0 : spinnerPos + 1;
            }
        }

        /// Return Type: BOOL->int
        ///hConsoleInput: HANDLE->void*
        ///lpBuffer: INPUT_RECORD*
        ///nLength: DWORD->unsigned int
        ///lpNumberOfEventsWritten: LPDWORD->DWORD*
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteConsoleInputW(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///lpBuffer: CHAR_INFO*
        ///dwBufferSize: COORD->_COORD
        ///dwBufferCoord: COORD->_COORD
        ///lpWriteRegion: PSMALL_RECT->_SMALL_RECT*
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteConsoleOutputW(IntPtr hConsoleOutput, ref CHAR_INFO lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);

        /// Return Type: BOOL->int
        ///hConsoleOutput: HANDLE->void*
        ///lpBuffer: void*
        ///nNumberOfCharsToWrite: DWORD->unsigned int
        ///lpNumberOfCharsWritten: LPDWORD->DWORD*
        ///lpReserved: LPVOID->void*
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WriteConsoleW(IntPtr hConsoleOutput, IntPtr lpBuffer, uint nNumberOfCharsToWrite, ref uint lpNumberOfCharsWritten, IntPtr lpReserved);

        /// Return Type: DWORD->unsigned int
        [DllImport("kernel32.dll", EntryPoint = "WTSGetActiveConsoleSessionId")]
        private static extern uint WTSGetActiveConsoleSessionId();

        #endregion Private Methods

        #region Private Structs

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

        #endregion Private Structs
    }
}