// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinApi.Kernel32
{
    public static class Clipboard
    {
        public static bool SetText(string text)
        {
            //IntPtr ptr = IntPtr.Zero;
            //bool result = false;

            //result = PInvoke.User32.OpenClipboard(IntPtr.Zero);
            //if (!result)
            //{
            //    return result;
            //}

            //try
            //{
            //    result &= PInvoke.User32.EmptyClipboard();

            //    if (result)
            //    {
            //        // IMPORTANT: SetClipboardData requires memory that was acquired with GlobalAlloc
            //        //            using GMEM_MOVABLE.
            //        ptr = CopyToMoveableMemory(text);
            //        result &= ptr == IntPtr.Zero;

            //        result &= PInvoke.User32.SetClipboardData(13, ptr).ToInt64() == 0;
            //    }
            //}
            //finally
            //{
            //    result &= PInvoke.User32.CloseClipboard();

            //    if (ptr != IntPtr.Zero)
            //    {
            //        PInvoke.Kernel32.GlobalFree(ptr);
            //    }
            //}

            //return result;

            return false;
        }

        /// <summary>
        /// Creates a memory block with GlobalAlloc(GMEM_MOVEABLE), copies the data into it, and
        /// returns the handle to the memory.
        /// </summary>
        /// <param name="data">The data. Must not be null or zero-length — see the exception notes.</param>
        /// <returns>The *handle* to the allocated GMEM_MOVEABLE block.</returns>
        /// <exception cref="T:System.ArgumentException">
        /// The data was null or zero length. This is disallowed since a zero length allocation can't
        /// be made
        /// </exception>
        /// <exception cref="T:System.ComponentModel.Win32Exception">
        /// The allocation, or locking (handle-&gt;pointer) failed. Either out of memory or the
        /// handle table is full (256 max currently). Note Win32Exception is a subclass of
        /// ExternalException so this is OK in the documented Clipboard interface.
        /// </exception>
        //internal static IntPtr CopyToMoveableMemory(byte[] data)
        //{
        //    // detect this before GlobalAlloc does.
        //    if (data == null || data.Length == 0)
        //    {
        //        throw new ArgumentException("Can't create a zero length memory block.");
        //    }

        //    IntPtr hmem = PInvoke.Kernel32.GlobalAlloc(
        //        PInvoke.Kernel32.GblobalAllocFlags.GMEM_MOVEABLE | PInvoke.Kernel32.GblobalAllocFlags.GMEM_DDESHARE, 
        //        new UIntPtr(unchecked((uint)data.Length)));

        //    if (hmem == IntPtr.Zero)
        //    {
        //        throw new PInvoke.Win32Exception();
        //    }

        //    IntPtr hmem_ptr = PInvoke.Kernel32.GlobalLock(hmem);

        //    // If the allocation was valid this shouldn't occur.
        //    if (hmem_ptr == IntPtr.Zero)
        //    {
        //        throw new PInvoke.Win32Exception();
        //    }

        //    Marshal.Copy(data, 0, hmem_ptr, data.Length);
        //    PInvoke.Kernel32.GlobalUnlock(hmem);

        //    return hmem;
        //}

        internal static IntPtr CopyToMoveableMemory(string data)
        {
            //return CopyToMoveableMemory(Encoding.UTF8.GetBytes(data));

            return IntPtr.Zero;
        }
    }
}