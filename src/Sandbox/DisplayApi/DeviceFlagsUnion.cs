// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DeviceFlagsUnion
    {
        #region Public Fields

        // DWORD->unsigned int
        [FieldOffset(0)]
        public uint dmDisplayFlags;

        // DWORD->unsigned int
        [FieldOffset(0)]
        public uint dmNup;

        #endregion Public Fields
    }
}