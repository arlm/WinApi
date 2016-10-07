// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.WindowStationApi
{
    public static class DesktopExtensions
    {
        public static User32.SafeDesktopHandle CreateDesktop(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid desktop station name", nameof(name));
            }

            var access = (uint) Kernel32.ACCESS_MASK.DesktopSpecificRight.DESKTOP_ALL_ACCESS;
            var handle = User32.CreateDesktop(name, null, IntPtr.Zero, User32.DesktopCreationFlags.None, access, (Kernel32.SECURITY_ATTRIBUTES?)null);

            if (handle.IsInvalid)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return handle;
        }

        public static User32.SafeWindowStationHandle CreateWindowStation(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid window station name", nameof(name));
            }
            var access =(uint) Kernel32.ACCESS_MASK.WindowStationSpecificRight.WINSTA_ALL_ACCESS;
            var handle = User32.CreateWindowStation(name, User32.WindowStationCreationFlags.None, access, (Kernel32.SECURITY_ATTRIBUTES?)null);

            if (handle.IsInvalid)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }

            return handle;
        }

        public static unsafe int GetDesktopHeapSize(User32.SafeDesktopHandle desktop)
        {
            ulong heapSize = 0;
            uint capacity = sizeof(ulong);

            if (User32.GetUserObjectInformation(desktop.DangerousGetHandle(), User32.ObjectInformationType.UOI_HEAPSIZE, &heapSize, capacity, &capacity))
            {
                return unchecked((int)heapSize);
            }

            return -1;
        }

        public static unsafe string GetUserObjectName(User32.SafeDesktopHandle desktop)
        {
            return GetUserObjectName(desktop.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectName(User32.SafeWindowStationHandle winsta)
        {
            return GetUserObjectName(winsta.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectName(IntPtr handle)
        {
            char* text = stackalloc char[1025];
            uint capacity = 1025;

            if (User32.GetUserObjectInformation(handle, User32.ObjectInformationType.UOI_NAME, text, capacity, &capacity))
            {
                return new string(text, 0, unchecked((int)capacity)).Replace("\0", "");
            }

            return string.Empty;
        }

        public static unsafe string GetUserObjectType(User32.SafeDesktopHandle desktop)
        {
            return GetUserObjectType(desktop.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectType(User32.SafeWindowStationHandle winsta)
        {
            return GetUserObjectType(winsta.DangerousGetHandle());
        }

        public static unsafe string GetUserObjectType(IntPtr handle)
        {
            char* text = stackalloc char[1025];
            uint capacity = 1025;

            if (User32.GetUserObjectInformation(handle, User32.ObjectInformationType.UOI_TYPE, text, capacity, &capacity))
            {
                return new string(text, 0, unchecked((int)capacity)).Replace("\0", "");
            }

            return string.Empty;
        }

        public static unsafe bool HasDesktopInput(User32.SafeDesktopHandle desktop)
        {
            bool hasInput = false;
            uint capacity = sizeof(bool);

            if (User32.GetUserObjectInformation(desktop.DangerousGetHandle(), User32.ObjectInformationType.UOI_IO, &hasInput, unchecked(capacity), &capacity))
            {
                return hasInput;
            }

            return false;
        }
    }
}