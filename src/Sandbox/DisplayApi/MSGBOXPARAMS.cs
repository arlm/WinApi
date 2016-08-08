// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MSGBOXPARAMS
    {
        public uint cbSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpszText;
        public string lpszCaption;
        public uint dwStyle;
        public IntPtr lpszIcon;
        public IntPtr dwContextHelpId;
        public MsgBoxCallback lpfnMsgBoxCallback;
        public uint dwLanguageId;
    }
}