// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_DOS_HEADER : IEquatable<IMAGE_DOS_HEADER>
    {
        public ushort e_cblp;
        public ushort e_cp;
        public ushort e_cparhdr;
        public ushort e_crlc;
        public ushort e_cs;
        public ushort e_csum;
        public ushort e_ip;
        public uint e_lfanew;
        public ushort e_lfarlc;
        public ushort e_magic;
        public ushort e_maxalloc;
        public ushort e_minalloc;
        public ushort e_oemid;
        public ushort e_oeminfo;
        public ushort e_ovno;
        public ushort[] e_res;
        public ushort[] e_res2;
        public ushort e_sp;
        public ushort e_ss;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_DOS_HEADER?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_DOS_HEADER other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (e_cblp != other.e_cblp)
            {
                return false;
            }

            if (e_cp != other.e_cp)
            {
                return false;
            }

            if (e_cparhdr != other.e_cparhdr)
            {
                return false;
            }

            if (e_crlc != other.e_crlc)
            {
                return false;
            }

            if (e_cs != other.e_cs)
            {
                return false;
            }

            if (e_csum != other.e_csum)
            {
                return false;
            }

            if (e_ip != other.e_ip)
            {
                return false;
            }

            if (e_lfanew != other.e_lfanew)
            {
                return false;
            }

            if (e_lfarlc != other.e_lfarlc)
            {
                return false;
            }

            if (e_magic != other.e_magic)
            {
                return false;
            }

            if (e_maxalloc != other.e_maxalloc)
            {
                return false;
            }

            if (e_minalloc != other.e_minalloc)
            {
                return false;
            }

            if (e_oemid != other.e_oemid)
            {
                return false;
            }

            if (e_oeminfo != other.e_oeminfo)
            {
                return false;
            }

            if (e_ovno != other.e_ovno)
            {
                return false;
            }

            if (e_res != other.e_res)
            {
                return false;
            }

            if (e_res2 != other.e_res2)
            {
                return false;
            }

            if (e_sp != other.e_sp)
            {
                return false;
            }

            return e_ss == other.e_ss;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
            {

                hash = (hash * mersenePrime) + this.e_cblp.GetHashCode();
                hash = (hash * mersenePrime) + this.e_cp.GetHashCode();
                hash = (hash * mersenePrime) + this.e_cparhdr.GetHashCode();
                hash = (hash * mersenePrime) + this.e_crlc.GetHashCode();
                hash = (hash * mersenePrime) + this.e_cs.GetHashCode();
                hash = (hash * mersenePrime) + this.e_csum.GetHashCode();
                hash = (hash * mersenePrime) + this.e_ip.GetHashCode();
                hash = (hash * mersenePrime) + this.e_lfanew.GetHashCode();
                hash = (hash * mersenePrime) + this.e_lfarlc.GetHashCode();
                hash = (hash * mersenePrime) + this.e_magic.GetHashCode();
                hash = (hash * mersenePrime) + this.e_maxalloc.GetHashCode();
                hash = (hash * mersenePrime) + this.e_minalloc.GetHashCode();
                hash = (hash * mersenePrime) + this.e_oemid.GetHashCode();
                hash = (hash * mersenePrime) + this.e_oeminfo.GetHashCode();
                hash = (hash * mersenePrime) + this.e_ovno.GetHashCode();
                hash = (hash * mersenePrime) + this.e_res.GetHashCode();
                hash = (hash * mersenePrime) + this.e_res2.GetHashCode();
                hash = (hash * mersenePrime) + this.e_sp.GetHashCode();
                hash = (hash * mersenePrime) + this.e_ss.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_DOS_HEADER x, IMAGE_DOS_HEADER y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_DOS_HEADER x, IMAGE_DOS_HEADER y)
        {
            return !x.Equals(y);
        }
    }
}
