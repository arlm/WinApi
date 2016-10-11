// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.Shell
{
    [Flags]
    public enum THUMBBUTTONFLAGS
    {
        THBF_ENABLED = 0x00,
        THBF_DISABLED = 0x01,
        THBF_DISMISSONCLICK = 0x02,
        THBF_NOBACKGROUND = 0x04,
        THBF_HIDDEN = 0x08,
        THBF_NONINTERACTIVE = 0x10
    }
}
