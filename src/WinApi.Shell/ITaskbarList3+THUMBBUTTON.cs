// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace WinApi.Shell
{
    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Unicode)]
    internal struct THUMBBUTTON
    {
        /// <summary>
        /// WPARAM value for a THUMBBUTTON being clicked.
        /// </summary>
        public const int THBN_CLICKED = 0x1800;

        public THUMBBUTTONMASK dwMask;
        public uint iId;
        public uint iBitmap;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szTip;
        public THUMBBUTTONFLAGS dwFlags;
    }
}
