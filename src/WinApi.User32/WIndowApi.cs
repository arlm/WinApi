// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;
using static PInvoke.User32;

namespace WinApi.User32
{
    public static class WindowApi
    {
        public static bool GetExtendedFrameBounds(IntPtr handle, out RECT rectangle)
        {
            HResult result;
            IntPtr rect = IntPtr.Zero;

            try
            {
                var size = Marshal.SizeOf(typeof(RECT));
                rect = Marshal.AllocHGlobal(size);
                result = DwmApi.DwmGetWindowAttribute(handle, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_EXTENDED_FRAME_BOUNDS, out rect, size);
                rectangle = Marshal.PtrToStructure<RECT>(rect);
            }
            finally
            {
                if (rect == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(rect);
                }
            }

            return result >= 0;
        }

        public static RECT GetWindowRect(IntPtr handle)
        {
            RECT rect;
            PInvoke.User32.GetWindowRect(handle, out rect);
            return rect;
        }

        //          if (!for_metro)
        //          {
        //              // On restore, resize to the previous saved rect size.
        //              gfx::Rect new_rect(saved_window_info_.window_rect);
        //              SetWindowPos(hwnd_, NULL, new_rect.x(), new_rect.y(),
        //                           new_rect.width(), new_rect.height(),
        //                           SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
        //          }
        //          if (saved_window_info_.maximized)
        //::SendMessage(hwnd_, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
        //      }
        //  }
        public static RECT GetWindowRectangle(IntPtr handle)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return GetWindowRect(handle);
            }

            RECT rectangle;
            return GetExtendedFrameBounds(handle, out rectangle) ? rectangle : GetWindowRect(handle);
        }

        public static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate correct string length first
            var length = SendMessage(hwnd, PInvoke.User32.WindowMessage.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();

            return WindowMessage.Send.WM_GETTEXT(hwnd, length);
        }

        // Use this code within a class to show a topmost form without giving focus to it.
        public static void ShowInactiveTopmost(IntPtr hwnd)
        {
            var rect = GetWindowRectangle(hwnd);
            ShowWindow(hwnd, WindowShowStyle.SW_SHOWNOACTIVATE);
            SetWindowPos(
                hwnd,
                new IntPtr((int)SpecialWindowHandles.HWND_TOPMOST),
                rect.left,
                rect.top,
                rect.bottom - rect.top,
                rect.right - rect.left,
                SetWindowPosFlags.SWP_NOACTIVATE);
        }

        public static IntPtr WindowFromPhysicalPoint(int physicalX, int physicalY)
        {
            var ps = new POINT
            {
                x = physicalX,
                y = physicalY
            };

            if (Environment.OSVersion.Version.Major >= 6)
            {
                return PInvoke.User32.WindowFromPhysicalPoint(ps);
            }

            return WindowFromPoint(ps);
        }
    }
}