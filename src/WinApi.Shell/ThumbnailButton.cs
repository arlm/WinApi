﻿// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using PInvoke;

namespace WinApi.Shell
{
    public class ThumbnailButton : IDisposable
    {
        private bool isInitialized;
        private static readonly IList<uint> idList = new List<uint>();
        private THUMBBUTTONMASK type;
        private THUMBBUTTONFLAGS state;
        private Icon icon;
        private Taskbar taskbar;

        public uint ID { get; set; }
        public string ToolTip { get; set; }
        public IntPtr Parent { get; private set; }

        public event EventHandler Click;
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

        public bool IsDismissOnClick
        {
            get
            {
                return (state & THUMBBUTTONFLAGS.THBF_DISMISSONCLICK) != 0;
            }

            set
            {
                if (value == IsDismissOnClick)
                {
                    return;
                }

                state = value ? state | THUMBBUTTONFLAGS.THBF_DISMISSONCLICK : state ^ THUMBBUTTONFLAGS.THBF_DISMISSONCLICK;
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

        public ThumbnailButton(IntPtr parent, string tooltip, Icon icon)
        {
            this.ID = CreateID();
            this.ToolTip = tooltip;
            this.state = THUMBBUTTONFLAGS.THBF_ENABLED;

            this.icon = icon;
            this.type = THUMBBUTTONMASK.THB_ICON | THUMBBUTTONMASK.THB_TOOLTIP | THUMBBUTTONMASK.THB_FLAGS;

            this.Parent = parent;
        }

        public void Initialize(Taskbar taskbar)
        {
            this.taskbar = taskbar;

            taskbar.AddThumbnailButtons(this.Parent, this);
            taskbar.UpdateThumbnailButtons(this.Parent, this);
            this.isInitialized = true;
        }

        internal THUMBBUTTON ToTHUMBBUTTON()
        {
            var btn = new THUMBBUTTON
            {
                dwFlags = state,
                dwMask = type,
                hIcon = icon.Handle,
                iBitmap = 0,
                iId = ID,
                szTip = ToolTip
            };

            return btn;
        }

        public void WndProcCall(ref User32.MSG msg)
        {
            switch (msg.message)
            {
                case User32.WindowMessage.WM_COMMAND:
                    if ((uint)(msg.wParam.ToInt32() & 0xFFFF) == this.ID)
                    {
                        Click?.Invoke(this, new EventArgs());
                    }
                    break;
            }
        }

        protected virtual void OnUpdate()
        {
            if (!isInitialized)
            {
                return;
            }

            Updated?.Invoke(this, new EventArgs());

            taskbar.UpdateThumbnailButtons(this.Parent, this);
        }

        public void Dispose()
        {
            taskbar = null;

            if (isInitialized)
            {
                taskbar.UpdateThumbnailButtons(this.Parent);
            }

            DeleteID(ID);
        }

        private static uint CreateID()
        {
            var rand = new Random();
            var id = (uint)rand.Next();

            while (idList.Contains(id))
            {
                id = (uint)rand.Next();
            }

            return id;
        }

        private static void DeleteID(uint id)
        {
            idList.Remove(id);
        }
    }
}
