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

            PInvoke.User32.WNDCLASS wndclass;

            wndclass.style = PInvoke.User32.ClassStyles.HorizontalRedraw | PInvoke.User32.ClassStyles.VerticalRedraw;
            wndclass.lpfnWndProc = (hWnd, message, wParam, lParam) =>
            {
                IntPtr hdc;
                PInvoke.User32.PAINTSTRUCT ps;
                RECT rect;

                switch (message)
                {
                    case PInvoke.User32.WindowMessage.WM_PAINT:
                        hdc = PInvoke.User32.BeginPaint(hWnd, out ps);
                        PInvoke.User32.GetClientRect(hWnd, out rect);

                        PInvoke.User32.DrawText(hdc, "Hello, Windows 98!".ToCharArray(), -1, ref rect, PInvoke.User32.TextFormats.DT_SINGLELINE | PInvoke.User32.TextFormats.DT_CENTER | PInvoke.User32.TextFormats.DT_VCENTER);

                        PInvoke.User32.EndPaint(hWnd, ref ps);
                        return IntPtr.Zero;

                    case PInvoke.User32.WindowMessage.WM_DESTROY:
                        PInvoke.User32.PostQuitMessage(0);
                        return IntPtr.Zero;
                }

                return PInvoke.User32.DefWindowProc(hWnd, message, wParam, lParam);
            };

            wndclass.cbClsExtra = 0;
            wndclass.cbWndExtra = 0;
            wndclass.hInstance = hInstance;
            wndclass.hIcon = PInvoke.User32.LoadIcon(IntPtr.Zero, new IntPtr((int)SystemIcons.IDI_APPLICATION));
            wndclass.hCursor = PInvoke.User32.LoadCursor(IntPtr.Zero, new IntPtr((int)IDC_STANDARD_CURSORS.IDC_ARROW));
            wndclass.hbrBackground = Gdi32.GetStockObject(PInvoke.Gdi32.StockObjects.WHITE_BRUSH);
            wndclass.lpszMenuName = null;
            wndclass.lpszClassName = szAppName;

            ushort regResult = PInvoke.User32.RegisterClass(ref wndclass);

            if (regResult == 0)
            {
                PInvoke.User32.MessageBox(IntPtr.Zero, "This program requires Windows NT!", szAppName, PInvoke.User32.MessageBoxOptions.IconError);
                return;
            }

            IntPtr hwnd = PInvoke.User32.CreateWindowEx(
                PInvoke.User32.WindowStylesEx.WS_EX_OVERLAPPEDWINDOW,
                szAppName, // window class name
                "The Hello Program", // window caption
                PInvoke.User32.WindowStyles.WS_OVERLAPPEDWINDOW, // window style
                PInvoke.User32.CW_USEDEFAULT, // initial x position
                PInvoke.User32.CW_USEDEFAULT, // initial y position
                PInvoke.User32.CW_USEDEFAULT, // initial x size
                PInvoke.User32.CW_USEDEFAULT, // initial y size
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
            PInvoke.User32.UpdateWindow(hwnd);

            PInvoke.User32.MSG msg;
            while (PInvoke.User32.GetMessage(out msg, IntPtr.Zero, 0, 0) != 0)
            {
                PInvoke.User32.TranslateMessage(ref msg);
                PInvoke.User32.DispatchMessage(ref msg);
            }

            return;
        }

        #endregion Private Methods
    }
}