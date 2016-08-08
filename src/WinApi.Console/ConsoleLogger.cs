// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;

namespace WinApi.Console
{
    public class ConsoleLogger : IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Implements IDisposable so we implement IDisposable, too!
        /// </summary>
        private StreamWriter _writer;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Creates a new instance of Logger.
        /// </summary>
        /// <param name="filename">File to log to.</param>
        public ConsoleLogger(string filename)
        {
            _writer = new StreamWriter(filename);
        }

        #endregion Public Constructors

        #region Private Destructors

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ConsoleLogger()
        {
            // We should never get into this point. Getting here is an error of the developer!
            Debug.WriteLine("Error - we forgot to dispose {0}", GetType().FullName);
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Methods

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

        #endregion Public Methods

        #region Protected Methods

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

        #endregion Protected Methods

        // Some usefull methods are required here to write log messages.
    }
}