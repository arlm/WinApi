// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using PInvoke;

namespace WinApi
{
    /// <summary>
    /// The struct that contains the display information
    /// </summary>
    public class DisplayInfo
    {
        public string Availability { get; set; }

        public RECT MonitorArea { get; set; }

        public string ScreenHeight { get; set; }

        public string ScreenWidth { get; set; }

        public RECT WorkArea { get; set; }
    }
}