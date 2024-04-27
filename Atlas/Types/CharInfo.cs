using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types
{
    internal struct CharInfo
    {
        public char Char { get; set; }
        public Color FgColor { get; set; }
        public Color BgColor { get; set; }

        internal CharInfo(char c)
        {
            Char = c;
        }

        internal CharInfo(char c, Color fgColor)
        {
            Char = c;
            FgColor = fgColor;
        }

        internal CharInfo(char c, Color fgColor, Color bgColor)
        {
            Char = c;
            FgColor = fgColor;
            BgColor = bgColor;
        }
    }
}
