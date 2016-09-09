// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEVMODE
    {
        public DeviceDescriptionUnion description;

        // DWORD->unsigned int
        public uint dmBitsPerPel;

        // short
        public short dmCollate;

        // short
        public short dmColor;

        // WCHAR[32]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;

        // DWORD->unsigned int
        public uint dmDisplayFrequency;

        // DWORD->unsigned int
        public uint dmDitherType;

        // WORD->unsigned short
        public ushort dmDriverExtra;

        // WORD->unsigned short
        public ushort dmDriverVersion;

        // short
        public short dmDuplex;

        // DWORD->unsigned int
        public uint dmFields;

        // WCHAR[32]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;

        // DWORD->unsigned int
        public uint dmICMIntent;

        // DWORD->unsigned int
        public uint dmICMMethod;

        // WORD->unsigned short
        public ushort dmLogPixels;

        // DWORD->unsigned int
        public uint dmMediaType;

        // DWORD->unsigned int
        public uint dmPanningHeight;

        // DWORD->unsigned int
        public uint dmPanningWidth;

        // DWORD->unsigned int
        public uint dmPelsHeight;

        // DWORD->unsigned int
        public uint dmPelsWidth;

        // DWORD->unsigned int
        public uint dmReserved1;

        // DWORD->unsigned int
        public uint dmReserved2;

        // WORD->unsigned short
        public ushort dmSize;

        // WORD->unsigned short
        public ushort dmSpecVersion;

        // short
        public short dmTTOption;

        // short
        public short dmYResolution;

        public DeviceFlagsUnion flags;
    }
}