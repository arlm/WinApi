// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace WinApi.User32
{
    public static class MouseInputApi
    {
        public static void SendMouseInput(PInvoke.User32.MOUSEEVENTF flags, int data, int dx = 0, int dy = 0)
        {
            PInvoke.User32.INPUT[] input = new[] { new PInvoke.User32.INPUT() };

            input[0].type = PInvoke.User32.InputType.INPUT_MOUSE;
            var mi = new PInvoke.User32.MOUSEINPUT();
            mi.dx = dx;
            mi.dy = dy;
            mi.dwFlags = flags;
            mi.time = 0;
            mi.dwExtraInfo_IntPtr = IntPtr.Zero;
            mi.mouseData = unchecked((uint)data);
            input[0].mi = mi;
            var result = PInvoke.User32.SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }
    }
}