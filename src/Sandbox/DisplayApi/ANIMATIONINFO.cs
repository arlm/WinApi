// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace Sandbox
{
    /// <summary>
    /// ANIMATIONINFO specifies animation effects associated with user actions. Used with
    /// SystemParametersInfo when SPI_GETANIMATION or SPI_SETANIMATION action is specified.
    /// </summary>
    /// <remark>
    /// The uiParam value must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)) when
    /// using this structure.
    /// </remark>
    [StructLayout(LayoutKind.Sequential)]
    public struct ANIMATIONINFO
    {
        /// <summary>
        /// Always must be set to (System.UInt32)Marshal.SizeOf(typeof(ANIMATIONINFO)).
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// If non-zero, minimize/restore animation is enabled, otherwise disabled.
        /// </summary>
        public int iMinAnimate;

        /// <summary>
        /// Creates an AMINMATIONINFO structure.
        /// </summary>
        /// <param name="iMinAnimate">
        /// If non-zero and SPI_SETANIMATION is specified, enables minimize/restore animation.
        /// </param>
        public ANIMATIONINFO(int iMinAnimate)
        {
            this.cbSize = (uint)Marshal.SizeOf(typeof(ANIMATIONINFO));
            this.iMinAnimate = iMinAnimate;
        }
    }
}