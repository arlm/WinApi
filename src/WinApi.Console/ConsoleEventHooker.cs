// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using PInvoke;

namespace WinApi.Console
{
    public sealed class ConsoleEventHooker : IDisposable
    {
        private static Kernel32.HandlerRoutine callback;
        private static ConsoleEventHooker instance;

        public EventHandler Closed;

        public EventHandler Shutdown;

        public ConsoleEventHooker()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("ConsoleEventHooker already setup for this AppDomain");
            }

            instance = this;
            callback = ConsoleEventCallback;
            Kernel32.SetConsoleCtrlHandler(callback, true);
        }

        private bool ConsoleEventCallback(Kernel32.ControlType dwCtrlType)
        {
            if (dwCtrlType == Kernel32.ControlType.CTRL_CLOSE_EVENT)
            {
                Closed?.Invoke(null, new EventArgs());
            }

            if (dwCtrlType == Kernel32.ControlType.CTRL_SHUTDOWN_EVENT)
            {
                Shutdown?.Invoke(null, new EventArgs());
            }

            return false;
        }

        public void Dispose()
        {
            if (ReferenceEquals(instance, this))
            {
                Kernel32.SetConsoleCtrlHandler(callback, false);
                instance = null;
            }
        }
    }
}