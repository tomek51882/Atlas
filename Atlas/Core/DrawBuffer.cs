using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Core
{
    internal class DrawBuffer
    {
        private CharInfo[,] buffer;
        private int width;
        private int height;
        private int cursorX;
        private int cursorY;

        private Color lastFgColor = Color.DefaultForeground;
        private Color lastBgColor = Color.DefaultBackground;
        private StringBuilder lineBuilder = new StringBuilder();

        internal DrawBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            cursorX = 0;
            cursorY = 0;
            buffer = new CharInfo[width, height];
        }

        internal void Flush()
        {
            
        }
    }
}
