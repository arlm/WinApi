// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinApi.PeCoff
{
    public struct IMAGE_COR20_HEADER
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
    }
}