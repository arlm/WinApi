// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_SECTION_HEADER : IEquatable<IMAGE_SECTION_HEADER>
    {
        public uint Characteristics;
        public byte[] Name;
        public ushort NumberOfLinenumbers;
        public ushort NumberOfRelocations;
        public uint PointerToLinenumbers;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint SizeOfRawData;
        public uint VirtualAddress;
        public uint VirtualSize;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_SECTION_HEADER?;

            if (!x.HasValue)
            {
                return false;
            }
            
            return Equals(x);
        }

        public bool Equals(IMAGE_SECTION_HEADER other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Characteristics != other.Characteristics)
            {
                return false;
            }

            if (Name != other.Name)
            {
                return false;
            }

            if (NumberOfLinenumbers != other.NumberOfLinenumbers)
            {
                return false;
            }

            if (NumberOfRelocations != other.NumberOfRelocations)
            {
                return false;
            }

            if (PointerToLinenumbers != other.PointerToLinenumbers)
            {
                return false;
            }
            if (PointerToRawData != other.PointerToRawData)
            {
                return false;
            }
            if (PointerToRelocations != other.PointerToRelocations)
            {
                return false;
            }
            if (SizeOfRawData != other.SizeOfRawData)
            {
                return false;
            }
            if (VirtualAddress != other.VirtualAddress)
            {
                return false;
            }

            return VirtualSize == other.VirtualSize;
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
                hash = (hash * mersenePrime) + this.Name.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfLinenumbers.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfRelocations.GetHashCode();
                hash = (hash * mersenePrime) + this.PointerToLinenumbers.GetHashCode();
                hash = (hash * mersenePrime) + this.PointerToRawData.GetHashCode();
                hash = (hash * mersenePrime) + this.PointerToRelocations.GetHashCode();
                hash = (hash * mersenePrime) + this.SizeOfRawData.GetHashCode();
                hash = (hash * mersenePrime) + this.VirtualAddress.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_SECTION_HEADER x, IMAGE_SECTION_HEADER y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_SECTION_HEADER x, IMAGE_SECTION_HEADER y)
        {
            return !x.Equals(y);
        }
    }
}
