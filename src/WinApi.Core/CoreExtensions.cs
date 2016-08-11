// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.Core
{
    public static class CoreExtensions
    {
        public static ushort GetHiWord(IntPtr dwValue)
        {
            return unchecked((ushort)((((long)dwValue) >> 0x10) & 0xffff));
        }

        public static ushort GetHiWord(uint dwValue)
        {
            return unchecked((ushort)(dwValue >> 0x10));
        }

        public static ushort GetLoWord(IntPtr dwValue)
        {
            return unchecked((ushort)(((long)dwValue) & 0xffff));
        }

        public static ushort GetLoWord(uint dwValue)
        {
            return unchecked((ushort)(dwValue & 0xffff));
        }

        public static IntPtr MakeLParam(int loWord, int hiWord)
        {
            return new IntPtr(unchecked((loWord & 0xffff) | ((hiWord & 0xffff) << 0x10)));
        }

        public static IntPtr MakeWParam(int loWord, int hiWord)
        {
            return new IntPtr(unchecked((loWord & 0xffff) | ((hiWord & 0xffff) << 0x10)));
        }
    }
}