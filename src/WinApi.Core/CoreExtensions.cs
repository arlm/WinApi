// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.Core
{
    public static class CoreExtensions
    {
        #region Public Methods

        public static ushort GetHiWord(IntPtr dwValue)
        {
            return (ushort)((((long)dwValue) >> 0x10) & 0xffff);
        }

        public static ushort GetHiWord(uint dwValue)
        {
            return (ushort)(dwValue >> 0x10);
        }

        public static ushort GetLoWord(IntPtr dwValue)
        {
            return (ushort)(((long)dwValue) & 0xffff);
        }

        public static ushort GetLoWord(uint dwValue)
        {
            return (ushort)(dwValue & 0xffff);
        }

        public static IntPtr MakeLParam(int loWord, int hiWord)
        {
            return (IntPtr)((loWord & 0xffff) | ((hiWord & 0xffff) << 0x10));
        }

        public static IntPtr MakeWParam(int loWord, int hiWord)
        {
            return (IntPtr)((loWord & 0xffff) | ((hiWord & 0xffff) << 0x10));
        }

        #endregion Public Methods
    }
}