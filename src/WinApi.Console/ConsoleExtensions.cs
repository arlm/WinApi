// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Diagnostics;

namespace WinApi.Console
{
    public static class ConsoleExtensions
    {
        public static void WaitIfDebugging(string message)
        {
            var parentProcess = Process.GetCurrentProcess().Parent();

            if ((parentProcess != null && parentProcess.ProcessName != "cmd") ||
                Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(message);
                System.Console.ReadKey();
            }
        }

        public static void WaitIfNotDebugging(string message)
        {
            var parentProcess = Process.GetCurrentProcess().Parent();

            if ((parentProcess != null && parentProcess.ProcessName == "cmd") &&
                !Debugger.IsAttached)
            {
                System.Console.Out.WriteLine(message);
                System.Console.ReadKey();
            }
        }
    }
}
