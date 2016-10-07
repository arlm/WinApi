// Copyright (c) to owners found in https://github.com/arlm/WinApi/blob/master/COPYRIGHT.md. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace WinApi.HighDpi
{
    public struct Dpi
    {
        public static readonly Dpi Default = new Dpi(96, 96);

        public Dpi(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public static bool operator !=(Dpi dpi1, Dpi dpi2)
        {
            return !(dpi1 == dpi2);
        }

        public static bool operator ==(Dpi dpi1, Dpi dpi2)
        {
            return dpi1.X == dpi2.X && dpi1.Y == dpi2.Y;
        }

        public bool Equals(Dpi other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return false;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return obj is Dpi && this.Equals((Dpi)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return this.X * 397 ^ this.Y;
            }
        }

        public override string ToString()
        {
            return string.Format("[X={0},Y={1}]", this.X, this.Y);
        }
    }
}