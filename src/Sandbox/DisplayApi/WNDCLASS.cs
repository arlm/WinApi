// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WNDCLASS
    {
        #region Public Fields

        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hbrBackground;
        public IntPtr hCursor;
        public IntPtr hIcon;
        public IntPtr hInstance;

        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WndProc lpfnWndProc;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;

        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;

        public ClassStyles style;

        #endregion Public Fields

        #region Public Methods

        public void Dispose()
        {
        }

        #endregion Public Methods
    }
}