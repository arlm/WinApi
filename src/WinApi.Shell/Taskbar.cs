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
        private static ITaskbarList4 taskbarInstance;
        private static bool hasTried;

        private static readonly bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

        static Taskbar()
        {
            try
            {
                taskbarInstance = (ITaskbarList4)new TaskbarInstance();
                taskbarInstance.HrInit();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);

                if (taskbarInstance != null)
                {
                    Marshal.ReleaseComObject(taskbarInstance);
                    taskbarInstance = null;
                }
            }
            finally
            {
                hasTried = true;
            }
        }

        public static void SetState(IntPtr windowHandle, TaskbarState taskbarState)
        {
            if (taskbarSupported && hasTried)
            {
                taskbarInstance?.SetProgressState(windowHandle, (TBPFLAG)taskbarState);
            }
        }

        public static void SetValue(IntPtr windowHandle, ulong progressValue, ulong progressMax)
        {
            if (taskbarSupported && hasTried)
            {
                taskbarInstance?.SetProgressValue(windowHandle, progressValue, progressMax);
            }
        }

        public static void SetOverlayIcon(IntPtr hwnd, Icon icon, string description)
        {
            if (taskbarSupported && hasTried)
            {
                taskbarInstance?.SetOverlayIcon(hwnd, icon.Handle, description);
            }
        }

        public static void SetOverlayIcon(IntPtr hwnd, Bitmap icon, string description)
        {
            if (taskbarSupported && hasTried)
            {
                taskbarInstance?.SetOverlayIcon(hwnd, icon.GetHicon(), description);
            }
        }

        public static void SetThumbnailClip(IntPtr hwnd, int x, int y, int width, int height)
        {
            if (taskbarSupported && hasTried)
            {
                var rect = new RECT { left = x, top = y, right = x + width, bottom = y + height };
                taskbarInstance?.SetThumbnailClip(hwnd, ref rect);
            }
        }

        public static void SetThumbnailToolTip(IntPtr hwnd, String tip)
        {
            if (taskbarSupported && hasTried)
            {
                taskbarInstance?.SetThumbnailTooltip(hwnd, tip);
            }
        }

        public static void UpdateThumbnailButtons(IntPtr hwnd, params ThumbnailButton[] buttons)
        {
            if (taskbarSupported && hasTried)
            {
                var comButtons = new List<THUMBBUTTON>();
                foreach (ThumbnailButton button in buttons)
                {
                    comButtons.Add(button);
                }

                var result = taskbarInstance?.ThumbBarUpdateButtons(hwnd, (uint)buttons.Length, comButtons.ToArray()) ?? HResult.Code.E_FAIL;

                if (result.Failed)
                {
                    throw new COMException("Cannot update thumb button: " + result.ToString());
                }
            }
        }

        public static void AddThumbnailButtons(IntPtr hwnd, params ThumbnailButton[] buttons)
        {
            if (taskbarSupported && hasTried)
            {
                var comButtons = new List<THUMBBUTTON>();
                foreach (ThumbnailButton button in buttons)
                {
                    comButtons.Add(button);
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
        [Guid("56FDF344-FD6D-11d0-958A-006097C9A090")]
        [ClassInterface(ClassInterfaceType.None)]
        private class TaskbarInstance
        {
            // When an application displays a window, its taskbar button is created by the system.
            // When the button is in place, the taskbar sends a TaskbarButtonCreated message to the window.Your application should call RegisterWindowMessage(L"TaskbarButtonCreated")
            // and handle that message in its wndproc.That message must be received by your application before it calls any ITaskbarList3 method.
            //RegisterWindowMessage(L"TaskbarButtonCreated")
        }
    }
}