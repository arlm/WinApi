// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HELPINFO
    {
        public uint cbSize;
        public int iContextType;
        public int iCtrlId;
        public IntPtr hItemHandle;
        public IntPtr dwContextId;
        public POINT MousePos;
    }
}