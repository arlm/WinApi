// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinApi.PeCoff
{
    public enum PEType : ushort
    {
        None = 0,

        /// <summary>
        /// ROM Image
        /// </summary>
        ROMImage = 0x107,

        /// <summary>
        /// Normal executable
        /// </summary>
        PE32 = 0x10B,

        /// <summary>
        /// PE32+ executable
        /// </summary>
        PE32Plus = 0x20B
    }
}