// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.PeCoff
{
    public struct IMAGE_COR20_HEADER : IEquatable<IMAGE_COR20_HEADER>
    {
        public uint Cb;
        public IMAGE_DATA_DIRECTORY CodeManagerTable;
        public uint EntryPointToken;
        public IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
        public uint ImageSettings;
        public ushort MajorRuntimeVersion;
        public IMAGE_DATA_DIRECTORY ManagedNativeHeader;
        public IMAGE_DATA_DIRECTORY Metadata;
        public ushort MinorRuntimeVersion;
        public IMAGE_DATA_DIRECTORY Resources;
        public IMAGE_DATA_DIRECTORY StrongNameSignature;
        public IMAGE_DATA_DIRECTORY VTableFixups;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var x = obj as IMAGE_COR20_HEADER?;

            if (!x.HasValue)
            {
                return false;
            }

            return Equals(x);
        }

        public bool Equals(IMAGE_COR20_HEADER other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (Cb != other.Cb)
            {
                return false;
            }

            if (CodeManagerTable != other.CodeManagerTable)
            {
                return false;
            }

            if (EntryPointToken != other.EntryPointToken)
            {
                return false;
            }

            if (ExportAddressTableJumps != other.ExportAddressTableJumps)
            {
                return false;
            }

            if (ImageSettings != other.ImageSettings)
            {
                return false;
            }

            if (MajorRuntimeVersion != other.MajorRuntimeVersion)
            {
                return false;
            }

            if (ManagedNativeHeader != other.ManagedNativeHeader)
            {
                return false;
            }

            if (Metadata != other.Metadata)
            {
                return false;
            }

            if (MinorRuntimeVersion != other.MinorRuntimeVersion)
            {
                return false;
            }

            if (Resources != other.Resources)
            {
                return false;
            }

            if (StrongNameSignature != other.StrongNameSignature)
            {
                return false;
            }

            return VTableFixups == other.VTableFixups;
        }

        public override int GetHashCode()
        {
            // From The Online Encyclopedia of Integer Sequences: https://oeis.org/A000668
            // Mersenne primes (of form 2^p - 1 where p is a prime).
            const int mersenePrime = 131071;
            int hash = 8191;

            unchecked
            {

                hash = (hash * mersenePrime) + this.Cb.GetHashCode();
                hash = (hash * mersenePrime) + this.CodeManagerTable.GetHashCode();
                hash = (hash * mersenePrime) + this.EntryPointToken.GetHashCode();
                hash = (hash * mersenePrime) + this.ExportAddressTableJumps.GetHashCode();
                hash = (hash * mersenePrime) + this.ImageSettings.GetHashCode();
                hash = (hash * mersenePrime) + this.MajorRuntimeVersion.GetHashCode();
                hash = (hash * mersenePrime) + this.ManagedNativeHeader.GetHashCode();
                hash = (hash * mersenePrime) + this.Metadata.GetHashCode();
                hash = (hash * mersenePrime) + this.MinorRuntimeVersion.GetHashCode();
                hash = (hash * mersenePrime) + this.Resources.GetHashCode();
                hash = (hash * mersenePrime) + this.StrongNameSignature.GetHashCode();
                hash = (hash * mersenePrime) + this.VTableFixups.GetHashCode();
            }

            return hash;
        }

        public static bool operator ==(IMAGE_COR20_HEADER x, IMAGE_COR20_HEADER y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(IMAGE_COR20_HEADER x, IMAGE_COR20_HEADER y)
        {
            return !x.Equals(y);
        }
    }
}