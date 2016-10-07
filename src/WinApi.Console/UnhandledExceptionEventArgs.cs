// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace WinApi.Console
{
    public class UnhandledExceptionEventArgs : System.UnhandledExceptionEventArgs
    {
        public UnhandledExceptionEventArgs(object exception, bool isTerminating) : base(exception, isTerminating)
        {
        }

        public UnhandledExceptionEventArgs(System.UnhandledExceptionEventArgs e) : base(e?.ExceptionObject, e?.IsTerminating ?? false)
        {
        }

        public UnhandledExceptionEventArgs(UnobservedTaskExceptionEventArgs e) : base(e?.Exception.Flatten(), !e?.Observed ?? false)
        {
        }

        public UnhandledExceptionEventArgs(FirstChanceExceptionEventArgs e) : base(e?.Exception, false)
        {
        }

        public int ExitCode { get; set; } = int.MinValue;
    }
}