// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.InteropServices;

namespace WinApi.Console
{
    internal static class NativeAPI
    {
        /// _CALL_REPORTFAULT -> 0x2
        public const int _CALL_REPORTFAULT = 2;

        /// _ONEXIT_T_DEFINED ->
        /// Error generating expression: Der Wert darf nicht NULL sein.
        ///Parametername: node
        public const string _ONEXIT_T_DEFINED = "";

        /// _WRITE_ABORT_MSG -> 0x1
        public const int _WRITE_ABORT_MSG = 1;

        /// CREATE_NEW_CONSOLE -> 0x00000010
        public const int CREATE_NEW_CONSOLE = 16;

        /// EVENT_CONSOLE_CARET -> 0x4001
        public const int EVENT_CONSOLE_CARET = 16385;

        /// EVENT_CONSOLE_LAYOUT -> 0x4005
        public const int EVENT_CONSOLE_LAYOUT = 16389;

        /// EVENT_CONSOLE_START_APPLICATION -> 0x4006
        public const int EVENT_CONSOLE_START_APPLICATION = 16390;

        /// EVENT_CONSOLE_UPDATE_REGION -> 0x4002
        public const int EVENT_CONSOLE_UPDATE_REGION = 16386;

        /// EVENT_CONSOLE_UPDATE_SCROLL -> 0x4004
        public const int EVENT_CONSOLE_UPDATE_SCROLL = 16388;

        /// EVENT_CONSOLE_UPDATE_SIMPLE -> 0x4003
        public const int EVENT_CONSOLE_UPDATE_SIMPLE = 16387;

        /// SEE_MASK_NO_CONSOLE -> 0x00008000
        public const int SEE_MASK_NO_CONSOLE = 32768;

        /// Return Type: int
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int _onexit_t();

        /// Return Type: void
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void atexit_function();

        /// Return Type: int
        ///param0: Anonymous_a3debd67_ecba_49a0_9c67_1b83f463a375
        [DllImport("msvcr80.dll", EntryPoint = "atexit", CallingConvention = CallingConvention.Cdecl)]
        public static extern int atexit(atexit_function param0);

        /// Return Type: terminate_function
        [DllImport("msvcr80.dll", EntryPoint = "_get_terminate")]
        public static extern terminate_function _get_terminate();

        /// Return Type: unexpected_function
        [DllImport("msvcr80.dll", EntryPoint = "_get_unexpected")]
        public static extern unexpected_function _get_unexpected();

        /// Return Type: _onexit_t
        ///_Func: _onexit_t
        [DllImport("msvcr80.dll", EntryPoint = "_onexit", CallingConvention = CallingConvention.Cdecl)]
        public static extern _onexit_t _onexit(_onexit_t _Func);

        /// Return Type: unsigned int
        ///_Flags: unsigned int
        ///_Mask: unsigned int
        [DllImport("msvcr80.dll", EntryPoint = "_set_abort_behavior", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint _set_abort_behavior(uint _Flags, uint _Mask);

        /// Return Type: void
        ///param0: terminate_function
        [DllImport("msvcr80.dll", EntryPoint = "set_terminate", CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_terminate(terminate_function param0);

        /// Return Type: void
        ///param0: int
        [DllImport("msvcr80.dll", EntryPoint = "set_unexpected", CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_unexpected(int param0);

        /// Return Type: int
        ///sig: int
        ///param1: signal_function
        [DllImport("msvcr80.dll", EntryPoint = "signal", CallingConvention = CallingConvention.Cdecl)]
        private static extern int signal(int sig, signal_function param1);

        /// Return Type: void
        ///param0: int[]
        public delegate void signal_function(int[] param0);

        /// Return Type: void
        public delegate void terminate_function();

        /// Return Type: void
        public delegate void unexpected_function();
    }
}
