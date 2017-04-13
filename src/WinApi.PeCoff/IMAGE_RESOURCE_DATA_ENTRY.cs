// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_RESOURCE_DATA_ENTRY : IEquatable<IMAGE_RESOURCE_DATA_ENTRY>
    {
        public uint CodePage;
        public uint OffsetToData;
        public uint Reserved;
        public uint Size;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_RESOURCE_DATA_ENTRY?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_RESOURCE_DATA_ENTRY other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (CodePage != other.CodePage)
            {
                return false;
            }

            if (OffsetToData != other.OffsetToData)
            {
                return false;
            }

            if (Reserved != other.Reserved)
            {
                return false;
            }

            return Size == other.Size;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
        {
            hash = (hash * mersenePrime) + this.CodePage.GetHashCode();
            hash = (hash * mersenePrime) + this.OffsetToData.GetHashCode();
            hash = (hash * mersenePrime) + this.Reserved.GetHashCode();
            hash = (hash * mersenePrime) + this.Size.GetHashCode();
        }

            return hash;
        }

        public static bool operator ==(IMAGE_RESOURCE_DATA_ENTRY x, IMAGE_RESOURCE_DATA_ENTRY y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_RESOURCE_DATA_ENTRY x, IMAGE_RESOURCE_DATA_ENTRY y)
        {
            return !x.Equals(y);
        }
    }
}
