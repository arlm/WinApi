// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    //This is a sample HelloWin ported from Petzolds book (chapter 3) to illustrate the IntPtr issue for classname on CreateWindow
    public class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            IntPtr hInstance = Process.GetCurrentProcess().Handle;
            string szAppName = "HelloWin";

            WNDCLASS wndclass;

            wndclass.style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw;
            wndclass.lpfnWndProc = (hWnd, message, wParam, lParam) =>
            {
                IntPtr hdc;
                PAINTSTRUCT ps;
                RECT rect;

                switch (message)
                {
                    case PInvoke.User32.WindowMessage.WM_PAINT:
                        hdc = Sandbox.User32.BeginPaint(hWnd, out ps);
                        Sandbox.User32.GetClientRect(hWnd, out rect);

                        Sandbox.User32.DrawText(hdc, "Hello, Windows 98!", -1, ref rect, TextFormats.DT_SINGLELINE | TextFormats.DT_CENTER | TextFormats.DT_VCENTER);

                        Sandbox.User32.EndPaint(hWnd, ref ps);
                        return IntPtr.Zero;

                    case PInvoke.User32.WindowMessage.WM_DESTROY:
                        Sandbox.User32.PostQuitMessage(0);
                        return IntPtr.Zero;
                }

                return Sandbox.User32.DefWindowProc(hWnd, message, wParam, lParam);
            };

            wndclass.cbClsExtra = 0;
            wndclass.cbWndExtra = 0;
            wndclass.hInstance = hInstance;
            wndclass.hIcon = Sandbox.User32.LoadIcon(IntPtr.Zero, new IntPtr((int)SystemIcons.IDI_APPLICATION));
            wndclass.hCursor = Sandbox.User32.LoadCursor(IntPtr.Zero, new IntPtr((int)IDC_STANDARD_CURSORS.IDC_ARROW));
            wndclass.hbrBackground = Gdi32.GetStockObject(StockObjects.WHITE_BRUSH);
            wndclass.lpszMenuName = null;
            wndclass.lpszClassName = szAppName;

            ushort regResult = Sandbox.User32.RegisterClass(ref wndclass);

            if (regResult == 0)
            {
                Sandbox.User32.MessageBox(IntPtr.Zero, "This program requires Windows NT!", szAppName, MessageBoxOptions.IconError);
                return;
            }

            IntPtr hwnd = Sandbox.User32.CreateWindowEx(
                WindowStylesEx.WS_EX_OVERLAPPEDWINDOW,
                szAppName, // window class name
                "The Hello Program", // window caption
                WindowStyles.WS_OVERLAPPEDWINDOW, // window style
                Sandbox.User32.CW_USEDEFAULT, // initial x position
                Sandbox.User32.CW_USEDEFAULT, // initial y position
                Sandbox.User32.CW_USEDEFAULT, // initial x size
                Sandbox.User32.CW_USEDEFAULT, // initial y size
                IntPtr.Zero, // parent window handle
                IntPtr.Zero, // window menu handle
                hInstance, // program instance handle
                IntPtr.Zero); // creation parameters

            if (hwnd == IntPtr.Zero)
            {
                int lastError = Marshal.GetLastWin32Error();
                string errorMessage = new System.ComponentModel.Win32Exception(lastError).Message;
            }

            PInvoke.User32.ShowWindow(hwnd, PInvoke.User32.WindowShowStyle.SW_SHOWNORMAL);
            Sandbox.User32.UpdateWindow(hwnd);

            MSG msg;
            while (Sandbox.User32.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                Sandbox.User32.TranslateMessage(ref msg);
                Sandbox.User32.DispatchMessage(ref msg);
            }

            return;
        }

        #endregion Private Methods
    }
}