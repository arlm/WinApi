// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.WindowStationApi
{
    public static class DesktopExtensions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The user should dispose the handle when appropriate")]
        public static unsafe User32.SafeDesktopHandle CreateDesktop(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid desktop station name", nameof(name));
            }

            var access = (uint)Kernel32.ACCESS_MASK.DesktopSpecificRight.DESKTOP_ALL_ACCESS;
            var handle = User32.CreateDesktop(name, null, IntPtr.Zero, User32.DesktopCreationFlags.None, access, (Kernel32.SECURITY_ATTRIBUTES*)null);
            var error = Marshal.GetLastWin32Error();

            if (handle.IsInvalid)
            {
                throw new Win32Exception(error);
            }

            return handle;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The user should dispose the handle when appropriate")]
        public static unsafe User32.SafeWindowStationHandle CreateWindowStation(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Invalid window station name", nameof(name));
            }

            var access = (uint)Kernel32.ACCESS_MASK.WindowStationSpecificRight.WINSTA_ALL_ACCESS;
            var handle = User32.CreateWindowStation(name, User32.WindowStationCreationFlags.None, access, (Kernel32.SECURITY_ATTRIBUTES*)null);
            var error = Marshal.GetLastWin32Error();

            if (handle.IsInvalid)
            {
                throw new Win32Exception(error);
            }

            return handle;
        }

        public static unsafe int GetDesktopHeapSize(User32.SafeDesktopHandle desktop)
        {
            if (desktop == null)
            {
                throw new ArgumentNullException(nameof(desktop));
            }

            if (desktop.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(desktop));
            }

            ulong heapSize = 0;
            uint capacity = sizeof(ulong);

            bool success = false;
            bool result = false;

            try
            {
                desktop.DangerousAddRef(ref success);

                result = User32.GetUserObjectInformation(desktop.DangerousGetHandle(), User32.ObjectInformationType.UOI_HEAPSIZE, &heapSize, capacity, &capacity);
            }
            finally
            {
                if (success)
                {
                    desktop.DangerousRelease();
                }
            }

            if (result)
            {
                return unchecked((int)heapSize);
            }

            return -1;
        }

        public static unsafe string GetUserObjectName(User32.SafeDesktopHandle desktop)
        {
            if (desktop == null)
            {
                throw new ArgumentNullException(nameof(desktop));
            }

            if (desktop.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(desktop));
            }

            if (desktop.IsClosed)
            {
                throw new ArgumentException("Handle should not be closed", nameof(desktop));
            }

            bool success = false;
            string result = null;

            try
            {
                desktop.DangerousAddRef(ref success);

                result = GetUserObjectName(desktop.DangerousGetHandle());
            }
            finally
            {
                if (success)
                {
                    desktop.DangerousRelease();
                }
            }

            return result;
        }

        public static unsafe string GetUserObjectName(User32.SafeWindowStationHandle winsta)
        {
            if (winsta == null)
            {
                throw new ArgumentNullException(nameof(winsta));
            }

            if (winsta.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(winsta));
            }

            if (winsta.IsClosed)
            {
                throw new ArgumentException("Handle should not be closed", nameof(winsta));
            }

            bool success = false;
            string result = null;

            try
            {
                winsta.DangerousAddRef(ref success);

                result = GetUserObjectName(winsta.DangerousGetHandle());
            }
            finally
            {
                if (success)
                {
                    winsta.DangerousRelease();
                }
            }

            return result;
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
            if (desktop == null)
            {
                throw new ArgumentNullException(nameof(desktop));
            }

            if (desktop.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(desktop));
            }

            if (desktop.IsClosed)
            {
                throw new ArgumentException("Handle should not be closed", nameof(desktop));
            }

            bool success = false;
            string result = null;

            try
            {
                desktop.DangerousAddRef(ref success);

                result = GetUserObjectType(desktop.DangerousGetHandle());
            }
            finally
            {
                if (success)
                {
                    desktop.DangerousRelease();
                }
            }

            return result;
        }

        public static unsafe string GetUserObjectType(User32.SafeWindowStationHandle winsta)
        {
            if (winsta == null)
            {
                throw new ArgumentNullException(nameof(winsta));
            }

            if (winsta.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(winsta));
            }

            if (winsta.IsClosed)
            {
                throw new ArgumentException("Handle should not be closed", nameof(winsta));
            }

            bool success = false;
            string result = null;

            try
            {
                winsta.DangerousAddRef(ref success);

                result = GetUserObjectType(winsta.DangerousGetHandle());
            }
            finally
            {
                if (success)
                {
                    winsta.DangerousRelease();
                }
            }

            return result;
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
            if (desktop == null)
            {
                throw new ArgumentNullException(nameof(desktop));
            }

            if (desktop.IsInvalid)
            {
                throw new ArgumentException("Handle should not be invalid", nameof(desktop));
            }

            if (desktop.IsClosed)
            {
                throw new ArgumentException("Handle should not be closed", nameof(desktop));
            }

            bool hasInput = false;
            uint capacity = sizeof(bool);
            bool success = false;

            try
            {
                desktop.DangerousAddRef(ref success);

                hasInput = User32.GetUserObjectInformation(desktop.DangerousGetHandle(), User32.ObjectInformationType.UOI_IO, &hasInput, unchecked(capacity), &capacity);
            }
            finally
            {
                if (success)
                {
                    desktop.DangerousRelease();
                }
            }

            return false;
        }
    }
}