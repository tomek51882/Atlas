using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types
{
    internal struct Color
    {
        internal static readonly Color Invalid = new Color();
        internal static readonly Color Black = new Color(0, 0, 0);
        internal static readonly Color Red = new Color(255, 0, 0);
        internal static readonly Color Green = new Color(0, 255, 0);
        internal static readonly Color DefaultForeground = new Color(204, 204, 204);
        internal static readonly Color DefaultBackground = new Color(12, 12, 12);

        internal byte R;
        internal byte G;
        internal byte B;
        internal bool validColor = false;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            validColor = true;
        }
        public Color(int hexValue)
        {
            R = (byte)((hexValue >> 16) & 0xFF);
            G = (byte)((hexValue >> 8) & 0xFF);
            B = (byte)(hexValue & 0xFF);
            validColor = true;
        }
    }
}
