// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using Microsoft.Win32;

namespace WinApi.HighDpi
{
    internal static class InternalHelpers
    {
        public static T GetValue<T>(this RegistryKey registryKey, string subKey, string valueName)
        {
            try
            {
                using (var key = registryKey.OpenSubKey(subKey))
                {
                    if (key == null)
                    {
                        return default(T);
                    }

                    return (T)registryKey.GetValue(valueName, default(T));
                }
            }
            catch (Exception)
            {
            }

            return default(T);
        }

        public static T GetValue<T>(this RegistryKey registryKey, string subKeyWithValueName)
        {
            var keys = subKeyWithValueName.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return GetValue<T>(registryKey, subKeys, keys[keys.Length - 1]);
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string subKey, string valueName, T value)
        {
            try
            {
                using (var key = registryKey.OpenSubKey(subKey))
                {
                    if (key == null)
                    {
                        return false;
                    }

                    registryKey.SetValue(valueName, value);
                    registryKey.Flush();
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        public static bool SetValue<T>(this RegistryKey registryKey, string subKeyWithValueName, T value)
        {
            var keys = subKeyWithValueName.Split('\\');
            var subKeys = string.Join("\\", keys, 0, keys.Length - 1);

            return SetValue(registryKey, subKeys, keys[keys.Length - 1], value);
        }
    }
}