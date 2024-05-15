using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types
{
    internal struct Rect : IEquatable<Rect>
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public Rect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public override string ToString()
        {
            return $"X:{x} Y:{y} W:{width} H:{height}";
        }

        public bool Equals(Rect other)
        {
            return this.x == other.x
                && this.y == other.y
                && this.width == other.width
                && this.height == other.height;
        }

        public bool IsInside(Rect outerRect)
        {
            bool isInside =
            this.x >= outerRect.x &&
            this.y >= outerRect.y &&
            this.x + this.width <= outerRect.x + outerRect.width &&
            this.y + this.height <= outerRect.y + outerRect.height;

            return isInside;
        }
        public static bool operator ==(Rect left, Rect right) => left.Equals(right);

        public static bool operator !=(Rect left, Rect right) => !(left == right);
    }
}
