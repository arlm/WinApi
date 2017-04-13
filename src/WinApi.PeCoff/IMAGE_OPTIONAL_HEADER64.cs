// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_OPTIONAL_HEADER64 : IEquatable<IMAGE_OPTIONAL_HEADER64>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_OPTIONAL_HEADER64?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_OPTIONAL_HEADER64 other)
        {
            if (ReferenceEquals(other, this))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
            {

                hash = (hash * mersenePrime);
            }

            return hash;
        }

        public static bool operator ==(IMAGE_OPTIONAL_HEADER64 x, IMAGE_OPTIONAL_HEADER64 y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_OPTIONAL_HEADER64 x, IMAGE_OPTIONAL_HEADER64 y)
        {
            return !x.Equals(y);
        }
    }
}
