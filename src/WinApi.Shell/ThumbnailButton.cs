// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace WinApi.Shell
{
    public class ThumbnailButton : IDisposable
    {
        private bool isInitialized;
        private static IList<uint> idList = new List<uint>();

        private THUMBBUTTONMASK type;
        private THUMBBUTTONFLAGS state;
        public uint ID { get; set; }
        private Icon icon;
        public string ToolTip { get; set; }
        public IntPtr Parent { get; private set; }

        public event EventHandler Click;
        public event EventHandler Updated;

        public bool IsEnabled
        {
            get { return (state & THUMBBUTTONFLAGS.THBF_ENABLED) != 0; }
            set
            {
                if (value == IsEnabled)
                    return;

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
            get { return (state & THUMBBUTTONFLAGS.THBF_NOBACKGROUND) == 0; }
            set
            {
                if (value == HasBackground)
                    return;

                if (value)
                    state = state ^ THUMBBUTTONFLAGS.THBF_NOBACKGROUND;
                else
                    state = state | THUMBBUTTONFLAGS.THBF_NOBACKGROUND;

                OnUpdate();
            }
        }

        public bool IsVisible
        {
            get { return (state & THUMBBUTTONFLAGS.THBF_HIDDEN) == 0; }
            set
            {
                if (value == IsVisible)
                    return;

                if (value)
                    state = state ^ THUMBBUTTONFLAGS.THBF_HIDDEN;
                else
                    state = state | THUMBBUTTONFLAGS.THBF_HIDDEN;

                OnUpdate();
            }
        }

        public bool IsDismissionClick
        {
            get { return (state & THUMBBUTTONFLAGS.THBF_DISMISSONCLICK) != 0; }
            set
            {
                if (value == IsDismissionClick)
                    return;

                if (value)
                    state = state | THUMBBUTTONFLAGS.THBF_DISMISSONCLICK;
                else
                    state = state ^ THUMBBUTTONFLAGS.THBF_DISMISSONCLICK;

                OnUpdate();
            }
        }

        public Icon Icon
        {
            get { return icon; }
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

            this.isInitialized = true;

            Taskbar.AddThumbnailButtons(this.Parent, this);
            Taskbar.UpdateThumbnailButtons(this.Parent, this);
        }

        public static implicit operator THUMBBUTTON(ThumbnailButton tb)
        {
            THUMBBUTTON btn = new THUMBBUTTON();
            btn.dwFlags = tb.state;
            btn.dwMask = tb.type;
            btn.hIcon = tb.icon.Handle;
            btn.iBitmap = 0;
            btn.iId = tb.ID;
            btn.szTip = tb.ToolTip;

            return btn;
        }

        public void WndProcCall(ref PInvoke.User32.MSG msg)
        {
            switch (msg.message)
            {
                case (PInvoke.User32.WindowMessage)0x0111:
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

            Taskbar.UpdateThumbnailButtons(this.Parent, this);
        }

        public void Dispose()
        {

        }

        private static uint CreateID()
        {
            Random rand = new Random();

            uint id = (uint)rand.Next();

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
