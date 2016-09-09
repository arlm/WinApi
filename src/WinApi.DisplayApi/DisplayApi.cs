// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PInvoke;
using WinApi.Core;
using WinApi.User32;
using static PInvoke.User32;

namespace WinApi
{
    public class DisplayApi
    {
        // // On expand, if we're given a window_rect, grow to it, otherwise do // not resize. if
        // (!for_metro) { MONITORINFO monitor_info; monitor_info.cbSize = sizeof(monitor_info);
        // GetMonitorInfo(MonitorFromWindow(hwnd_, MONITOR_DEFAULTTONEAREST), &monitor_info);
        // gfx::Rect window_rect(monitor_info.rcMonitor); SetWindowPos(hwnd_, NULL, window_rect.x(),
        // window_rect.y(), window_rect.width(), window_rect.height(), SWP_NOZORDER | SWP_NOACTIVATE
        // | SWP_FRAMECHANGED); } } else { // Reset original window style and size. The multiple
        // window size/moves // here are ugly, but if SetWindowPos() doesn't redraw, the taskbar
        // won't be // repainted. Better-looking methods welcome. SetWindowLong(hwnd_, GWL_STYLE,
        // saved_window_info_.style); SetWindowLong(hwnd_, GWL_EXSTYLE, saved_window_info_.ex_style);

        // http://src.chromium.org/viewvc/chrome/trunk/src/ui/views/win/fullscreen_handler.cc?revision=HEAD&view=markup
        // void FullscreenHandler::SetFullscreenImpl(bool fullscreen, bool for_metro) {
        // ScopedFullscreenVisibility visibility(hwnd_);

        // // Save current window state if not already fullscreen. if (!fullscreen_) { // Save
        // current window information. We force the window into restored mode // before going
        // fullscreen because Windows doesn't seem to hide the // taskbar if the window is in the
        // maximized state. saved_window_info_.maximized = !!::IsZoomed(hwnd_); if
        // (saved_window_info_.maximized) ::SendMessage(hwnd_, WM_SYSCOMMAND, SC_RESTORE, 0);
        // saved_window_info_.style = GetWindowLong(hwnd_, GWL_STYLE); saved_window_info_.ex_style =
        // GetWindowLong(hwnd_, GWL_EXSTYLE); GetWindowRect(hwnd_, &saved_window_info_.window_rect); }

        // fullscreen_ = fullscreen;

        //      if (fullscreen_)
        //      {
        //          // Set new window style and size.
        //          SetWindowLong(hwnd_, GWL_STYLE,
        //                        saved_window_info_.style & ~(WS_CAPTION | WS_THICKFRAME));
        //          SetWindowLong(hwnd_, GWL_EXSTYLE,
        //                        saved_window_info_.ex_style & ~(WS_EX_DLGMODALFRAME |
        //                        WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE));

        public static IntPtr CreateFullscreenWindow(string className, string windowName)
        {
            return CreateFullscreenWindow(IntPtr.Zero, className, windowName, IntPtr.Zero);
        }

        public static IntPtr CreateFullscreenWindow(IntPtr hwndParent, string className, string windowName)
        {
            return CreateFullscreenWindow(hwndParent, className, windowName, IntPtr.Zero);
        }

        public static unsafe IntPtr CreateFullscreenWindow(IntPtr hwndParent, string className, string windowName, IntPtr hInstance)
        {
            if (hInstance == IntPtr.Zero)
            {
                hInstance = Process.GetCurrentProcess().Handle;
            }

            if (hwndParent == IntPtr.Zero)
            {
                hwndParent = Process.GetCurrentProcess().MainWindowHandle;
            }

            var hmon = MonitorFromWindow(hwndParent, MonitorOptions.MONITOR_DEFAULTTONEAREST);
            var mi = new MONITORINFO();

            if (!GetMonitorInfo(hmon, new IntPtr(&mi)))
            {
                return IntPtr.Zero;
            }

            return WindowApi.CreateWindow(
                className,
                windowName,
                WindowStyles.WS_POPUP | WindowStyles.WS_VISIBLE,
                mi.rcMonitor.left,
                mi.rcMonitor.top,
                mi.rcMonitor.right - mi.rcMonitor.left,
                mi.rcMonitor.bottom - mi.rcMonitor.top,
                hwndParent,
                IntPtr.Zero,
                hInstance,
                IntPtr.Zero);
        }

