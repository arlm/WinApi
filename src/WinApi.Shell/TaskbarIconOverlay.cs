// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace WinApi.Shell
{
    public class TaskbarIconOverlay : IDisposable
    {
        private bool isInitialized;
        private THUMBBUTTONFLAGS state;
        private Icon icon;
        private Taskbar taskbar;

        public string Description { get; set; }
        public IntPtr Parent { get; private set; }

        public event EventHandler Updated;

        public bool IsEnabled
        {
            get
            {
                return (state & THUMBBUTTONFLAGS.THBF_ENABLED) != 0;
            }

            set
            {
                if (value == IsEnabled)
                {
                    return;
                }

                if (value)
                {
                    state = state | THUMBBUTTONFLAGS.THBF_ENABLED;
                    state = state ^ THUMBBUTTONFLAGS.THBF_DISABLED;
                }
                else
                {
                    state = state ^ THUMBBUTTONFLAGS.THBF_ENABLED;
                    state = state | THUMBBUTTONFLAGS.THBF_DISABLED;
                }

                OnUpdate();
            }
        }

        public bool HasBackground
        {
            get
            {
                return (state & THUMBBUTTONFLAGS.THBF_NOBACKGROUND) == 0;
            }

            set
            {
                if (value == HasBackground)
                {
                    return;
                }

                state = value ? state ^ THUMBBUTTONFLAGS.THBF_NOBACKGROUND : state | THUMBBUTTONFLAGS.THBF_NOBACKGROUND;
                OnUpdate();
            }
        }

        public bool IsVisible
        {
            get
            {
                return (state & THUMBBUTTONFLAGS.THBF_HIDDEN) == 0;
            }

            set
            {
                if (value == IsVisible)
                {
                    return;
                }

                state = value ? state ^ THUMBBUTTONFLAGS.THBF_HIDDEN : state | THUMBBUTTONFLAGS.THBF_HIDDEN;
                OnUpdate();
            }
        }

        public Icon Icon
        {
            get
            {
                return icon;
            }

            set
            {
                icon = value;
                OnUpdate();
            }
        }

        public TaskbarIconOverlay(IntPtr parent, Icon icon, string description)
        {
            this.Description = description;
            this.state = THUMBBUTTONFLAGS.THBF_ENABLED;
            this.icon = icon;
            this.Parent = parent;
        }

        public TaskbarIconOverlay(IntPtr parent, Bitmap bitmap, string description)
        {
            this.Description = description;
            this.state = THUMBBUTTONFLAGS.THBF_ENABLED;
            this.icon = Icon.FromHandle(bitmap.GetHicon());
            this.Parent = parent;
        }

        public TaskbarIconOverlay(IntPtr parent, string message)
        {
            this.Description = message;
            this.state = THUMBBUTTONFLAGS.THBF_ENABLED;
            this.icon = BuildIcon(message);
            this.Parent = parent;
        }

        private static Icon BuildIcon(string text)
        {
            Bitmap image = null;
            IntPtr icon = IntPtr.Zero;

            try
            {
                // The source Bitmap must use PixelFormat.Format24BppRgb or PixelFormat.Format32bppArgb 
                // The source Bitmap must use at most 256 colors when using Format24BppRgb
                // The source Bitmap must be 16x16 pixels
                // The target Icon must be 16x16 pixels
                // The pixel in the lower left corner (0, 15) is used to determine the transparency color
                image = new Bitmap(16, 16, PixelFormat.Format24bppRgb);

                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.DrawString(text, new Font("Arial", 54), Brushes.White, 10, 25);
                }

                icon = image.GetHicon();
            }
            finally
            {
                image?.Dispose();
            }

            if (icon == IntPtr.Zero)
            {
                return null;
            }

            return Icon.FromHandle(icon);
        }

        public void Initialize(Taskbar taskbar)
        {
            this.taskbar = taskbar;

            taskbar.SetOverlayIcon(this.Parent, icon, Description);
            this.isInitialized = true;
        }

        protected virtual void OnUpdate()
        {
            if (!isInitialized)
            {
                return;
            }

            Updated?.Invoke(this, new EventArgs());

            if (IsEnabled && IsVisible)
            {
                taskbar.SetOverlayIcon(this.Parent, icon, Description);
            }
            else
            {
                taskbar.SetOverlayIcon(this.Parent, (Icon)null, null);
            }
        }

        public void Dispose()
        {
            taskbar = null;

            if (isInitialized)
            {
                taskbar.SetOverlayIcon(this.Parent, (Icon)null, null);
            }
        }
    }
}
