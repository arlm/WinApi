// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace Sandbox
{
    [Flags]
    public enum MinimizedMetricsArrangement
    {
        BottomLeft = 0,
        BottomRight = 1,
        TopLeft = 2,
        TopRight = 3,
        Left = 0,
        Right = 0,
        Up = 4,
        Down = 4,
        Hide = 8
    }
}