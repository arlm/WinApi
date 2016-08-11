// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using PInvoke;

namespace Sandbox
{
    public class User32
    {
        // size of a device name string
        public const int CCHDEVICENAME = 32;

        public const int CW_USEDEFAULT = unchecked((int)0x80000000);
        public const int LF_FACESIZE = 32;

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT Point);

        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, POINT pt, WindowFromPointFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        /// <summary>
        /// The CreateWindowEx function creates an overlapped, pop-up, or child window with an
        /// extended window style; otherwise, this function is identical to the CreateWindow function.
        /// </summary>
        /// <param name="dwExStyle">Specifies the extended window style of the window being created.</param>
        /// <param name="lpClassName">
        /// Pointer to a null-terminated string or a class atom created by a previous call to the
        /// RegisterClass or RegisterClassEx function. The atom must be in the low-order word of
        /// lpClassName; the high-order word must be zero. If lpClassName is a string, it specifies
        /// the window class name. The class name can be any name registered with RegisterClass or
        /// RegisterClassEx, provided that the module that registers the class is also the module
        /// that creates the window. The class name can also be any of the predefined system class names.
        /// </param>
        /// <param name="lpWindowName">
        /// Pointer to a null-terminated string that specifies the window name. If the window style
        /// specifies a title bar, the window title pointed to by lpWindowName is displayed in the
        /// title bar. When using CreateWindow to create controls, such as buttons, check boxes, and
        /// static controls, use lpWindowName to specify the text of the control. When creating a
        /// static control with the SS_ICON style, use lpWindowName to specify the icon name or
        /// identifier. To specify an identifier, use the syntax "#num".
        /// </param>
        /// <param name="dwStyle">
        /// Specifies the style of the window being created. This parameter can be a combination of
        /// window styles, plus the control styles indicated in the Remarks section.
        /// </param>
        /// <param name="x">
        /// Specifies the initial horizontal position of the window. For an overlapped or pop-up
        /// window, the x parameter is the initial x-coordinate of the window's upper-left corner, in
        /// screen coordinates. For a child window, x is the x-coordinate of the upper-left corner of
        /// the window relative to the upper-left corner of the parent window's client area. If x is
        /// set to CW_USEDEFAULT, the system selects the default position for the window's upper-left
        /// corner and ignores the y parameter. CW_USEDEFAULT is valid only for overlapped windows;
        /// if it is specified for a pop-up or child window, the x and y parameters are set to zero.
        /// </param>
        /// <param name="y">
        /// Specifies the initial vertical position of the window. For an overlapped or pop-up
        /// window, the y parameter is the initial y-coordinate of the window's upper-left corner, in
        /// screen coordinates. For a child window, y is the initial y-coordinate of the upper-left
        /// corner of the child window relative to the upper-left corner of the parent window's
        /// client area. For a list box y is the initial y-coordinate of the upper-left corner of the
        /// list box's client area relative to the upper-left corner of the parent window's client area.
        /// <para>
        /// If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter
        /// is set to CW_USEDEFAULT, then the y parameter determines how the window is shown. If the
        /// y parameter is CW_USEDEFAULT, then the window manager calls ShowWindow with the SW_SHOW
        /// flag after the window has been created. If the y parameter is some other value, then the
        /// window manager calls ShowWindow with that value as the nCmdShow parameter.
        /// </para>
        /// </param>
        /// <param name="nWidth">
        /// Specifies the width, in device units, of the window. For overlapped windows, nWidth is
        /// the window's width, in screen coordinates, or CW_USEDEFAULT. If nWidth is CW_USEDEFAULT,
        /// the system selects a default width and height for the window; the default width extends
        /// from the initial x-coordinates to the right edge of the screen; the default height
        /// extends from the initial y-coordinate to the top of the icon area. CW_USEDEFAULT is valid
        /// only for overlapped windows; if CW_USEDEFAULT is specified for a pop-up or child window,
        /// the nWidth and nHeight parameter are set to zero.
        /// </param>
        /// <param name="nHeight">
        /// Specifies the height, in device units, of the window. For overlapped windows, nHeight is
        /// the window's height, in screen coordinates. If the nWidth parameter is set to
        /// CW_USEDEFAULT, the system ignores nHeight.
        /// </param>
        /// <param name="hWndParent">
        /// Handle to the parent or owner window of the window being created. To create a child
        /// window or an owned window, supply a valid window handle. This parameter is optional for
        /// pop-up windows.
        /// <para>
        /// Windows 2000/XP: To create a message-only window, supply HWND_MESSAGE or a handle to an
        /// existing message-only window.
        /// </para>
        /// </param>
        /// <param name="hMenu">
        /// Handle to a menu, or specifies a child-window identifier, depending on the window style.
        /// For an overlapped or pop-up window, hMenu identifies the menu to be used with the window;
        /// it can be NULL if the class menu is to be used. For a child window, hMenu specifies the
        /// child-window identifier, an integer value used by a dialog box control to notify its
        /// parent about events. The application determines the child-window identifier; it must be
        /// unique for all child windows with the same parent window.
        /// </param>
        /// <param name="hInstance">
        /// Handle to the instance of the module to be associated with the window.
        /// </param>
        /// <param name="lpParam">
        /// Pointer to a value to be passed to the window through the CREATESTRUCT structure
        /// (lpCreateParams member) pointed to by the lParam param of the WM_CREATE message. This
        /// message is sent to the created window by this function before it returns.
        /// <para>
        /// If an application calls CreateWindow to create a MDI client window, lpParam should point
        /// to a CLIENTCREATESTRUCT structure. If an MDI client window calls CreateWindow to create
        /// an MDI child window, lpParam should point to a MDICREATESTRUCT structure. lpParam may be
        /// NULL if no additional data is needed.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the new window.
        /// <para>
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </para>
        /// <para>This function typically fails for one of the following reasons:</para>
        /// <list type="">
        /// <item>an invalid parameter value</item>
        /// <item>the system class was registered by a different module</item>
        /// <item>The WH_CBT hook is installed and returns a failure code</item>
        /// <item>
        /// if one of the controls in the dialog template is not registered, or its window window
        /// procedure fails WM_CREATE or WM_NCCREATE
        /// </item>
        /// </list>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        // When you call this function, the WndProc function must respond to the WM_NCCREATE message
        // by returning TRUE. If it does not, the creation process will fail. A null handle will be
        // returned from CreateWindowEx and GetLastError will return 0. See MSDN on WM_NCCREATE
        // (http://msdn.microsoft.com/en-us/library/ms632635.aspx) and also WM_CREATE
        // (http://msdn.microsoft.com/en-us/library/ms632619.aspx). You can have your WndProc call
        // DefWindowProc, which will take care of this issue. Tips & Tricks: This method should have
        // at least two versions: one where lpClassName is string, and other in which is IntPtr. I
        // sometimes get error 1407 ("Cannot find window class.") when trying to create the
        // window(after a successful call to RegisterClass) Solution is to pass in the second version
        // the class as new IntPtr((int)(uint)regResult), where regResult is the result from RegisterClass
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateWindowEx(
                   int dwExStyle,
                   string lpClassName,
                   string lpWindowName,
                   WindowStyles dwStyle,
                   int x,
                   int y,
                   int nWidth,
                   int nHeight,
                   IntPtr hWndParent,
                   IntPtr hMenu,
                   IntPtr hInstance,
                   IntPtr lpParam);

        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, PInvoke.User32.WindowMessage uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr DispatchMessage(ref MSG lpmsg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, TextFormats uFormat);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawTextEx(IntPtr hdc, StringBuilder lpchText, int cchText, ref RECT lprc, uint dwDTFormat, ref DRAWTEXTPARAMS lpDTParams);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool GetClassInfoEx(IntPtr hInstance, string lpClassName, ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently
        /// working). The system assigns a slightly higher priority to the thread that creates the
        /// foreground window than it does to other threads.
        /// <para>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633505%28v=vs.85%29.aspx
        /// for more information.
        /// </para>
        /// </summary>
        /// <returns>
        /// C++ ( Type: Type: HWND ) <br/> The return value is a handle to the foreground window. The
        /// foreground window can be NULL in certain circumstances, such as when a window is losing activation.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("user32")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        /// <summary>
        /// Retrieves a handle to the Shell's desktop window.
        /// <para>
        /// Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633512%28v=vs.85%29.aspx
        /// for more information
        /// </para>
        /// </summary>
        /// <returns>
        /// C++ ( Type: HWND ) <br/> The return value is the handle of the Shell's desktop window. If
        /// no Shell process is present, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetWindowDisplayAffinity(IntPtr hWnd, out int dwAffinity);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the
        /// specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        /// <para>
        /// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT).
        /// GetWindowPlacement fails if lpwndpl-&gt; length is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsGUIThread([MarshalAs(UnmanagedType.Bool)] bool bConvert);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsHungAppWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowUnicode(IntPtr hWnd);

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// <para>
        /// Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633530%28v=vs.85%29.aspx
        /// for more information. For WS_VISIBLE information go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600%28v=vs.85%29.aspx
        /// </para>
        /// </summary>
        /// <param name="hWnd">
        /// C++ ( hWnd Type: HWND ) <br/> A handle to the window to be tested.
        /// </param>
        /// <returns>
        /// <c>true</c> or the return value is nonzero if the specified window, its parent window,
        /// its parent's parent window, and so forth, have the WS_VISIBLE style; otherwise,
        /// <c>false</c> or the return value is zero.
        /// </returns>
        /// <remarks>
        /// The visibility state of a window is indicated by the WS_VISIBLE[0x10000000L] style bit.
        /// When WS_VISIBLE[0x10000000L] is set, the window is displayed and subsequent drawing into
        /// it is displayed as long as the window has the WS_VISIBLE[0x10000000L] style. Any drawing
        /// to a window with the WS_VISIBLE[0x10000000L] style will not be displayed if the window is
        /// obscured by other windows or is clipped by its parent window.
        /// </remarks>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

        //For loading icon-resource identified as integer ID
        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

        [DllImport("user32", SetLastError = true)]
        public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string text, string caption, MessageBoxOptions options);

