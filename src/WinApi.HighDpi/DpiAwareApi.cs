// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.Win32;
using PInvoke;

namespace WinApi.HighDpi
{
    public static class DpiAwareApi
    {
        private const string HKCU_CONTROL_PANEL_DESKTOP_DESKTOP_DPI_OVERRIDE = @"Control Panel\Desktop\DesktopDPIOverride";
        private const string HKCU_CONTROL_PANEL_DESKTOP_LOGPIXELS = @"Control Panel\Desktop\LogPixels";
        private const string HKCU_CONTROL_PANEL_DESKTOP_WIN8_DPI_SCALING = @"Control Panel\Desktop\Win8DpiScaling";

        /// <summary>
        /// <para>
        /// Checks if the Control Panel\Appearance and Personalization\Display user interface (UI) includes a checkbox: "Let me choose one scaling level for all my displays" is disabled,
        /// and gets the scaling factor the user provided to override the current scale factors, from Smaller, to Medium, to Larger.
        /// </para>
        /// <para>
        /// Possible values are:
        /// <list>
        /// <item>Key value &lt; 0, Meaning: Reduce each display scale factor from the default by this value (for example, if the default was 150% scaling, -1 corresponds to 125%, -2 to 100%).</item>
        /// <item>Key value = 0, Meaning: Use the default value for each display.</item>
        /// <item>Key value &gt; 0, Meaning: Increase each display factor by this value (using the previous example, +1 corresponds to 200% scaling).</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// All display scale factors in this mode are constrained to be one of these four values: 100%, 125%, 150%, 200%. In addition, after scaling is applied,
        /// applications expect to have at least 720 effective lines of resolution (that is, the physical vertical resolution of the display divided by the scale factor);
        /// this can further limit the range of allowed display scale factors.
        /// </remarks>
        public static int DesktopDPIOverride
        {
            get
            {
                if (Win8DpiScaling)
                {
                    return 0;
                }

                // Key value &lt; 0, Meaning: Reduce each display scale factor from the default by this value (for example, if the default was 150% scaling, -1 corresponds to 125%, -2 to 100%).
                // Key value = 0, Meaning: Use the default value for each display.
                // Key value &gt; 0, Meaning: Increase each display factor by this value (using the previous example, +1 corresponds to 200% scaling).
                return Registry.CurrentUser.GetValue<int>(HKCU_CONTROL_PANEL_DESKTOP_DESKTOP_DPI_OVERRIDE);
            }
        }

        /// <summary>
        /// <para>
        /// Checks if the Control Panel\Appearance and Personalization\Display user interface (UI) includes a checkbox: "Let me choose one scaling level for all my displays" is disabled,
        /// and gets the scaling factor the user provided to override the current scale factors, from Smaller, to Medium, to Larger.
        /// </para>
        /// </summary>
        /// <remarks>
        /// All display scale factors in this mode are constrained to be one of these four values: 100%, 125%, 150%, 200%. In addition, after scaling is applied,
        /// applications expect to have at least 720 effective lines of resolution (that is, the physical vertical resolution of the display divided by the scale factor);
        /// this can further limit the range of allowed display scale factors.
        /// </remarks>
        public static ScaleFactors DesktopDPIOverrideScaleFactor
        {
            get
            {
                var possibleValues = Enum.GetValues(typeof(ScaleFactors)).Cast<ScaleFactors>().ToArray();
                var defaultValue = 0;

                return possibleValues[defaultValue + DesktopDPIOverride];
            }
        }

        /// <summary>
        /// Checks if the Windows version is Vista (Windows Server 2008) or higher.
        /// </summary>
        public static bool HasDpiSupport
        {
            get
            {
                var version = Environment.OSVersion.Version;
                return ((version.Major * 1000) + version.Minor) >= 6000; // Windows Vista or Windows Server 2008: 6.0
            }
        }

        /// <summary>
        /// Checks if the Windows version is 8.1 (Windows Server 2012 R2) or higher.
        /// </summary>
        public static bool HasPerMonitorDpiSupport
        {
            get
            {
                var version = Environment.OSVersion.Version;
                var result = ((version.Major * 1000) + version.Minor) >= 6003; // Windows 8.1 or Windows Server 2012 R2: 6.3

                return result;
            }
        }

