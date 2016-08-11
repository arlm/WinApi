// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WinApi.Console
{
    internal static class InternalHelpers
    {
        public const string EXCEPTION_SEPARATOR = "----------------------------------------";

        public static string Flatten(this Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);
                stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

                exception = exception.InnerException;
            }

            return stringBuilder.ToString();
        }

        public static T GetAttribute<T>(this Assembly assembly)
                            where T : Attribute
        {
            return GetAttributes<T>(assembly).FirstOrDefault();
        }

        public static IEnumerable<T> GetAttributes<T>(this Assembly assembly)
            where T : Attribute
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var attributes = assembly.GetCustomAttributes<T>();

            return attributes;
        }

        public static string GetMetaData(this Assembly assembly, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var attributes = GetAttributes<AssemblyMetadataAttribute>(assembly);

            var values = from item in attributes
                         where item.Key == key
                         select item.Value;

            return values.FirstOrDefault();
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }

        public static string ToFlattenedString(this AggregateException exception)
        {
            var stringBuilder = new StringBuilder();
            var flattenedException = exception?.Flatten();

            stringBuilder.AppendLine(flattenedException.Message);
            stringBuilder.AppendLine(flattenedException.StackTrace);
            stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

            stringBuilder.AppendLine("BASE EXCEPTION");
            stringBuilder.AppendLine(flattenedException.GetBaseException().Flatten());

            stringBuilder.AppendLine("INNER EXCEPTION");
            stringBuilder.AppendLine(flattenedException.InnerException.Flatten());

            stringBuilder.AppendLine("AGGREGATED INNER EXCEPTIONS");
            stringBuilder.AppendLine(EXCEPTION_SEPARATOR);

            foreach (var ex in flattenedException.InnerExceptions)
            {
                stringBuilder.AppendLine(ex.Flatten());
            }

            return stringBuilder.ToString();
        }

        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);

            var processId = (int)parentId.NextValue();

            return processId == 0 ? null : Process.GetProcessById(processId);
        }
    }
}