        //http://www.codeproject.com/Articles/8460/Advanced-MessageBoxing-with-the-C-MessageBoxIndire
        [DllImport("user32.dll")]
        public static extern int MessageBoxIndirect(ref MSGBOXPARAMS lpMsgBoxParams);

        //#if(WINVER >= 0x0600)
        //OSVERSIONINFO osvi;
        //memset(&osvi,0,sizeof(osvi));
        //osvi.dwOSVersionInfoSize = sizeof(osvi);
        //GetVersionEx(&osvi);
        //if (osvi.dwMajorVersion < 6)
        //ncm.cbSize -= sizeof(ncm.iPaddedBorderWidth);
        //#endif
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr MonitorFromPoint(POINT point, int flags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect(ref RECT lprc, MonitorOptions dwFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorOptions dwFlags);

        //MSDN recommends using PostQuitMessage over PostMessage when you want to send WM_QUIT http://msdn.microsoft.com/en-us/library/ms632641%28v=vs.85%29.aspx
        [DllImport("user32.dll")]
        public static extern void PostQuitMessage(int nExitCode);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(uint threadId, PInvoke.User32.WindowMessage msg, UIntPtr wParam, IntPtr lParam);

        /// Return Type: LONG->int
        ///Flags: UINT32->unsigned int
        ///pNumPathArrayElements: UINT32*
        ///pPathInfoArray: DISPLAYCONFIG_PATH_INFO*
        ///pNumModeInfoArrayElements: UINT32*
        ///pModeInfoArray: DISPLAYCONFIG_MODE_INFO*
        ///pCurrentTopologyId: DISPLAYCONFIG_TOPOLOGY_ID*
        [DllImport("user32.dll")]
        public static extern int QueryDisplayConfig(uint Flags, ref uint pNumPathArrayElements, ref DISPLAYCONFIG_PATH_INFO pPathInfoArray, ref uint pNumModeInfoArrayElements, ref DISPLAYCONFIG_MODE_INFO pModeInfoArray, ref DISPLAYCONFIG_TOPOLOGY_ID pCurrentTopologyId);

        [DllImport("user32.dll")]
        public static extern IntPtr RealChildWindowFromPoint(IntPtr hwndParent, POINT ptParentClientCoords);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint RealGetWindowClass(IntPtr hwnd, [Out] StringBuilder pszType, uint cchType);

        [DllImport("user32.dll")]
        public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern short RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool SetWindowDisplayAffinity(IntPtr hWnd, int dwAffinity);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These
        /// windows are ordered according to their appearance on the screen. The topmost window
        /// receives the highest rank and is the first window in the Z order.
        /// <para>
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx
        /// for more information.
        /// </para>
        /// </summary>
        /// <param name="hWnd">C++ ( hWnd Type: HWND ) <br/> A handle to the window.</param>
        /// <param name="hWndInsertAfter">
        /// C++ ( hWndInsertAfter Type: HWND ) <br/> A handle to the window to
        /// precede the positioned window in the Z order. This parameter must be a window handle or
        /// one of the following values.
        /// <list type="table">
        /// <itemheader>
        /// <term>HWND placement</term>
        /// <description>Window to precede placement</description>
        /// </itemheader>
        /// <item>
        /// <term>HWND_BOTTOM ((HWND)1)</term>
        /// <description>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a
        /// topmost window, the window loses its topmost status and is placed at the bottom of all
        /// other windows.
        /// </description>
        /// </item>
        /// <item>
        /// <term>HWND_NOTOPMOST ((HWND)-2)</term>
        /// <description>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows).
        /// This flag has no effect if the window is already a non-topmost window.
        /// </description>
        /// </item>
        /// <item>
        /// <term>HWND_TOP ((HWND)0)</term>
        /// <description>Places the window at the top of the Z order.</description>
        /// </item>
        /// <item>
        /// <term>HWND_TOPMOST ((HWND)-1)</term>
        /// <description>
        /// Places the window above all non-topmost windows. The window maintains its topmost
        /// position even when it is deactivated.
        /// </description>
        /// </item>
        /// </list>
        /// <para>
        /// For more information about how this parameter is used, see the following Remarks section.
        /// </para>
        /// </param>
        /// <param name="X">
        /// C++ ( X Type: int ) <br/> The new position of the left side of the window, in
        /// client coordinates.
        /// </param>
        /// <param name="Y">
        /// C++ ( Y Type: int ) <br/> The new position of the top of the window, in client coordinates.
        /// </param>
        /// <param name="cx">C++ ( cx Type: int ) <br/> The new width of the window, in pixels.</param>
        /// <param name="cy">
        /// C++ ( cy Type: int ) <br/> The new height of the window, in pixels.
        /// </param>
        /// <param name="uFlags">
        /// C++ ( uFlags Type: UINT ) <br/> The window sizing and positioning flags. This
        /// parameter can be a combination of the following values.
        /// <list type="table">
        /// <itemheader>
        /// <term>HWND sizing and positioning flags</term>
        /// <description>Where to place and size window. Can be a combination of any</description>
        /// </itemheader>
        /// <item>
        /// <term>SWP_ASYNCWINDOWPOS (0x4000)</term>
        /// <description>
        /// If the calling thread and the thread that owns the window are attached to different input
        /// queues, the system posts the request to the thread that owns the window. This prevents
        /// the calling thread from blocking its execution while other threads process the request.
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_DEFERERASE (0x2000)</term>
        /// <description>Prevents generation of the WM_SYNCPAINT message.</description>
        /// </item>
        /// <item>
        /// <term>SWP_DRAWFRAME (0x0020)</term>
        /// <description>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_FRAMECHANGED (0x0020)</term>
        /// <description>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE
        /// message to the window, even if the window's size is not being changed. If this flag is
        /// not specified, WM_NCCALCSIZE is sent only when the window's size is being changed
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_HIDEWINDOW (0x0080)</term>
        /// <description>Hides the window.</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOACTIVATE (0x0010)</term>
        /// <description>
        /// Does not activate the window. If this flag is not set, the window is activated and moved
        /// to the top of either the topmost or non-topmost group (depending on the setting of the
        /// hWndInsertAfter parameter).
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_NOCOPYBITS (0x0100)</term>
        /// <description>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid
        /// contents of the client area are saved and copied back into the client area after the
        /// window is sized or repositioned.
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_NOMOVE (0x0002)</term>
        /// <description>Retains the current position (ignores X and Y parameters).</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOOWNERZORDER (0x0200)</term>
        /// <description>Does not change the owner window's position in the Z order.</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOREDRAW (0x0008)</term>
        /// <description>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This
        /// applies to the client area, the nonclient area (including the title bar and scroll bars),
        /// and any part of the parent window uncovered as a result of the window being moved. When
        /// this flag is set, the application must explicitly invalidate or redraw any parts of the
        /// window and parent window that need redrawing.
        /// </description>
        /// </item>
        /// <item>
        /// <term>SWP_NOREPOSITION (0x0200)</term>
        /// <description>Same as the SWP_NOOWNERZORDER flag.</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOSENDCHANGING (0x0400)</term>
        /// <description>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOSIZE (0x0001)</term>
        /// <description>Retains the current size (ignores the cx and cy parameters).</description>
        /// </item>
        /// <item>
        /// <term>SWP_NOZORDER (0x0004)</term>
        /// <description>Retains the current Z order (ignores the hWndInsertAfter parameter).</description>
        /// </item>
        /// <item>
        /// <term>SWP_SHOWWINDOW (0x0040)</term>
        /// <description>Displays the window.</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>
        /// <c>true</c> or nonzero if the function succeeds, <c>false</c> or zero otherwise or if
        /// function fails.
        /// </returns>
        /// <remarks>
        /// <para>
        /// As part of the Vista re-architecture, all services were moved off the interactive desktop
        /// into Session 0. hwnd and window manager operations are only effective inside a session
        /// and cross-session attempts to manipulate the hwnd will fail. For more information, see
        /// The Windows Vista Developer Story: Application Compatibility Cookbook.
        /// </para>
        /// <para>
        /// If you have changed certain window data using SetWindowLong, you must call SetWindowPos
        /// for the changes to take effect. Use the following combination for uFlags: SWP_NOMOVE |
        /// SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED.
        /// </para>
        /// <para>
        /// A window can be made a topmost window either by setting the hWndInsertAfter parameter to
        /// HWND_TOPMOST and ensuring that the SWP_NOZORDER flag is not set, or by setting a window's
        /// position in the Z order so that it is above any existing topmost windows. When a
        /// non-topmost window is made topmost, its owned windows are also made topmost. Its owners,
        /// however, are not changed.
        /// </para>
        /// <para>
        /// If neither the SWP_NOACTIVATE nor SWP_NOZORDER flag is specified (that is, when the
        /// application requests that a window be simultaneously activated and its position in the Z
        /// order changed), the value specified in hWndInsertAfter is used only in the following circumstances.
        /// </para>
        /// <list type="bullet">
        /// <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag is specified in hWndInsertAfter.</item>
        /// <item>The window identified by hWnd is not the active window.</item>
        /// </list>
        /// <para>
        /// An application cannot activate an inactive window without also bringing it to the top of
        /// the Z order. Applications can change an activated window's position in the Z order
        /// without restrictions, or it can activate a window and then move it to the top of the
        /// topmost or non-topmost windows.
        /// </para>
        /// <para>
        /// If a topmost window is repositioned to the bottom (HWND_BOTTOM) of the Z order or after
        /// any non-topmost window, it is no longer topmost. When a topmost window is made
        /// non-topmost, its owners and its owned windows are also made non-topmost windows.
        /// </para>
        /// <para>
        /// A non-topmost window can own a topmost window, but the reverse cannot occur. Any window
        /// (for example, a dialog box) owned by a topmost window is itself made a topmost window, to
        /// ensure that all owned windows stay above their owner.
        /// </para>
        /// <para>
        /// If an application is not in the foreground, and should be in the foreground, it must call
        /// the SetForegroundWindow function.
        /// </para>
        /// <para>
        /// To use SetWindowPos to bring a window to the top, the process that owns the window must
        /// have SetForegroundWindow permission.
        /// </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, PInvoke.User32.SetWindowPosFlags uFlags);

        /// <summary>
        /// Shows a Window
        /// </summary>
        /// <remarks>
        /// <para>To perform certain special effects when showing or hiding a window, use AnimateWindow.</para>
        /// <para>
        /// The first time an application calls ShowWindow, it should use the WinMain function's
        /// nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one
        /// of the values in the given list, instead of the one specified by the WinMain function's
        /// nCmdShow parameter.
        /// </para>
        /// <para>
        /// As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in
        /// the first call to ShowWindow if the program that launched the application specifies
        /// startup information in the structure. In this case, ShowWindow uses the information
        /// specified in the STARTUPINFO structure to show the window. On subsequent calls, the
        /// application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup
        /// information provided by the program that launched the application. This behavior is
        /// designed for the following situations:
        /// </para>
        /// <list type="">
        /// <item>
        /// Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.
        /// </item>
        /// <item>
        /// Applications create their main window by calling CreateWindow with the WS_VISIBLE flag
        /// cleared, and later call ShowWindow with the SW_SHOW flag set to make it visible.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">
        /// Specifies how the window is to be shown. This parameter is ignored the first time an
        /// application calls ShowWindow, if the program that launched the application provides a
        /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called, the value should
        /// be the value obtained by the WinMain function in its nCmdShow parameter. In subsequent
        /// calls, this parameter can be one of the WindowShowStyle members.
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero. If the window was
        /// previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, PInvoke.User32.WindowShowStyle nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, IntPtr pvParam, SPIF fWinIni);

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPhysicalPoint(POINT pt);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT p);
    }
}