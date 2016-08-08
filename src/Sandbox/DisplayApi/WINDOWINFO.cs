// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
        #region Public Fields

        public ushort atomWindowType;
        public uint cbSize;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public uint dwExStyle;
        public uint dwStyle;
        public uint dwWindowStatus;
        public RECT rcClient;
        public RECT rcWindow;
        public ushort wCreatorVersion;

        #endregion Public Fields

        #region Public Constructors

        public WINDOWINFO(bool? filler) : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            this.cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
        }

        #endregion Public Constructors
    }
}