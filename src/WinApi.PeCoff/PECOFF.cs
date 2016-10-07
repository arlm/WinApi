// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct PeCoffStructure
    {
        public static readonly PeCoffStructure Empty = Initialize();
        public Characteristics Characteristics;
        public uint[] DataDictionaryRVA;
        public uint[] DataDictionarySize;
        public MachineType Machine;
        public ushort NumberOfSections;
        public uint NumberOfSymbols;
        public PEType PEFormat;
        public uint PointerToSymbolTable;
        public ushort SizeOfOptionalHeader;
        public DateTime TimeDateStamp;

        public static PeCoffStructure Initialize()
        {
            var result = new PeCoffStructure();
            result.DataDictionaryRVA = new uint[16];
            result.DataDictionarySize = new uint[16];
            return result;
        }
    }
}