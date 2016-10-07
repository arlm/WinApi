// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PInvoke;
using static PInvoke.User32;

namespace Sandbox
{
    //This is a sample HelloWin ported from Petzolds book (chapter 3) to illustrate the IntPtr issue for classname on CreateWindow
    public class Program
    {
        private const string APP_NAME = "HelloWin";

        private static unsafe IntPtr WndProc(IntPtr hWnd, WindowMessage msg, void* wParam, void* lParam)
        {
            PAINTSTRUCT ps;
            RECT rect;

            switch (msg)
            {
                case WindowMessage.WM_PAINT:
                    using (var hdc = BeginPaint(hWnd, out ps))
                    {
                        GetClientRect(hWnd, out rect);

                        DrawText(hdc, "Hello, Windows 98!".ToCharArray(), -1, ref rect, TextFormats.DT_SINGLELINE | TextFormats.DT_CENTER | TextFormats.DT_VCENTER);

                        EndPaint(hWnd, ps);
                    }
                    return IntPtr.Zero;

                case WindowMessage.WM_DESTROY:
                    PostQuitMessage(0);
                    return IntPtr.Zero;
            }

            return DefWindowProc(hWnd, msg, (IntPtr)wParam, (IntPtr)lParam);
        }

#pragma warning disable RECS0154 // Parameter is never used
        private static unsafe void Main(string[] args)
#pragma warning restore RECS0154 // Parameter is never used
        {
            var hInstance = Process.GetCurrentProcess().Handle;

            ushort regResult;
            fixed (char* className = APP_NAME)
            {
                WNDCLASS wndclass;
                wndclass.style = ClassStyles.CS_HREDRAW | ClassStyles.CS_VREDRAW;
                wndclass.lpfnWndProc = WndProc;
                wndclass.cbClsExtra = 0;
                wndclass.cbWndExtra = 0;
                wndclass.hInstance = hInstance;
                wndclass.hIcon = LoadIcon(IntPtr.Zero, new IntPtr((int)SystemIcons.IDI_APPLICATION));
                wndclass.hCursor = LoadCursor(IntPtr.Zero, new IntPtr((int)IDC_STANDARD_CURSORS.IDC_ARROW));
                wndclass.hbrBackground = Gdi32.GetStockObject(Gdi32.StockObject.WHITE_BRUSH);
                wndclass.lpszMenuName = null;
                wndclass.lpszClassName = className;

                regResult = RegisterClass(ref wndclass);
            }

            if (regResult == 0)
            {
                MessageBox(IntPtr.Zero, "This program requires Windows NT!", APP_NAME, MessageBoxOptions.MB_ICONERROR);
                return;
            }

            var hwnd = CreateWindowEx(
                WindowStylesEx.WS_EX_OVERLAPPEDWINDOW,
                APP_NAME, // window class name
                "The Hello Program", // window caption
                WindowStyles.WS_OVERLAPPEDWINDOW, // window style
                CW_USEDEFAULT, // initial x position
                CW_USEDEFAULT, // initial y position
                CW_USEDEFAULT, // initial x size
                CW_USEDEFAULT, // initial y size
                IntPtr.Zero, // parent window handle
                IntPtr.Zero, // window menu handle
                hInstance, // program instance handle
                IntPtr.Zero); // creation parameters

            if (hwnd == IntPtr.Zero)
            {
                var lastError = Kernel32.GetLastError();
                var errorMessage = lastError.GetMessage();
            }

            ShowWindow(hwnd, WindowShowStyle.SW_SHOWNORMAL);
            UpdateWindow(hwnd);

            
            IntPtr message = IntPtr.Zero;
            try
            {
                var size = Marshal.SizeOf(typeof(RECT));
                message = Marshal.AllocHGlobal(size);

                while (GetMessage(message, IntPtr.Zero, 0, 0) != 0)
                {
                    TranslateMessage(message);
                    DispatchMessage(message);
                }
            }
            finally
            {
                if (message == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(message);
                }
            }

            return;
        }
    }
}