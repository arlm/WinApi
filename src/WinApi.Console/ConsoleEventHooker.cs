// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;

namespace WinApi.Console
{
    // An enumerated type for the control messages sent to the handler routine.
    public enum CtrlTypes
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }

    public static class ConsoleEventHooker
    {
        private static EventHandler _closed;
        private static ConsoleEventDelegate _d;
        private static bool _initedHooker;
        private static EventHandler _shutdown;

        // A delegate type to be used as the handler routine for SetConsoleCtrlHandler.
        private delegate bool ConsoleEventDelegate(CtrlTypes ctrlType);

        public static event EventHandler Closed
        {
            add
            {
                Init();

                _closed += value;
            }
            remove
            {
                _closed -= value;
            }
        }

        public static event EventHandler Shutdown
        {
            add
            {
                Init();

                _shutdown += value;
            }
            remove
            {
                _shutdown -= value;
            }
        }

        private static bool ConsoleEventCallback(CtrlTypes eventType)
        {
            if (eventType == CtrlTypes.CTRL_CLOSE_EVENT)
            {
                _closed?.Invoke(null, new EventArgs());
            }

            if (eventType == CtrlTypes.CTRL_SHUTDOWN_EVENT)
            {
                _shutdown?.Invoke(null, new EventArgs());
            }

            return false;
        }

        private static void Init()
        {
            if (_initedHooker)
            {
                return;
            }

            _initedHooker = true;
            _d = ConsoleEventCallback;
            //PInvoke.Kernel32.SetConsoleCtrlHandler(_d, true);
        }        
    }
}