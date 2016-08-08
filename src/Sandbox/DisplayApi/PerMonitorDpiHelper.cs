// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using WinApi.Core;

namespace Sandbox
{
    /// <summary>
    /// Handles all the Per-Monitor-DPI related tasks for a window.
    /// </summary>
    public class PerMonitorDpiHelper
    {
        // Attached property implementation
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PerMonitorDpiHelper), new PropertyMetadata(false, SetEnabled));

        private static readonly DependencyProperty PerMonitorDpiHelperProperty =
            DependencyProperty.RegisterAttached("PerMonitorDpiHelper", typeof(PerMonitorDpiHelper), typeof(PerMonitorDpiHelper));

        private Dpi currentDpi;

        private HwndSource hwndSource;

        private Dpi systemDpi;

        private Window window;

        static PerMonitorDpiHelper()
        {
            DpiAwareApi.RegisterAsDpiAware();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerMonitorDpiHelper"/> class. It handles
        /// Per-Monitor-DPI configurations on a window.
        /// </summary>
        /// <param name="window">Target window.</param>
        public PerMonitorDpiHelper(Window window)
        {
            this.window = window;
            this.window.Loaded += this.Window_Loaded;
        }

        public static bool GetIsEnabled(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject dp, bool value)
        {
            dp.SetValue(IsEnabledProperty, value);
        }

        public void RemoveHook()
        {
            if (this.hwndSource != null)
            {
                this.hwndSource.RemoveHook(this.WndProc);
                this.hwndSource = null;
            }

            if (this.window != null)
            {
                this.window.Loaded -= this.Window_Loaded;
                this.window.Closed -= this.Window_Closed;
                this.window = null;
            }
        }

        private static PerMonitorDpiHelper GetPerMonitorDpiHelper(DependencyObject dp)
        {
            return (PerMonitorDpiHelper)dp.GetValue(PerMonitorDpiHelperProperty);
        }

        private static void SetEnabled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
            {
                return;
            }

            if ((bool)e.OldValue)
            {
                GetPerMonitorDpiHelper(window).RemoveHook();
            }

            if ((bool)e.NewValue)
            {
                SetPerMonitorDpiHelper(window, new PerMonitorDpiHelper(window));
            }
        }

        private static void SetPerMonitorDpiHelper(DependencyObject dp, PerMonitorDpiHelper value)
        {
            dp.SetValue(PerMonitorDpiHelperProperty, value);
        }

        private void ChangeDpi(Dpi dpi)
        {
            if (!DpiAwareApi.HasPerMonitorDpiSupport)
            {
                return;
            }

            var elem = this.window.Content as FrameworkElement;
            if (elem != null)
            {
                elem.LayoutTransform = (dpi == this.systemDpi)
                    ? Transform.Identity
                    : new ScaleTransform((double)dpi.X / this.systemDpi.X, (double)dpi.Y / this.systemDpi.Y);
            }

            this.window.Width = this.window.Width * dpi.X / this.currentDpi.X;
            this.window.Height = this.window.Height * dpi.Y / this.currentDpi.Y;

            Debug.WriteLine(string.Format("DPI Change: {0} -> {1} (System: {2})", this.currentDpi, dpi, this.systemDpi));

            this.currentDpi = dpi;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.RemoveHook();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.systemDpi = this.window.GetSystemDpi();
            this.hwndSource = PresentationSource.FromVisual(this.window) as HwndSource;
            if (this.hwndSource != null)
            {
                this.currentDpi = this.hwndSource.GetDpi();
                this.ChangeDpi(this.currentDpi);
                this.hwndSource.AddHook(this.WndProc);
                this.window.Closed += this.Window_Closed;
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)PInvoke.User32.WindowMessage.WM_DPICHANGED)
            {
                var dpiX = CoreExtensions.GetHiWord(wParam);
                var dpiY = CoreExtensions.GetLoWord(wParam);
                this.ChangeDpi(new Dpi(dpiX, dpiY));
                handled = true;
            }

            return IntPtr.Zero;
        }
    }
}