// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Sandbox
{
    public enum ScaleFactors
    {
        /// <summary>
        /// Scaling is not available or supported
        /// </summary>
        NotAvailable = 0,

        /// <summary>
        /// If the display have less then 900 vertical lines this the only supported option
        /// </summary>
        NoScaling = 100,
        
        /// <summary>
        /// This is supported on displays with 900 lines or more and less than 1080 lines
        /// </summary>
        Quarter = 125,

        /// <summary>
        /// This is supported on displays with 1080 lines or more and less than 1440 lines
        /// </summary>
        Half = 150,

        /// <summary>
        /// This is supported on displays with 1440 lines
        /// </summary>
        Twice = 200,

        /// <summary>
        /// Not supported yet, but valid value
        /// </summary>
        TwoAndAHalf = 250,

        /// <summary>
        /// Not supported yet, but valid value
        /// </summary>
        Thrice = 300,

        /// <summary>
        /// Not supported yet, but valid value
        /// </summary>
        FourTimes = 400,

        /// <summary>
        /// Not supported yet, but valid value (this is the highest possible value)
        /// </summary>
        FiveTimes = 500
    }
}
