// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.Shell
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ChangeWindowMessageFilterEx(
            IntPtr hwnd,
            User32.WindowMessage message,
            MSGFLT action,
            /*[In, Out, Optional]*/ ref CHANGEFILTERSTRUCT pChangeFilterStruct);
    }
}
