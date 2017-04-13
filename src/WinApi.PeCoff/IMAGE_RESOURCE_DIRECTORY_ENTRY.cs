// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_RESOURCE_DIRECTORY_ENTRY : IEquatable<IMAGE_RESOURCE_DIRECTORY_ENTRY>
    {
        public uint Name;
        public uint OffsetToData;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_RESOURCE_DIRECTORY_ENTRY?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_RESOURCE_DIRECTORY_ENTRY other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Name != other.Name)
            {
                return false;
            }

            return OffsetToData == other.OffsetToData;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
            {
                hash = (hash * mersenePrime) + this.Name.GetHashCode();
                hash = (hash * mersenePrime) + this.OffsetToData.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_RESOURCE_DIRECTORY_ENTRY x, IMAGE_RESOURCE_DIRECTORY_ENTRY y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_RESOURCE_DIRECTORY_ENTRY x, IMAGE_RESOURCE_DIRECTORY_ENTRY y)
        {
            return !x.Equals(y);
        }
    }
}
