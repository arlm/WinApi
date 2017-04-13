// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    [Flags]
    public enum Characteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Microsoft Windows NT and later. This indicates that the
        /// file does not contain base relocations and must therefore be loaded at its preferred
        /// base address. If the base address is not available, the loader reports an error.
        /// </summary>
        /// <remarks>
        /// The default behavior of the linker is to strip base relocations from executable (EXE) files.
        /// </remarks>
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001,

        /// <summary>
        /// Image only. This indicates that the image file is valid and can be run.
        /// </summary>
        /// <remarks>If this flag is not set, it indicates a linker error.</remarks>
        IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002,

        /// <summary>
        /// COFF line numbers have been removed.
        /// </summary>
        /// <remarks>This flag is deprecated and should be zero.</remarks>
        [Obsolete("This flag is deprecated and should be zero.")]
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,

        /// <summary>
        /// COFF symbol table entries for local symbols have been removed.
        /// </summary>
        /// <remarks>This flag is deprecated and should be zero.</remarks>
        [Obsolete("This flag is deprecated and should be zero.")]
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,

        /// <summary>
        /// Aggressively trim working set.
        /// </summary>
        /// <remarks>This flag is deprecated for Windows 2000 and later and must be zero.</remarks>
        [Obsolete("This flag is deprecated for Windows 2000 and later and must be zero.")]
        IMAGE_FILE_AGGRESSIVE_WS_TRIM = 0x0010,

        /// <summary>
        /// Application can handle &gt; 2 GB addresses
        /// </summary>
        IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020,

        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1700:DoNotNameEnumValuesReserved")]
        Reserved = 0x0040,

        /// <summary>
        /// Little endian: the least significant bit (LSB) precedes the most significant bit
        /// (MSB) in memory.
        /// </summary>
        /// <remarks>This flag is deprecated and should be zero.</remarks>
        [Obsolete("This flag is deprecated and should be zero.")]
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,

        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        IMAGE_FILE_32BIT_MACHINE = 0x0100,

        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,

        /// <summary>
        /// If the image is on removable media, fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400,

        /// <summary>
        /// If the image is on network media, fully load it and copy it to the swap file.
        /// </summary>
        IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800,

        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        IMAGE_FILE_SYSTEM = 0x1000,

        /// <summary>
        /// The image file is a dynamic-link library (DLL). Such files are considered executable
        /// files for almost all purposes, although they cannot be directly run.
        /// </summary>
        IMAGE_FILE_DLL = 0x2000,

        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000,

        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory.
        /// </summary>
        /// <remarks>This flag is deprecated and should be zero.</remarks>
        [Obsolete("This flag is deprecated and should be zero.")]
        IMAGE_FILE_BYTES_REVERSED_HI = 0x8000
    }
}