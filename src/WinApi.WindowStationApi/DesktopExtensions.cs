// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WinApi.WindowStationApi
{
    public static class DesktopExtensions
    {
        public static PInvoke.User32.SafeDesktopHandle CreateDesktop(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid desktop station name", "name");
            }

            var access = (PInvoke.User32.DESKTOP_ACCESS_MASK)PInvoke.User32.DESKTOP_ACCESS_MASK.SpecificRight.DESKTOP_ALL_ACCESS;
            var handle = PInvoke.User32.CreateDesktop(name, null, IntPtr.Zero, PInvoke.User32.DesktopCreationFlags.None, access, (PInvoke.Kernel32.SECURITY_ATTRIBUTES?)null);

            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return handle;
        }

        public static PInvoke.User32.SafeWindowStationHandle CreateWindowStation(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid window station name", "name");
            }
            var access = (PInvoke.User32.WINDOW_STATION_ACCESS_MASK)PInvoke.User32.WINDOW_STATION_ACCESS_MASK.SpecificRight.WINSTA_ALL_ACCESS;
            var handle = PInvoke.User32.CreateWindowStation(name, PInvoke.User32.WindowStationCreationFlags.None, access, (PInvoke.Kernel32.SECURITY_ATTRIBUTES?)null);

            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return handle;
        }

        public static unsafe int GetDesktopHeapSize(PInvoke.User32.SafeDesktopHandle desktop)
        {
            ulong heapSize = 0;
            uint capacity = sizeof(ulong);

            if (PInvoke.User32.GetUserObjectInformation(desktop.DangerousGetHandle(), PInvoke.User32.ObjectInformationType.UOI_HEAPSIZE, &heapSize, capacity, &capacity))
            {
                return unchecked((int)heapSize);
            }
            else
            {
                return -1;
            }
        }

        public static unsafe string GetUserObjectName(PInvoke.User32.SafeDesktopHandle desktop)
        {
            return GetUserObjectName(desktop.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectName(PInvoke.User32.SafeWindowStationHandle winsta)
        {
            return GetUserObjectName(winsta.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectName(IntPtr handle)
        {
            char* text = stackalloc char[1025];
            uint capacity = 1025;

            if (PInvoke.User32.GetUserObjectInformation(handle, PInvoke.User32.ObjectInformationType.UOI_NAME, text, capacity, &capacity))
            {
                return new string(text, 0, unchecked((int)capacity)).Replace("\0", "");
            }
            else
            {
                return string.Empty;
            }
        }

        public static unsafe string GetUserObjectType(PInvoke.User32.SafeDesktopHandle desktop)
        {
            return GetUserObjectType(desktop.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectType(PInvoke.User32.SafeWindowStationHandle winsta)
        {
            return GetUserObjectType(winsta.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectType(IntPtr handle)
        {
            char* text = stackalloc char[1025];
            uint capacity = 1025;

            if (PInvoke.User32.GetUserObjectInformation(handle, PInvoke.User32.ObjectInformationType.UOI_TYPE, text, capacity, &capacity))
            {
                return new string(text, 0, unchecked((int)capacity)).Replace("\0", "");
            }
            else
            {
                return string.Empty;
            }
        }

        public static unsafe bool HasDesktopInput(PInvoke.User32.SafeDesktopHandle desktop)
        {
            bool hasInput = false;
            uint capacity = sizeof(bool);

            if (PInvoke.User32.GetUserObjectInformation(desktop.DangerousGetHandle(), PInvoke.User32.ObjectInformationType.UOI_IO, &hasInput, unchecked((uint)capacity), &capacity))
            {
                return hasInput;
            }
            else
            {
                return false;
            }
        }
    }
}