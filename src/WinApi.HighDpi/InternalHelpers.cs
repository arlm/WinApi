// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using Microsoft.Win32;

namespace WinApi.HighDpi
{
    internal static class InternalHelpers
    {
        public static T GetValue<T>(this RegistryKey registryKey, string key, string valueName)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueName == null)
                throw new ArgumentNullException(nameof(valueName));

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey == null)
                {
                    return default(T);
                }

                return (T)registryKey.GetValue(valueName, default(T));
            }
        }

        public static T GetValue<T>(this RegistryKey registryKey, string key)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return GetValue<T>(registryKey, subKeys, keys[keys.Length - 1]);
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string key, string valueName, T value)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (valueName == null)
                throw new ArgumentNullException(nameof(valueName));

            using (var subKey = registryKey.OpenSubKey(key))
            {
                if (subKey == null)
                {
                    return false;
                }

                registryKey.SetValue(valueName, value);
                registryKey.Flush();
                return true;
            }
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string key, T value)
        {
            if (registryKey == null)
                throw new ArgumentNullException(nameof(registryKey));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            var keys = key.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return SetValue(registryKey, subKeys, keys[keys.Length - 1], value);
        }
    }
}