        public static Dpi SystemDPI
        {
            get
            {
                var dpi = Dpi.Default;
                var desktopDC = User32.SafeDCHandle.Null;

                try
                {
                    desktopDC = User32.GetDC(IntPtr.Zero);

                    if (!desktopDC.IsInvalid)
                    {
                        dpi.X = Gdi32.GetDeviceCaps(desktopDC, Gdi32.DeviceCap.LOGPIXELSX);
                        dpi.Y = Gdi32.GetDeviceCaps(desktopDC, Gdi32.DeviceCap.LOGPIXELSY);
                    }
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                }
                finally
                {
                    if (!desktopDC.IsInvalid)
                    {
                        desktopDC.Dispose();
                    }
                }

                return dpi;
            }
        }

        /// <summary>
        /// When the Let me choose one scaling level for all my displays checkbox is checked, the user can specify a scale factor that applies to all displays,
        /// regardless of each display’s pixel density. By using the custom setting, the user can select values other than 100%, 125%, 150%, 200%,
        /// although they are limited to the range (100%-500%).
        /// It will return zero (0) if the setting is not there.
        /// </summary>
        public static int SystemWideScaleFactor
        {
            get
            {
                if (Win8DpiScaling)
                {
                    var scaleFactor = Registry.CurrentUser.GetValue<int>(HKCU_CONTROL_PANEL_DESKTOP_LOGPIXELS);
                    return scaleFactor / 96;
                }

                return 0;
            }
        }

        /// <summary>
        /// Checks if the Control Panel\Appearance and Personalization\Display user interface (UI) includes a checkbox: "Let me choose one scaling level for all my displays" is enabled.
        /// </summary>
        /// <remarks>
        /// It controls whether the system applies a single scale factor to all displays (as in Windows® 8 and earlier versions of Windows),
        /// or different scale factors that take into account the pixel density of each display (the Windows 8.1 default).
        /// </remarks>
        public static bool Win8DpiScaling
        {
            get
            {
                // Key value = 0, Meaning: Different scale factors for each display: Windows 8.1 default.Content that is moved from one display to another will be the right size, but can be bitmap - scaled.
                // Key value = 1, Meaning: Same scale factor is applied to all displays: Windows 8 and earlier Windows versions behavior.Content that is moved from one display to another might be the wrong size
                return Registry.CurrentUser.GetValue<int>(HKCU_CONTROL_PANEL_DESKTOP_WIN8_DPI_SCALING) == 1;
            }
        }

        public static Dpi GetDpiForMonitor(IntPtr hwndMonitor, MONITOR_DPI_TYPE dpiType = MONITOR_DPI_TYPE.MDT_DEFAULT)
        {
            if (!HasPerMonitorDpiSupport || hwndMonitor == IntPtr.Zero)
            {
                if (HasDpiSupport)
                {
                    return SystemDPI;
                }
                else
                {
                    return Dpi.Default;
                }
            }

            int dpiX = 96;
            int dpiY = 96;
            if (SHCore.GetDpiForMonitor(hwndMonitor, dpiType, out dpiX, out dpiY).Failed)
            {
                return Dpi.Default;
            }

            return new Dpi(dpiX, dpiY);
        }

        public static Dpi GetDpiForWindow(IntPtr hwnd, MONITOR_DPI_TYPE dpiType = MONITOR_DPI_TYPE.MDT_DEFAULT)
        {
            if (!HasPerMonitorDpiSupport || hwnd == IntPtr.Zero)
            {
                if (HasDpiSupport)
                {
                    return SystemDPI;
                }
                else
                {
                    return Dpi.Default;
                }
            }

            var monitor = User32.MonitorFromWindow(hwnd, User32.MonitorOptions.MONITOR_DEFAULTTONEAREST);

            return GetDpiForMonitor(monitor, dpiType);
        }

