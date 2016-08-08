// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DeviceDescriptionUnion
    {
        [FieldOffset(0)]
        public PrintData printData;

        [FieldOffset(0)]
        public DisplayData displayData;
    }
}