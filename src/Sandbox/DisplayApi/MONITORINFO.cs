// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        #region Public Fields

        public int cbSize;
        public uint dwFlags;
        public RECT rcMonitor;
        public RECT rcWork;

        #endregion Public Fields
    }
}