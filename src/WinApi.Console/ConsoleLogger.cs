// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace WinApi.Console
{
    public class ConsoleLogger : IDisposable
    {
        /// <summary>
        /// Implements IDisposable so we implement IDisposable, too!
        /// </summary>
        private StreamWriter _writer;

        /// <summary>
        /// Creates a new instance of Logger.
        /// </summary>
        /// <param name="filename">File to log to.</param>
        public ConsoleLogger(string filename)
        {
            _writer = new StreamWriter(filename);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ConsoleLogger()
        {
            // We should never get into this point. Getting here is an error of the developer!
            Debug.WriteLine("Error - we forgot to dispose {0}", GetType().FullName);
            Dispose(false);
        }

        /// <summary>
        /// Dispose / close this instance.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Dispose / close this instance.
        /// </summary>
        public void Dispose()
        {
            // Do a full dispose
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Call base class
            //base.Dispose(disposing);

            if (disposing)
            {
                // Dispose managed stuff
                _writer?.Close();
            }

            // Dispose unmanaged stuff.
        }

        // Some usefull methods are required here to write log messages.
    }
}