        public static bool IsForegroundWindowFullScreen()
        {
            IntPtr hWnd;

            hWnd = PInvoke.User32.GetForegroundWindow();

            return IsFullScreenWindow(hWnd);
        }

        public static bool IsFullScreenWindow(IntPtr hWnd)
        {
            var desktopHandle = GetDesktopWindow();
            var shellHandle = GetShellWindow();

            bool runningFullScreen = false;

            if (!hWnd.Equals(IntPtr.Zero))
            {
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle)))
                {
                    RECT appBounds = WindowApi.GetWindowRectangle(hWnd);

                    var screenBounds = Screen.FromHandle(hWnd).Bounds;

                    if (appBounds.ToRectangle() == screenBounds)
                    {
                        runningFullScreen = true;
                    }
                }
            }

            return runningFullScreen;
        }

        // you use Windows Presentation Foundation you'll need WindowInteropHelper to get Window
        // handle. Make sure you referenced PresentationFramework assembly. Insert this into Using
        // block using System.Windows.Interop; Create instance of WindowInteropHelper
        // WindowInteropHelper winHelp = new WindowInteropHelper(target); Then use winHelp.Handle
        // insted of GetActiveWindowHandle(). To bad that this cannot be resized out of the fixed
        // screen resolution size.
        public static void MoveWindowToMonitor(int monitor, IntPtr hwnd)
        {
            var windowRec = WindowApi.GetWindowRect(hwnd);

            // When I move a window to a different monitor it subtracts 16 from the Width and 38 from
            // the Height, Not sure if this is on my system or others.
            PInvoke.User32.SetWindowPos(
                hwnd,
                (IntPtr)WindowApi.SpecialWindowHandles.HWND_TOP,
                Screen.AllScreens[monitor].WorkingArea.Left,
                Screen.AllScreens[monitor].WorkingArea.Top,
                (windowRec.right - windowRec.left) + 16,
                (windowRec.bottom - windowRec.top) + 38,
                PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        public Screen DetectScreen(IntPtr windowHandle)
        {
            // Figure out which screen the window is located on (in a multi screen setup)
            var appWindowBounds = WindowApi.GetWindowRectangle(windowHandle).ToRectangle();
            foreach (var screen in Screen.AllScreens)
            {
                // If the app is not fullscreen then the screen.bounds will contain the window if the
                // app is fullscreen then IT will actually contain the screen bounds
                if (screen.Bounds.Contains(appWindowBounds) ||
                    appWindowBounds.Contains(screen.Bounds))
                {
                    return screen;
                }
            }

            // By default use the primary screen
            return Screen.PrimaryScreen;
        }

        /// <summary>
        /// Returns the number of Displays using the Win32 functions
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public unsafe List<DisplayInfo> GetDisplays()
        {
            var col = new List<DisplayInfo>();

            EnumDisplayMonitors(
                IntPtr.Zero,
                IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
                {
                    var mi = new MONITORINFO();
                    mi.cbSize = Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, new IntPtr(&mi));

                    if (success)
                    {
                        var di = new DisplayInfo();
                        di.ScreenWidth = (mi.rcMonitor.right - mi.rcMonitor.left).ToString();
                        di.ScreenHeight = (mi.rcMonitor.bottom - mi.rcMonitor.top).ToString();
                        di.MonitorArea = mi.rcMonitor;
                        di.WorkArea = mi.rcWork;
                        di.Availability = mi.dwFlags.ToString();
                        col.Add(di);
                    }

                    return true;
                },
                IntPtr.Zero);
            return col;
        }
    }
}