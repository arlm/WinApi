// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using PInvoke;

namespace WinApi.Shell
{
    [ComVisible(true)]
    [ComImport]
    [Guid("EA1AFB91-9E28-4B86-90E9-9E9F8A5EEFAF")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ITaskbarList3 : ITaskbarList2, ITaskbarList
    {
        /// <summary>
        /// Initializes the taskbar list object. This method must be called before any other ITaskbarList methods can be called.
        /// </summary>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        new HResult HrInit();

        /// <summary>
        /// Adds an item to the taskbar.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be added to the taskbar.</param>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        new HResult AddTab(IntPtr hWnd);

        /// <summary>
        /// Deletes an item from the taskbar.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be deleted from the taskbar.</param>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        new HResult DeleteTab(IntPtr hWnd);

        /// <summary>
        /// Activates an item on the taskbar. The window is not actually activated; the window's item on the taskbar is merely displayed as active.
        /// </summary>
        /// <param name="hWnd">A handle to the window on the taskbar to be displayed as active.</param>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        new HResult ActivateTab(IntPtr hWnd);

        /// <summary>
        /// Marks a taskbar item as active but does not visually activate it.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be marked as active.</param>
        /// <remarks>From ITaskbarList</remarks>
        [PreserveSig]
        new HResult SetActiveAlt(IntPtr hWnd);

        /// <summary>
        /// Marks a window as full-screen
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fFullscreen"></param>
        /// <remarks>From ITaskbarList2</remarks>
        [PreserveSig]
        new HResult MarkFullscreenWindow(
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        /// <summary>
        /// Displays or updates a progress bar hosted in a taskbar button to show the specific percentage
        /// completed of the full operation.
        /// </summary>
        /// <param name="hWnd">The handle of the window whose associated taskbar button is being used as
        /// a progress indicator.</param>
        /// <param name="ullCompleted">An application-defined value that indicates the proportion of the
        /// operation that has been completed at the time the method is called.</param>
        /// <param name="ullTotal">An application-defined value that specifies the value ullCompleted will
        /// have when the operation is complete.</param>
        [PreserveSig]
        HResult SetProgressValue(IntPtr hWnd, ulong ullCompleted, ulong ullTotal);

        /// <summary>
        /// Sets the type and state of the progress indicator displayed on a taskbar button.
        /// </summary>
        /// <param name="hWnd">The handle of the window in which the progress of an operation is being
        /// shown. This window's associated taskbar button will display the progress bar.</param>
        /// <param name="tbpFlags">Flags that control the current state of the progress button. Specify
        /// only one of the following flags; all states are mutually exclusive of all others.</param>
        [PreserveSig]
        HResult SetProgressState(IntPtr hWnd, TBPFLAG tbpFlags);

        /// <summary>
        /// Informs the taskbar that a new tab or document thumbnail has been provided for display in an
        /// application's taskbar group flyout.
        /// </summary>
        /// <param name="hWndTab">Handle of the tab or document window. This value is required and cannot
        /// be NULL.</param>
        /// <param name="hWndMDI">Handle of the application's main window. This value tells the taskbar
        /// which application's preview group to attach the new thumbnail to. This value is required and
        /// cannot be NULL.</param>
        [PreserveSig]
        HResult RegisterTab(IntPtr hWndTab, IntPtr hWndMDI);

        /// <summary>
        /// Removes a thumbnail from an application's preview group when that tab or document is closed in the application.
        /// </summary>
        /// <param name="hWndTab">The handle of the tab window whose thumbnail is being removed. This is the same
        /// value with which the thumbnail was registered as part the group through ITaskbarList3::RegisterTab.
        /// This value is required and cannot be NULL.</param>
        [PreserveSig]
        HResult UnregisterTab(IntPtr hWndTab);

        /// <summary>
        /// Inserts a new thumbnail into a tabbed-document interface (TDI) or multiple-document interface
        /// (MDI) application's group flyout or moves an existing thumbnail to a new position in the
        /// application's group.
        /// </summary>
        /// <param name="hWndTab">The handle of the tab window whose thumbnail is being placed. This value
        /// is required, must already be registered through ITaskbarList3::RegisterTab, and cannot be NULL.</param>
        /// <param name="hWndInsertBefore">[Optional] The handle of the tab window whose thumbnail that hwndTab is
        /// inserted to the left of. This handle must already be registered through ITaskbarList3::RegisterTab.
        /// If this value is NULL, the new thumbnail is added to the end of the list.</param>
        [PreserveSig]
        HResult SetTabOrder(IntPtr hWndTab, IntPtr hWndInsertBefore);

        /// <summary>
        /// Informs the taskbar that a tab or document window has been made the active window.
        /// </summary>
        /// <param name="hWndTab">Handle of the active tab window. This handle must already be registered
        /// through ITaskbarList3::RegisterTab. This value can be NULL if no tab is active.</param>
        /// <param name="hWndMDI">Handle of the application's main window. This value tells the taskbar
        /// which group the thumbnail is a member of. This value is required and cannot be NULL.</param>
        /// <param name="tbatFlags">None, one, or both of the following values that specify a thumbnail
        /// and peek view to use in place of a representation of the specific tab or document.</param>
        [PreserveSig]
        HResult SetTabActive(IntPtr hWndTab, IntPtr hWndMDI, uint tbatFlags);

        /// <summary>
        /// Adds a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window in a
        /// taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">The handle of the window whose thumbnail representation will receive the toolbar.
        /// This handle must belong to the calling process.</param>
        /// <param name="cButtons">The number of buttons defined in the array pointed to by pButton. The maximum
        /// number of buttons allowed is 7.</param>
        /// <param name="pButton">A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an
        /// individual button to be added to the toolbar. Buttons cannot be added or deleted later, so this must
        /// be the full defined set. Buttons also cannot be reordered, so their order in the array, which is the
        /// order in which they are displayed left to right, will be their permanent order.</param>
        [PreserveSig]
        HResult ThumbBarAddButtons(
            IntPtr hWnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButton);

        /// <summary>
        /// Updates a thumbnail toolbar with a specified set of buttons to the thumbnail image of a window in a
        /// taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">The handle of the window whose thumbnail representation will receive the toolbar.
        /// This handle must belong to the calling process.</param>
        /// <param name="cButtons">The number of buttons defined in the array pointed to by pButton. The maximum
        /// number of buttons allowed is 7.</param>
        /// <param name="pButton">A pointer to an array of THUMBBUTTON structures. Each THUMBBUTTON defines an
        /// individual button to be added to the toolbar. Buttons cannot be added or deleted later, so this must
        /// be the full defined set. Buttons also cannot be reordered, so their order in the array, which is the
        /// order in which they are displayed left to right, will be their permanent order.</param>
        [PreserveSig]
        HResult ThumbBarUpdateButtons(
            IntPtr hWnd,
            uint cButtons,
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButton);

        /// <summary>
        /// Specifies an image list that contains button images for a toolbar embedded in a thumbnail image of a
        /// window in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">The handle of the window whose thumbnail representation contains the toolbar to be
        /// updated. This handle must belong to the calling process.</param>
        /// <param name="himl">The handle of the image list that contains all button images to be used in the toolbar.</param>
        [PreserveSig]
        HResult ThumbBarSetImageList(
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.IUnknown)] object himl);

        /// <summary>
        /// Applies an overlay to a taskbar button to indicate application status or a notification to the user.
        /// </summary>
        /// <param name="hWnd">The handle of the window whose associated taskbar button receives the overlay.
        /// This handle must belong to a calling process associated with the button's application and must be
        /// a valid HWND or the call is ignored.</param>
        /// <param name="hIcon">The handle of an icon to use as the overlay. This should be a small icon,
        /// measuring 16x16 pixels at 96 dots per inch (dpi). If an overlay icon is already applied to the
        /// taskbar button, that existing overlay is replaced.</param>
        /// <param name="pszDescription">A pointer to a string that provides an alt text version of the
        /// information conveyed by the overlay, for accessibility purposes.</param>
        [PreserveSig]
        HResult SetOverlayIcon(
            IntPtr hWnd, 
            IntPtr hIcon, 
            [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);

        /// <summary>
        /// Specifies or updates the text of the tooltip that is displayed when the mouse pointer rests on an
        /// individual preview thumbnail in a taskbar button flyout.
        /// </summary>
        /// <param name="hWnd">The handle to the window whose thumbnail displays the tooltip. This handle must
        /// belong to the calling process.</param>
        /// <param name="pszTip">The pointer to the text to be displayed in the tooltip. This value can be NULL,
        /// in which case the title of the window specified by hwnd is used as the tooltip.</param>
        [PreserveSig]
        HResult SetThumbnailTooltip(
            IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr)] string pszTip);

        /// <summary>
        /// Selects a portion of a window's client area to display as that window's thumbnail in the taskbar.
        /// </summary>
        /// <param name="hWnd">The handle to a window represented in the taskbar.</param>
        /// <param name="prcClip">[In, RECT*] A pointer to a RECT structure that specifies a selection within the window's
        /// client area, relative to the upper-left corner of that client area. To clear a clip that is already
        /// in place and return to the default display of the thumbnail, set this parameter to NULL.</param>
        [PreserveSig]
        HResult SetThumbnailClip(IntPtr hWnd, ref RECT prcClip);
    }
}
