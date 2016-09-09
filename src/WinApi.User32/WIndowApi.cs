// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.User32
{
    public static class WindowApi
    {
        public static bool GetExtendedFrameBounds(IntPtr handle, out RECT rectangle)
        {
            var result = DwmApi.DwmGetWindowAttribute(handle, DwmApi.DWMWINDOWATTRIBUTE.ExtendedFrameBounds, out rectangle, Marshal.SizeOf(typeof(RECT)));

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
            else
            {
                RECT rectangle;
                return GetExtendedFrameBounds(handle, out rectangle) ? rectangle : GetWindowRect(handle);
            }
        }

        public static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate correct string length first
            var length = PInvoke.User32.SendMessage(hwnd, PInvoke.User32.WindowMessage.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();

            return WindowMessage.Send.WM_GETTEXT(hwnd, length);
        }

        // Use this code within a class to show a topmost form without giving focus to it.
        public static void ShowInactiveTopmost(IntPtr hwnd)
        {
            var rect = GetWindowRectangle(hwnd);
            PInvoke.User32.ShowWindow(hwnd, PInvoke.User32.WindowShowStyle.SW_SHOWNOACTIVATE);
            PInvoke.User32.SetWindowPos(
                hwnd,
                new IntPtr((int)PInvoke.User32.SpecialWindowHandles.HWND_TOPMOST),
                rect.left,
                rect.top,
                rect.bottom - rect.top,
                rect.right - rect.left,
                PInvoke.User32.SetWindowPosFlags.SWP_NOACTIVATE);
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
            else
            {
                return PInvoke.User32.WindowFromPoint(ps);
            }
        }
    }
}