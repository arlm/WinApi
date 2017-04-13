// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.IO;

namespace WinApi.Console
{
    public sealed class ConsoleLogger : IDisposable
    {
        /// <summary>
        /// Implements IDisposable so we implement IDisposable, too!
        /// </summary>
        private readonly StreamWriter writer;

        /// <summary>
        /// Creates a new instance of Logger.
        /// </summary>
        /// <param name="fileName">File to log to.</param>
        public ConsoleLogger(string fileName)
        {
            writer = new StreamWriter(fileName);
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
            writer.Close();
        }
    }
}