// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using WinApi.Core;
using static PInvoke.User32;

namespace WinApi.User32
{
    public static class MouseInputApi
    {
        public static int GetWheelDeltaWParam(IntPtr wParam)
        {
            return unchecked((short)CoreExtensions.GetHiWord(wParam));
        }

        public static int GetWheelDeltaWParam(uint wParam)
        {
            return unchecked((short)CoreExtensions.GetHiWord(wParam));
        }

        public static void SendMouseInput(MOUSEEVENTF mouseEvent, int data, int dx = 0, int dy = 0)
        {
            INPUT[] input = { new INPUT() };

            input[0].type = InputType.INPUT_MOUSE;
            var mi = new MOUSEINPUT();
            mi.dx = dx;
            mi.dy = dy;
            mi.dwFlags = mouseEvent;
            mi.time = 0;
            mi.dwExtraInfo_IntPtr = IntPtr.Zero;
            mi.mouseData = unchecked((uint)data);
            input[0].mi = mi;
            var result = SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }
    }
}