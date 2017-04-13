// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_FILE_HEADER : IEquatable<IMAGE_FILE_HEADER>
    {
        public static readonly IMAGE_FILE_HEADER Empty = Create();

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

        public static IMAGE_FILE_HEADER Create()
        {
            var result = default(IMAGE_FILE_HEADER);
            result.DataDictionaryRVA = new uint[16];
            result.DataDictionarySize = new uint[16];
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_FILE_HEADER?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_FILE_HEADER other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Characteristics != other.Characteristics)
            {
                return false;
            }

            if (DataDictionaryRVA != other.DataDictionaryRVA)
            {
                return false;
            }

            if (DataDictionarySize != other.DataDictionarySize)
            {
                return false;
            }

            if (Machine != other.Machine)
            {
                return false;
            }

            if (NumberOfSections != other.NumberOfSections)
            {
                return false;
            }

            if (NumberOfSymbols != other.NumberOfSymbols)
            {
                return false;
            }

            if (PEFormat != other.PEFormat)
            {
                return false;
            }

            if (PointerToSymbolTable != other.PointerToSymbolTable)
            {
                return false;
            }

            if (SizeOfOptionalHeader != other.SizeOfOptionalHeader)
            {
                return false;
            }
          
            return TimeDateStamp == other.TimeDateStamp;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
            {

                hash = (hash * mersenePrime) + this.Characteristics.GetHashCode();
                hash = (hash * mersenePrime) + this.DataDictionaryRVA.GetHashCode();
                hash = (hash * mersenePrime) + this.DataDictionarySize.GetHashCode();
                hash = (hash * mersenePrime) + this.Machine.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfSections.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfSymbols.GetHashCode();
                hash = (hash * mersenePrime) + this.PEFormat.GetHashCode();
                hash = (hash * mersenePrime) + this.PointerToSymbolTable.GetHashCode();
                hash = (hash * mersenePrime) + this.SizeOfOptionalHeader.GetHashCode();
                hash = (hash * mersenePrime) + this.TimeDateStamp.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_FILE_HEADER x, IMAGE_FILE_HEADER y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_FILE_HEADER x, IMAGE_FILE_HEADER y)
        {
            return !x.Equals(y);
        }
    }
}