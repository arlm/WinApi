// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

using static PInvoke.User32;

namespace WinApi.User32
{
    public static class KeyboardInputApi
    {
        public static void SendKeyDownInput(VirtualKey vkKey)
        {
            INPUT[] input = { new INPUT() };

            input[0].type = InputType.INPUT_KEYBOARD;
            var ki = new KEYBDINPUT();
            ki.wVk = vkKey;
            ki.wScan = ScanCode.NONAME;
            ki.dwFlags = /* User32.KEYEVENTF.KEYEVENTF_KEY_DOWN */ 0;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyDownInput(ScanCode scanCode)
        {
            INPUT[] input = { new INPUT() };

            input[0].type = InputType.INPUT_KEYBOARD;
            var ki = new KEYBDINPUT();
            ki.wVk = VirtualKey.VK_NO_KEY;
            ki.wScan = scanCode;
            ki.dwFlags = /*User32.KEYEVENTF.KEYEVENTF_KEY_DOWN |*/ KEYEVENTF.KEYEVENTF_SCANCODE;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyUpInput(VirtualKey vkKey)
        {
            INPUT[] input = { new INPUT() };

            input[0].type = InputType.INPUT_KEYBOARD;
            var ki = new KEYBDINPUT();
            ki.wVk = vkKey;
            ki.wScan = ScanCode.NONAME;
            ki.dwFlags = KEYEVENTF.KEYEVENTF_KEYUP;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }

        public static void SendKeyUpInput(ScanCode scanCode)
        {
            INPUT[] input = { new INPUT() };

            input[0].type = InputType.INPUT_KEYBOARD;
            var ki = new KEYBDINPUT();
            ki.wVk = VirtualKey.VK_NO_KEY;
            ki.wScan = scanCode;
            ki.dwFlags = KEYEVENTF.KEYEVENTF_KEYUP | KEYEVENTF.KEYEVENTF_SCANCODE;
            ki.time = 0;
            ki.dwExtraInfo_IntPtr = IntPtr.Zero;
            input[0].ki = ki;
            var result = SendInput(input.Length, input, Marshal.SizeOf(input[0].GetType()));
        }
    }
}