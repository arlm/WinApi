// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Threading;

namespace Sandbox
{
    public static class OleApi
    {
        private const string OLE_INITIALIZARION_FAILURE = "OLE initialization failed.";
        private const string THREAD_MUST_BE_STA = "Current thread must be set to single thread apartment (STA) mode before OLE calls can be made. Ensure that your Main function has STAThreadAttribute marked on it. This exception is only raised if a debugger is attached to the process.";

        ///<SecurityNote>
        ///  Critical as this code performs an elevation.
        ///</SecurityNote>
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("ole32", ExactSpelling = true, SetLastError = true)]
        public static extern int OleInitialize(IntPtr val);

        // Wrapper for UnsafeNativeMethods.OleInitialize, useful for debugging.
        /// <SecurityNote>
        /// Critical - calls critical method (OleInitialize) TreatAsSafe - safe to call anytime (ref
        /// counting issues aside)
        /// </SecurityNote>
        [SecurityCritical]
        public static int OleInitialize()
        {
            return OleInitialize(IntPtr.Zero);
        }

        /// <SecurityNote>
        /// Critical: This code calls into unmanaged code which elevates TreatAsSafe - safe to call OLeUninitialize
        /// </SecurityNote>
        /// ///
        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("ole32", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int OleUninitialize();

        /// <summary>
        /// This is a callback when Dispatcher is shut down.
        /// </summary>
        /// <remarks>
        /// This method must be called before shutting down the application on the dispatcher thread.
        /// It must be called by the same thread running the dispatcher and the thread must have its
        /// ApartmentState property set to ApartmentState.STA.
        /// </remarks>
        public static void OnDispatcherShutdown(object sender, EventArgs args)
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new ThreadStateException(THREAD_MUST_BE_STA);
            }

            // Uninitialize Ole services. Balanced with OleInitialize call in SetDispatcherThread.
            OleUninitialize();
        }

        /// <summary>
        /// SetDispatcherThread - Initialize OleServicesContext that will call Ole initialize for ole
        /// services(DragDrop and Clipboard) and add the disposed event handler of Dispatcher to
        /// clean up resources and uninitalize Ole.
        /// </summary>
        public static void SetDispatcherThread()
        {
            int hr;

            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            {
                throw new ThreadStateException(THREAD_MUST_BE_STA);
            }

            // Initialize Ole services. Balanced with OleUninitialize call in OnDispatcherShutdown.
            hr = OleInitialize();

            if (!Succeeded(hr))
            {
                throw new SystemException(OLE_INITIALIZARION_FAILURE);
            }

            // Add Dispatcher.Shutdown event handler. We will call ole Uninitialize and clean up the
            // resource when UIContext is terminated.
            Dispatcher.CurrentDispatcher.ShutdownFinished += OnDispatcherShutdown;
        }

        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }
    }
}