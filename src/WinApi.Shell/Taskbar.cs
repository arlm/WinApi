// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.Shell
{
    /// <summary>
    /// Implements ITaskbarList1, ITaskbarList2, ITaskbarList3, ITaskbarList4  by exposing methods that support the 
    /// unified launching and switching taskbar button functionality added in Windows 7. 
    /// This functionality includes thumbnail representations and switch targets based on individual tabs in a tabbed application,
    /// thumbnail toolbars, notification and status overlays, and progress indicators.
    /// </summary>
    public class Taskbar : IDisposable
    {
        private const string WM_TASKBARBUTTONCREATED = "TaskbarButtonCreated";
        private ITaskbarList4 taskbarInstance;
        private static bool isCapturingMessages;
        private static int taskbarButtonCreated = -1;

        private static readonly bool taskbarSupported;

        public static int TaskbarButtonCreatedMessage => taskbarButtonCreated;

        public static bool TaskbarSupported => taskbarSupported;

        static Taskbar()
        {
            taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

            if (taskbarSupported)
            {
                try
                {
                    taskbarButtonCreated = User32.RegisterWindowMessage(WM_TASKBARBUTTONCREATED);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.Message);
                }
            }
        }

        public Taskbar(IntPtr hwnd)
        {
            if (taskbarSupported && !isCapturingMessages)
            {
                try
                {
                    var filter = CHANGEFILTERSTRUCT.Create();

                    if (!NativeMethods.ChangeWindowMessageFilterEx(hwnd, (User32.WindowMessage)taskbarButtonCreated, MSGFLT.ALLOW, ref filter))
                    {
                        Kernel32.GetLastError().ThrowOnError();
                    }

                    isCapturingMessages = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Increments the RCW reference count
        /// </summary>
        /// <typeparam name="T">The RCW type</typeparam>
        /// <param name="t">The RCW to be released</param>
        /// <returns>An instance of the RCW object, from the proper IUnknown interface</returns>
        private static T AddRcwRef<T>(T t)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.GetIUnknownForObject(t);
                return (T)Marshal.GetObjectForIUnknown(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.Release(ptr);
                }
            }
        }


        /// <summary>
        /// Releases the RCW decrementing the reference count, this method lets the finalizer do the work
        /// </summary>
        /// <typeparam name="T">The RCW type</typeparam>
        /// <param name="t">The RCW to be released</param>
        /// <remarks>If the object in question is an argument to a function the call stack will have a reference and this won’t work</remarks>
        private static void ReleaseRcw<T>(ref T t)
        {
            t = default(T);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Initialize()
        {
            if (taskbarSupported && taskbarInstance == null)
            {
                try
                {
                    taskbarInstance = (ITaskbarList4)new TaskbarInstance();
                    taskbarInstance.HrInit().ThrowOnFailure();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(ex.Message);

                    taskbarInstance = null;
                }
            }
        }

        public void WndProcCall(ref User32.MSG msg)
        {
            if (msg.message == (User32.WindowMessage)taskbarButtonCreated)
            {
                Initialize();
            }
        }

        public void SetState(IntPtr windowHandle, TaskbarState taskbarState)
        {
            if (taskbarSupported)
            {
                taskbarInstance?.SetProgressState(windowHandle, (TBPFLAG)taskbarState);
            }
        }

        public void SetValue(IntPtr windowHandle, ulong progressValue, ulong progressMax)
        {
            if (taskbarSupported)
            {
                taskbarInstance?.SetProgressValue(windowHandle, progressValue, progressMax);
            }
        }

        public void SetOverlayIcon(IntPtr hwnd, Icon icon, string description)
        {
            if (taskbarSupported)
            {
                taskbarInstance?.SetOverlayIcon(hwnd, icon.Handle, description);
            }
        }

        public void SetOverlayIcon(IntPtr hwnd, Bitmap icon, string description)
        {
            if (taskbarSupported)
            {
                taskbarInstance?.SetOverlayIcon(hwnd, icon.GetHicon(), description);
            }
        }

        public void SetThumbnailClip(IntPtr hwnd, int x, int y, int width, int height)
        {
            if (taskbarSupported)
            {
                var rect = new RECT { left = x, top = y, right = x + width, bottom = y + height };
                taskbarInstance?.SetThumbnailClip(hwnd, ref rect);
            }
        }

        public void SetThumbnailToolTip(IntPtr hwnd, string tip)
        {
            if (taskbarSupported)
            {
                taskbarInstance?.SetThumbnailTooltip(hwnd, tip);
            }
        }

        public void UpdateThumbnailButtons(IntPtr hwnd, params ThumbnailButton[] buttons)
        {
            if (taskbarSupported)
            {
                var comButtons = new List<THUMBBUTTON>();
                foreach (ThumbnailButton button in buttons)
                {
                    comButtons.Add(button.ToTHUMBBUTTON());
                }

                var result = taskbarInstance?.ThumbBarUpdateButtons(hwnd, (uint)buttons.Length, comButtons.ToArray()) ?? HResult.Code.E_FAIL;

                if (result.Failed)
                {
                    throw new COMException("Cannot update thumb button: " + result.ToString());
                }
            }
        }

        public void AddThumbnailButtons(IntPtr hwnd, params ThumbnailButton[] buttons)
        {
            if (taskbarSupported)
            {
                var comButtons = new List<THUMBBUTTON>();
                foreach (ThumbnailButton button in buttons)
                {
                    comButtons.Add(button.ToTHUMBBUTTON());
                }

                var result = taskbarInstance?.ThumbBarAddButtons(hwnd, (uint)buttons.Length, comButtons.ToArray()) ?? HResult.Code.E_FAIL;

                if (result.Failed)
                {
                    throw new COMException("Cannot add thumb button: " + result.ToString());
                }
            }
        }

        public void Dispose()
        {
            if (taskbarInstance != null)
            {
                Marshal.ReleaseComObject(taskbarInstance);
                taskbarInstance = null;
            }
        }

        [ComImport]
        [Guid("56FDF344-FD6D-11D0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class TaskbarInstance
        {
        }
    }
}