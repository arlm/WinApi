// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;

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

        public static bool Is64Bit(PInvoke.Kernel32.SafeObjectHandle hProcess = null)
        {
            bool isWow64 = false;

            try
            {
                if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                      Environment.OSVersion.Version.Major > 5)
                {
                    using (var moduleHandle = PInvoke.Kernel32.GetModuleHandle("Kernel32.dll"))
                    {
                        if (!moduleHandle.IsInvalid)
                        {
                            var proc = PInvoke.Kernel32.GetProcAddress(moduleHandle, "IsWow64Process");

                            if (proc != IntPtr.Zero)
                            {
                                IntPtr process = IntPtr.Zero;

                                if (hProcess == null || hProcess.IsInvalid || hProcess.IsClosed || hProcess == PInvoke.Kernel32.SafeObjectHandle.Null)
                                {
                                    process = Process.GetCurrentProcess().Handle;
                                }

                                var processHandle = process == IntPtr.Zero ? new PInvoke.Kernel32.SafeObjectHandle(process, true) : hProcess;

                                isWow64 = PInvoke.Kernel32.IsWow64Process(processHandle);

                                if (process == IntPtr.Zero)
                                {
                                    processHandle.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
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