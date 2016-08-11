// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace WinApi.User32
{
    public static class KeyboardInputApi
    {
        public static void SendKeyDownInput(PInvoke.User32.VirtualKey vkKey)
        {
            PInvoke.User32.INPUT[] input = new[] { new PInvoke.User32.INPUT() };

            input[0].type = PInvoke.User32.InputType.INPUT_KEYBOARD;
            var ki = new PInvoke.User32.KEYBDINPUT();
            ki.wVk = vkKey;
            ki.wScan = PInvoke.User32.ScanCode.NONAME;
            ki.dwFlags = /* User32.KEYEVENTF.KEYEVENTF_KEY_DOWN */ (PInvoke.User32.KEYEVENTF)0;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = PInvoke.User32.SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyDownInput(PInvoke.User32.ScanCode scanCode)
        {
            PInvoke.User32.INPUT[] input = new[] { new PInvoke.User32.INPUT() };

            input[0].type = PInvoke.User32.InputType.INPUT_KEYBOARD;
            var ki = new PInvoke.User32.KEYBDINPUT();
            ki.wVk = PInvoke.User32.VirtualKey.VK_NO_KEY;
            ki.wScan = scanCode;
            ki.dwFlags = /*User32.KEYEVENTF.KEYEVENTF_KEY_DOWN |*/ PInvoke.User32.KEYEVENTF.KEYEVENTF_SCANCODE;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = PInvoke.User32.SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyUpInput(PInvoke.User32.VirtualKey vkKey)
        {
            PInvoke.User32.INPUT[] input = new[] { new PInvoke.User32.INPUT() };

            input[0].type = PInvoke.User32.InputType.INPUT_KEYBOARD;
            var ki = new PInvoke.User32.KEYBDINPUT();
            ki.wVk = vkKey;
            ki.wScan = PInvoke.User32.ScanCode.NONAME;
            ki.dwFlags = PInvoke.User32.KEYEVENTF.KEYEVENTF_KEYUP;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = PInvoke.User32.SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyUpInput(PInvoke.User32.ScanCode scanCode)
        {
            PInvoke.User32.INPUT[] input = new[] { new PInvoke.User32.INPUT() };

            input[0].type = PInvoke.User32.InputType.INPUT_KEYBOARD;
            var ki = new PInvoke.User32.KEYBDINPUT();
            ki.wVk = PInvoke.User32.VirtualKey.VK_NO_KEY;
            ki.wScan = scanCode;
            ki.dwFlags = PInvoke.User32.KEYEVENTF.KEYEVENTF_KEYUP | PInvoke.User32.KEYEVENTF.KEYEVENTF_SCANCODE;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = PInvoke.User32.SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }
    }
}