        /// <summary>
        /// Retrieves the dots per inch (dpi) awareness of the specified process.
        /// Compatible with all Windows versions.
        /// </summary>
        /// <param name="hprocess">Handle of the process that is being queried. If this parameter is NULL (<see cref="IntPtr.Zero"/>, the current process is queried</param>
        /// <returns>The DPI awareness of the specified process from the <see cref="PROCESS_DPI_AWARENESS"/> enumeration.</returns>
        /// <remarks>
        /// Windows versions prior to Windows Vista do not support High DPI.
        /// This method will return <see cref="PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE"/> on these cases.
        /// </remarks>
        public static PROCESS_DPI_AWARENESS GetProcessDpiAwareness(IntPtr hprocess)
        {
            var awareness = PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;

            try
            {
                var result = SHCore.GetProcessDpiAwareness(hprocess, out awareness);

                if (result.Failed)
                {
                    result.ThrowOnFailure();
                }
            }
            catch (DllNotFoundException)
            {
                try
                {
                    // We're running on either Vista, Windows 7 or Windows 8.
                    // Return the correct ProcessDpiAwareness value.
                    awareness = User32.IsProcessDPIAware() ? PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE : PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                }
                catch (EntryPointNotFoundException)
                {
                }
            }
            catch (EntryPointNotFoundException)
            {
                try
                {
                    // We're running on either Vista, Windows 7 or Windows 8.
                    // Return the correct ProcessDpiAwareness value.
                    awareness = User32.IsProcessDPIAware() ? PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE : PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE;
                }
                catch (EntryPointNotFoundException)
                {
                }
            }

            // Return the value in awareness.
            return awareness;
        }

        /// <summary>
        /// Retrieves the dots per inch (dpi) awareness of the current process.
        /// Compatible with all Windows versions.
        /// </summary>
        /// <returns>The DPI awareness of the specified process from the <see cref="PROCESS_DPI_AWARENESS"/> enumeration.</returns>
        /// <remarks>
        /// Windows versions prior to Windows Vista do not support High DPI.
        /// This method will return <see cref="PROCESS_DPI_AWARENESS.PROCESS_DPI_UNAWARE"/> on these cases.
        /// </remarks>
        public static PROCESS_DPI_AWARENESS GetProcessDpiAwareness()
        {
            // hprocess NULL (IntPtr.Zero) means query the current process
            return GetProcessDpiAwareness(IntPtr.Zero);
        }

        public static unsafe int TryLogicalMonitorScaleFactor(IntPtr hwndMonitor)
        {
            try
            {
                return LogicalMonitorScaleFactor(hwndMonitor);
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
            }

            return 0;
        }

        public static unsafe int LogicalMonitorScaleFactor(IntPtr hwndMonitor)
        {
                var monitorInfo = User32.MONITORINFOEX.Create();
                if (User32.GetMonitorInfo(hwndMonitor, new IntPtr(&monitorInfo)))
                {
                    var logicalDesktopWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXVIRTUALSCREEN);
                    int logicalMonitorWidth = monitorInfo.Monitor.right - monitorInfo.Monitor.left;

                return (logicalMonitorWidth / logicalDesktopWidth);
                }

            return 0;
        }

        /// <summary>
        /// Overrides the scaling factor for all monitors on Windows 8.1 or higher versions
        /// </summary>
        /// <param name="scalingFactor">The desired scaling factor to override current configuration</param>
        /// <returns>Returns true if the system is overriding the scaling factor, false otherwise</returns>
        /// <remarks>Requires logoff/logon to enable the new settings</remarks>
        public static bool OverrideScalingFactor(ScaleFactors scalingFactor)
        {
            if (HasPerMonitorDpiSupport)
            {
                // If(verticalResolution < 1080) and (DPI == 125 %)
                var result = true;

                if (scalingFactor == ScaleFactors.NotAvailable)
                {
                    return false;
                }

                var value = ((int)scalingFactor / 100) * 96;

                result &= Registry.CurrentUser.SetValue<int>(HKCU_CONTROL_PANEL_DESKTOP_WIN8_DPI_SCALING, 1);
                result &= Registry.CurrentUser.SetValue<int>(HKCU_CONTROL_PANEL_DESKTOP_LOGPIXELS, value);

                return result;
            }

            return false;
        }

