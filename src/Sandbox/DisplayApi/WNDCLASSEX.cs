// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        #region Public Fields

        public int cbClsExtra;

        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;

        public int cbWndExtra;

        public IntPtr hbrBackground;

        public IntPtr hCursor;

        public IntPtr hIcon;

        public IntPtr hIconSm;

        public IntPtr hInstance;

        public IntPtr lpfnWndProc;

        public string lpszClassName;

        // not WndProc
        public string lpszMenuName;

        [MarshalAs(UnmanagedType.U4)]
        public int style;

        #endregion Public Fields

        #region Public Methods

        //Use this function to make a new one with cbSize already filled in.
        //For example:
        //var WndClss = WNDCLASSEX.Build()
        public static WNDCLASSEX Build()
        {
            var nw = new WNDCLASSEX();
            nw.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            return nw;
        }

        #endregion Public Methods
    }
}