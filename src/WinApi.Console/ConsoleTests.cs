// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

#define NO_UNHANDLED_EXCEPTION
#define NO_CORRUPTED_STATE_EXCEPTION
#define NO_UNOBSERVED_TASK_EXCEPTION
#define NO_CATCH_EXCEPTION
#define NO_CATCH_TASK_EXCEPTION

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace WinApi.Console
{
    public static class ConsoleTests
    {
        #region Public Methods

        [DebuggerStepThrough]
        [Conditional("UNOBSERVED_TASK_EXCEPTION")]
        [Conditional("CORRUPTED_STATE_EXCEPTION")]
        [Conditional("UNHANDLED_EXCEPTION")]
        [Conditional("DEBUG")]
        public static void RunTests()
        {
            bool useTasks = false;
            ShouldUseTasks(ref useTasks);

            if (useTasks)
            {
                TestUnsobservedTaskException();
            }
            else
            {
                TestCorruptedStateExceptions();
                TestUncatchedException();
            }
        }

        #endregion Public Methods

        #region Private Methods

        [DebuggerStepThrough]
        [Conditional("CATCH_EXCEPTION")]
        private static void ShouldCatchException(ref bool catchException)
        {
            catchException = true;
        }

        [DebuggerStepThrough]
        [Conditional("CATCH_TASK_EXCEPTION")]
        private static void ShouldCatchTaskException(ref bool catchTaskException)
        {
            catchTaskException = true;
        }

        [DebuggerStepThrough]
        [Conditional("UNOBSERVED_TASK_EXCEPTION")]
        private static void ShouldUseTasks(ref bool useTasks)
        {
            useTasks = true;
        }

        /// <summary>
        /// Here is the list of native Win32 exceptions that are considered CSEs on Structured
        /// Exception Handling (SEH) Exceptions:
        /// <list>
        /// <item>EXCEPTION_ACCESS_VIOLATION</item>
        /// <item>EXCEPTION_STACK_OVERFLOW</item>
        /// <item>EXCEPTION_ILLEGAL_INSTRUCTION</item>
        /// <item>EXCEPTION_IN_PAGE_ERROR</item>
        /// <item>EXCEPTION_INVALID_DISPOSITION</item>
        /// <item>EXCEPTION_NONCONTINUABLE_EXCEPTION</item>
        /// <item>EXCEPTION_PRIV_INSTRUCTION</item>
        /// <item>STATUS_UNWIND_CONSOLIDATE</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// The CLR converts most of these to a
        /// <see cref="SEHException"/> object. Except for
        /// EXCEPTION_ACCESS_VIOLATION, which is converted to a
        /// <see cref="AccessViolationException"/> object, and EXCEPTION_STACK_OVERFLOW, which
        /// is converted to a <see cref="StackOverflowException"/> object.
        /// </remarks>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        [DebuggerStepThrough]
        [Conditional("CORRUPTED_STATE_EXCEPTION")]
        private static void TestCorruptedStateExceptions()
        {
            bool catchException = false;
            ShouldCatchException(ref catchException);

            if (catchException)
            {
                try
                {
                    IntPtr ptr = new IntPtr(123);
                    Marshal.StructureToPtr(123, ptr, true);
                }
                catch (SEHException e)
                {
                    System.Console.Error.WriteLine($"\nSEHException: {e.ToString()}");
                }
                catch (AccessViolationException e)
                {
                    System.Console.Error.WriteLine($"\nAccessViolationException: {e.ToString()}");
                }
                catch (StackOverflowException e)
                {
                    System.Console.Error.WriteLine($"\nStackOverflowException: {e.ToString()}");
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine($"\nProcessCorruptedStateException: {e.ToString()}");
                }
            }
            else
            {
                IntPtr ptr = new IntPtr(123);
                Marshal.StructureToPtr(123, ptr, true);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        [DebuggerStepThrough]
        [Conditional("UNOBSERVED_TASK_EXCEPTION")]
        private static void TestTask(bool catchTaskException)
        {
            if (catchTaskException)
            {
                try
                {
                    TestCorruptedStateExceptions();
                    TestUncatchedException();
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine($"\nInner ObservedTaskException: {e.ToString()}");
                }
            }
            else
            {
                TestCorruptedStateExceptions();
                TestUncatchedException();
            }
        }

        [DebuggerStepThrough]
        [Conditional("UNHANDLED_EXCEPTION")]
        private static void TestUncatchedException()
        {
            ArrayList nullTest = null;
            nullTest.Contains("null");
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        [DebuggerStepThrough]
        [Conditional("UNOBSERVED_TASK_EXCEPTION")]
        private static void TestUnsobservedTaskException()
        {
            bool catchException = false;
            ShouldCatchException(ref catchException);

            bool catchTaskException = false;
            ShouldCatchTaskException(ref catchTaskException);

            if (catchException || (catchException && catchTaskException))
            {
                try
                {
                    var t = Task.Run(() => TestTask(catchTaskException));
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine($"\nObservedTaskException: {e.ToString()}");
                }
            }
            else
            {
                var t = Task.Run(() => TestTask(catchTaskException));
            }
        }

        #endregion Private Methods
    }
}