        public static bool RegisterAsDpiAware(bool perMonitorAwareness = true, bool fallBackToSystemAwareness = true)
        {
            if (HasPerMonitorDpiSupport)
            {
                var result = SHCore.SetProcessDpiAwareness(perMonitorAwareness ? PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE : PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE);

                if (result.Succeeded)
                {
                    return true;
                }

                if (fallBackToSystemAwareness)
                {
                    return User32.SetProcessDPIAware();
                }
            }
            else if (fallBackToSystemAwareness)
            {
                return User32.SetProcessDPIAware();
            }

            return false;
        }

        public static int ScaleFactor(this Dpi dpi)
        {
            // |--------------------|-----------------------|-----------------------|-----------------------|---------------|-------------------|
            // |    Display size    |   Display resolution  |   Horizontal (pixels) |    Vertical (pixels)  |    Panel DPI  |    Scaling level  |
            // |--------------------|-----------------------|-----------------------|-----------------------|---------------|-------------------|
            // |    10.6"           |    FHD                |            1920       |            1080       |        208    |        150%       |
            // |    10.6"           |    HD                 |            1366       |            768        |        148    |        100%       |
            // |    11.6"           |    WUXGA              |            1920       |            1200       |        195    |        150%       |
            // |    11.6"           |    HD                 |            1366       |            768        |        135    |        100%       |
            // |    13.3"           |    WUXGA              |            1920       |            1200       |        170    |        150%       |
            // |    13.3"           |    QHD                |            2560       |            1440       |        221    |        200%       |
            // |    13.3"           |    HD                 |            1366       |            768        |        118    |        100%       |
            // |    15.4"           |    FHD                |            1920       |            1080       |        143    |        125%       |
            // |    15.6"           |    QHD+               |            3200       |            1800       |        235    |        200%       |
            // |    17"             |    FHD                |            1920       |            1080       |        130    |        125%       |
            // |    23”             |    QFHD (4K)          |            3840       |            2160       |        192    |        200%       |
            // |    24"             |    QHD                |            2560       |            1440       |        122    |        125%       |
            // |--------------------|-----------------------|-----------------------|-----------------------|---------------|-------------------|
            return dpi.X * 100 / 96;
        }

        /// <summary>
        /// Sets the current process to a specified dots per inch (dpi) awareness level. The DPI awareness levels are from the <see cref="PROCESS_DPI_AWARENESS"/> enumeration.
        /// Compatible with all Windows versions.
        /// </summary>
        /// <param name="value">The DPI awareness value to set. Possible values are from the <see cref="PROCESS_DPI_AWARENESS"/> enumeration</param>
        /// <returns>true if the function succeeded on setting the DPI awareness, and false otherwise.</returns>
        /// <remarks>
        /// Windows versions prior to Windows Vista do not support High DPI, trying to set any value on these versions will result in returning false.
        /// Windows versions prior to Windows 8.1 do not support per-monitor DPI awareness. If you try to set it on these versions, the method will
        /// fall back to <see cref="PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE"/> and try to set it and return true if it succeeds.
        /// </remarks>
        public static bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS value)
        {
            try
            {
                var result = SHCore.SetProcessDpiAwareness(value);

                return result.Succeeded;
            }
            catch (DllNotFoundException)
            {
                try
                {
                    // We're running on either Vista, Windows 7 or Windows 8.
                    // Trying to set it as ShellScalingApi.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE.
                    return value == PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE && User32.SetProcessDPIAware();
                }
                catch (EntryPointNotFoundException)
                {
                }
            }
            catch (EntryPointNotFoundException)
            {
                try
                {
                    // We're running on either Vista, Windows 7 or Windows 8.
                    // Trying to set it as ShellScalingApi.PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE.
                    return value == PROCESS_DPI_AWARENESS.PROCESS_SYSTEM_DPI_AWARE && User32.SetProcessDPIAware();
                }
                catch (EntryPointNotFoundException)
                {
                }
            }

            return false;
        }
    }
}