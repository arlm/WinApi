// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace WinApi.Shell
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CHANGEFILTERSTRUCT
    {
        public uint cbSize;
        public MSGFLTINFO ExtStatus;

        public static CHANGEFILTERSTRUCT Create() => new CHANGEFILTERSTRUCT
        {
            cbSize = (uint)Marshal.SizeOf(typeof(CHANGEFILTERSTRUCT)),
            ExtStatus = MSGFLTINFO.NONE
        };
    }
}

