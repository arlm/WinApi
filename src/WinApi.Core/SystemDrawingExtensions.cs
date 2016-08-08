// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Drawing;
using PInvoke;

namespace WinApi.Core
{
    public static class SystemDrawingExtensions
    {
        #region Public Methods

        public static Point ToPoint(this POINT point)
        {
            return new Point(point.x, point.y);
        }

        public static POINT ToPOINT(this Point point)
        {
            return new POINT
            {
                x = point.X,
                y = point.Y
            };
        }

        public static RECT ToRECT(this Rectangle rect)
        {
            return new RECT
            {
                top = rect.Top,
                bottom = rect.Bottom,
                left = rect.Left,
                right = rect.Right
            };
        }

        public static Rectangle ToRectangle(this PInvoke.RECT rect)
        {
            return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }

        #endregion Public Methods
    }
}