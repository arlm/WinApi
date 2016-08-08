// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayData
    {
        #region Public Fields

        // DWORD->unsigned int
        public uint dmDisplayFixedOutput;

        // DWORD->unsigned int
        public uint dmDisplayOrientation;

        // POINTL->_POINTL
        public POINTL dmPosition;

        #endregion Public Fields
    }
}