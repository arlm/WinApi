// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinApi.Console
{
    using System;
    using System.Runtime.InteropServices;

    public static class Advapi32
    {
        public delegate void LPHANDLER_FUNCTION(uint dwControl);

        // Return Type: SERVICE_STATUS_HANDLE->SERVICE_STATUS_HANDLE__*
        //lpServiceName: LPCTSTR->LPCWSTR->WCHAR*
        //lpHandlerProc: LPHANDLER_FUNCTION
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterServiceCtrlHandler(string lpServiceName, LPHANDLER_FUNCTION lpHandlerProc);
    }
}