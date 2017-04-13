// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_RESOURCE_DIRECTORY : IEquatable<IMAGE_RESOURCE_DIRECTORY>
    {
        public uint Characteristics;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort NumberOfIdEntries;
        public ushort NumberOfNamedEntries;
        public uint TimeDateStamp;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_RESOURCE_DIRECTORY?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_RESOURCE_DIRECTORY other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Characteristics != other.Characteristics)
            {
                return false;
            }

            if (MajorVersion != other.MajorVersion)
            {
                return false;
            }

            if (MinorVersion != other.MinorVersion)
            {
                return false;
            }

            if (NumberOfIdEntries != other.NumberOfIdEntries)
            {
                return false;
            }

            if (NumberOfNamedEntries != other.NumberOfNamedEntries)
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
                hash = (hash * mersenePrime) + this.MajorVersion.GetHashCode();
                hash = (hash * mersenePrime) + this.MinorVersion.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfIdEntries.GetHashCode();
                hash = (hash * mersenePrime) + this.NumberOfNamedEntries.GetHashCode();
                hash = (hash * mersenePrime) + this.TimeDateStamp.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_RESOURCE_DIRECTORY x, IMAGE_RESOURCE_DIRECTORY y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_RESOURCE_DIRECTORY x, IMAGE_RESOURCE_DIRECTORY y)
        {
            return !x.Equals(y);
        }
    }
}
