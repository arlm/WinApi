// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;
using PInvoke;

namespace Sandbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        #region Public Fields

        public POINT ptMaxPosition;
        public POINT ptMaxSize;
        public POINT ptMaxTrackSize;
        public POINT ptMinTrackSize;
        public POINT ptReserved;

        #endregion Public Fields
    }
}