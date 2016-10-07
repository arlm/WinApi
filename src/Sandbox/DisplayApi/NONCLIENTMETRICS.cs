// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    //NONCLIENTMETRICS ncm;
    //memset(&ncm, 0, sizeof(NONCLIENTMETRICS));
    //ncm.cbSize = sizeof(NONCLIENTMETRICS);
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NONCLIENTMETRICS
    {
        public int cbSize;
        public int iBorderWidth;
        public int iCaptionHeight;
        public int iCaptionWidth;
        public int iMenuHeight;
        public int iMenuWidth;
        public int iScrollHeight;
        public int iScrollWidth;
        public int iSMCaptionHeight;

        public int iSMCaptionWidth;

        /// <summary>
        /// Since <see cref="LOGFONT"/> is a struct instead of a class, we don't have to do any
        /// special marshaling here. Much simpler this way.
        /// </summary>
        public LOGFONT lfCaptionFont;

        public LOGFONT lfMenuFont;
        public LOGFONT lfMessageFont;
        public LOGFONT lfSMCaptionFont;
        public LOGFONT lfStatusFont;
    }
}