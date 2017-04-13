// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinApi.PeCoff
{
    public struct IMAGE_OPTIONAL_HEADER32
    {
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;
        public uint Checksum;
        public ushort DllCharacteristics;
        public uint FileAlignment;
        public uint ImageBase;
        public uint LoaderConfig;
        public ushort Magic;
        public ushort MajorImageVersion;
        public byte MajorLinkerVersion;
        public ushort MajorOperatingSystemVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorImageVersion;
        public byte MinorLinkerVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MinorSubsystemVersion;
        public uint NumberOfRvaAndSizes;
        public uint SectionAlignment;
        public uint SizeOfCode;
        public uint SizeOfHeaders;
        public uint SizeOfHeapCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfImage;
        public uint SizeOfInitializedData;
        public uint SizeOfStackCommit;
        public uint SizeOfStackReserve;
        public uint SizeOfUninitializedData;
        public ushort Subsystem;
        public uint Win32VersionValue;
    }

}
