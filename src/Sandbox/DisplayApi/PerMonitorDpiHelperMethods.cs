// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using PInvoke;

namespace Sandbox
{
    internal static class PerMonitorDpiHelperMethods
    {
        public static Dpi GetDpi(this HwndSource hwndSource, MONITOR_DPI_TYPE dpiType = MONITOR_DPI_TYPE.MDT_DEFAULT)
        {
            return DpiAwareApi.GetDpiForWindow(hwndSource.Handle, dpiType);
        }

        public static Dpi GetSystemDpi(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null && source.CompositionTarget != null)
            {
                return new Dpi(
                    (int)(Dpi.Default.X * source.CompositionTarget.TransformToDevice.M11),
                    (int)(Dpi.Default.Y * source.CompositionTarget.TransformToDevice.M22));
            }

            return Dpi.Default;
        }
    }
}