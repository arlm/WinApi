// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using static PInvoke.User32;

namespace WinApi.User32
{
    public static partial class WindowMessage
    {
        public static class Send
        {
            /// <summary>
            /// Copies the text that corresponds to a window into a buffer provided by the caller.
            /// </summary>
            /// <param name="hwnd">The window or control handle</param>
            /// <param name="length">
            /// The maximum number of characters to be copied, including the terminating null character.
            /// ANSI applications may have the string in the buffer reduced in size (to a minimum of half that of the wParam value) due to conversion from ANSI to Unicode.
            /// </param>
            /// <returns>The string of the requested window or control</returns>
            /// <remarks>
            /// <para>
            /// The DefWindowProc function copies the text associated with the window into the specified buffer and returns the number of characters copied.
            /// Note, for non-text static controls this gives you the text with which the control was originally created, that is, the ID number.
            /// However, it gives you the ID of the non-text static control as originally created.
            /// That is, if you subsequently used a STM_SETIMAGE to change it the original ID would still be returned.
            /// </para>
            /// <para>
            /// For an edit control, the text to be copied is the content of the edit control.
            /// For a combo box, the text is the content of the edit control (or static-text) portion of the combo box.
            /// For a button, the text is the button name.
            /// For other windows, the text is the window title.
            /// To copy the text of an item in a list box, an application can use the LB_GETTEXT message.
            /// </para>
            /// <para>
            /// When the WM_GETTEXT message is sent to a static control with the SS_ICON style, a handle to the icon will be returned in the first four bytes of the buffer pointed to by lParam.
            /// This is true only if the WM_SETTEXT message has been used to set the icon.
            /// Rich Edit: If the text to be copied exceeds 64K, use either the EM_STREAMOUT or EM_GETSELTEXT message.
            /// </para>
            /// <para>
            /// Sending a WM_GETTEXT message to a non-text static control, such as a static bitmap or static icon control, does not return a string value.
            /// nstead, it returns zero.
            /// In addition, in early versions of Windows, applications could send a WM_GETTEXT message to a non-text static control to retrieve the control's ID.
            /// To retrieve a control's ID, applications can use GetWindowLong passing GWL_ID as the index value or GetWindowLongPtr using GWLP_ID.
            /// </para>
            /// </remarks>
            public static unsafe string WM_GETTEXT(IntPtr hwnd, int length = 256) // Any good enought value
            {
                if (length == 0)
                {
                    return string.Empty;
                }

                char* windowText = stackalloc char[length + 1];
                int capacity = length + 1;

                var result = SendMessage(hwnd, PInvoke.User32.WindowMessage.WM_GETTEXT, &capacity, windowText);

                if (result == IntPtr.Zero)
                {
                    return string.Empty;
                }

                return new string(windowText, 0, result.ToInt32());
            }

            /// <summary>
            /// Determines the length, in characters, of the text associated with a window.
            /// </summary>
            /// <param name="hwnd">The window or control handle</param>
            /// <returns>The return value is the length of the text in characters, not including the terminating null character.</returns>
            /// <remarks>
            /// <para>
            /// For an edit control, the text to be copied is the content of the edit control. For a combo box, the text is the content of the edit control (or static-text) portion of the combo box.
            /// For a button, the text is the button name. For other windows, the text is the window title. To determine the length of an item in a list box, an application can use the LB_GETTEXTLEN message.
            /// </para>
            /// <para>
            /// When the WM_GETTEXTLENGTH message is sent, the DefWindowProc function returns the length, in characters, of the text.
            /// Under certain conditions, the DefWindowProc function returns a value that is larger than the actual length of the text. This occurs with certain mixtures of ANSI and Unicode,
            /// and is due to the system allowing for the possible existence of double-byte character set (DBCS) characters within the text. The return value, however,
            /// will always be at least as large as the actual length of the text; you can thus always use it to guide buffer allocation.
            /// This behavior can occur when an application uses both ANSI functions and common dialogs, which use Unicode.
            /// </para>
            /// <para>
            /// To obtain the exact length of the text, use the <see cref="WM_GETTEXT"/>, LB_GETTEXT, or CB_GETLBTEXT messages, or the GetWindowText function.
            /// </para>
            /// <para>
            /// Sending a WM_GETTEXTLENGTH message to a non-text static control, such as a static bitmap or static icon controlc, does not return a string value. Instead, it returns zero.
            /// </para>
            /// </remarks>
            public static int WM_GETTEXTLENGTH(IntPtr hwnd)
            {
                return SendMessage(hwnd, PInvoke.User32.WindowMessage.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero).ToInt32();
            }
        }
    }
}