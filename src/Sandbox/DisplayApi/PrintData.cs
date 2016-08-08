// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PrintData
    {
        // short
        public short dmOrientation;

        // short
        public short dmPaperSize;

        // short
        public short dmPaperLength;

        // short
        public short dmPaperWidth;

        // short
        public short dmScale;

        // short
        public short dmCopies;

        // short
        public short dmDefaultSource;

        // short
        public short dmPrintQuality;
    }
}