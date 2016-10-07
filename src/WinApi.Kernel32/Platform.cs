// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;

using static PInvoke.Kernel32;

namespace WinApi.Kernel32
{
    public static class Platform
    {
        public static bool IsOS64Bit
        {
            get
            {
                return Is64Bit();
            }
        }

        public static bool IsRunningAs64Bit
        {
            get
            {
                if (IntPtr.Size == 8)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method should not throw exceptions")]
        public static bool Is64Bit(SafeObjectHandle hProcess = null)
        {
            bool isWow64 = false;

            try
            {
                if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                      Environment.OSVersion.Version.Major > 5)
                {
                    using (var moduleHandle = GetModuleHandle("Kernel32.dll"))
                    {
                        if (!moduleHandle.IsInvalid)
                        {
                            var proc = GetProcAddress(moduleHandle, "IsWow64Process");

                            if (proc != IntPtr.Zero)
                            {
                                if (hProcess == null || hProcess.IsInvalid || hProcess.IsClosed || hProcess == SafeObjectHandle.Null)
                                {
                                    var process = Process.GetCurrentProcess().Handle;

                                    using (var processHandle = new SafeObjectHandle(process, true))
                                    {
                                        isWow64 = IsWow64Process(processHandle);
                                    }
                                }
                                else
                                {
                                    isWow64 = IsWow64Process(hProcess);
                                }
                            }
                        }
                    }
                }
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
            }

            if (IntPtr.Size == 8 || (IntPtr.Size == 4 && isWow64))
            {
                return true;
            }

            return false;
        }
    }
}