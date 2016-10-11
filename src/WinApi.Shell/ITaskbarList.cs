// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.Shell
{
    [ComVisible(true)]
    [ComImport]
    [Guid("56FDF342-FD6D-11d0-958A-006097C9A090")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ITaskbarList
    {
        /// <summary>
        /// Initializes the taskbar list object. This method must be called before any other ITaskbarList methods can be called.
        /// </summary>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        HResult HrInit();

        /// <summary>
        /// Adds an item to the taskbar.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be added to the taskbar.</param>
        [PreserveSig]
        HResult AddTab(IntPtr hWnd);

        /// <summary>
        /// Deletes an item from the taskbar.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be deleted from the taskbar.</param>
        [PreserveSig]
        HResult DeleteTab(IntPtr hWnd);

        /// <summary>
        /// Activates an item on the taskbar. The window is not actually activated; the window's item on the taskbar is merely displayed as active.
        /// </summary>
        /// <param name="hWnd">A handle to the window on the taskbar to be displayed as active.</param>
        [PreserveSig]
        HResult ActivateTab(IntPtr hWnd);

        /// <summary>
        /// Marks a taskbar item as active but does not visually activate it.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be marked as active.</param>
        [PreserveSig]
        HResult SetActiveAlt(IntPtr hWnd);
    }
}
