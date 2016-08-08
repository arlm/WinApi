// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        #region Public Fields

        public bool fErase;
        public bool fIncUpdate;
        public bool fRestore;
        public IntPtr hdc;
        public RECT rcPaint;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;

        #endregion Public Fields
    }
}