// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    // A "logical font" used by old-school windows
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct LOGFONT
    {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;

        /// <summary>
        /// <see cref="UnmanagedType.ByValTStr"/> means that the string should be marshaled as an
        /// array of TCHAR embedded in the structure. This implies that the font names can be no
        /// larger than <see cref="User32.LF_FACESIZE"/> including the terminating '\0'. That works out to
        /// 31 characters.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = User32.LF_FACESIZE)]
        public string lfFaceName;

        // to shut it up about the warnings
        public LOGFONT(string lfFaceName)
        {
            this.lfFaceName = lfFaceName;
            lfHeight = lfWidth = lfEscapement = lfOrientation = lfWeight = 0;
            lfItalic = lfUnderline = lfStrikeOut = lfCharSet = lfOutPrecision = lfClipPrecision = lfQuality = lfPitchAndFamily = 0;
        }
    }
}