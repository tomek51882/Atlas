using Atlas.Components;
using Atlas.Types;
using Atlas.Types.Windows;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using System.Text;

namespace Atlas.Core.Render
{
    internal class DisplayBuffer
    {
        private CharInfo[,] displayBuffer;
        private int width;
        private int height;
        private int cursorX;
        private int cursorY;

        private Color lastFgColor = Color.DefaultForeground;
        private Color lastBgColor = Color.DefaultBackground;
        private StringBuilder stringBuilder = new StringBuilder();

        internal DisplayBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            cursorX = 0;
            cursorY = 0;
            displayBuffer = new CharInfo[width, height];
        }

        internal void Flush()
        {

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var charInfo = displayBuffer[x, y];
                    charInfo = charInfo.Char == '\0' ? new CharInfo(' ', Color.DefaultForeground, Color.DefaultBackground) : charInfo;

                    // Foreground color change
                    if (charInfo.FgColor.R != lastFgColor.R || charInfo.FgColor.G != lastFgColor.G || charInfo.FgColor.B != lastFgColor.B)
                    {
                        stringBuilder.Append($"\x1b[38;2;{charInfo.FgColor.R};{charInfo.FgColor.G};{charInfo.FgColor.B}m");
                        lastFgColor = charInfo.FgColor;
                    }

                    // Background color change
                    if (charInfo.BgColor.R != lastBgColor.R || charInfo.BgColor.G != lastBgColor.G || charInfo.BgColor.B != lastBgColor.B)
                    {
                        stringBuilder.Append($"\x1b[48;2;{charInfo.BgColor.R};{charInfo.BgColor.G};{charInfo.BgColor.B}m");
                        lastBgColor = charInfo.BgColor;
                    }

                    stringBuilder.Append(charInfo.Char);
                }

                // Reset color at the end of each line to default
                stringBuilder.Append("\x1b[0m");

                // Avoid adding a new line on the last row
                if (y < height - 1)
                {
                    stringBuilder.AppendLine();
                }
            }
            Console.Write(stringBuilder.ToString());
            stringBuilder.Clear();
            ClearBuffer();
        }

        private void ClearBuffer()
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    displayBuffer[x, y].Char = '\0';
            SetCursor(0, 0);
            lastFgColor = Color.DefaultForeground;
            lastBgColor = Color.DefaultBackground;
            Console.SetCursorPosition(0, 0);
            //Console.
        }

        internal void SetCursor(int cursorX, int cursorY)
        {
            this.cursorX = cursorX;
            this.cursorY = cursorY;
        }

        internal void DrawBox(Rect rect)
        {
            SetCursor(rect.x, rect.y);
            for (int y = 0; y < rect.height; y++)
            {
                for (int x = 0; x < rect.width; x++)
                {
                    var temp = new Vector2Int(rect.x + x, rect.y + y);
                    Write('╳');
                }
                SetCursor(rect.x, rect.y + y + 1);
            }
        }
        internal void Write(char input)
        {
            Write(input, Color.DefaultForeground, Color.DefaultBackground);
        }
        internal void Write(char input, Color fgColor)
        {
            Write(input, fgColor, Color.DefaultBackground);
        }
        internal void Write(char input, Color fgColor, Color bgColor)
        {
            if (cursorX >= width || cursorX < 0 || cursorY >= height || cursorY < 0)
            {
                return;
            }
            if (displayBuffer[cursorX, cursorY].Char == '\0')
            {
                displayBuffer[cursorX, cursorY] = new CharInfo(input, fgColor, bgColor);
            }
            cursorX++;
        }
        internal void WriteText(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (cursorX >= width)
                {
                    return;
                }
                if (displayBuffer[cursorX, cursorY].Char == '\0')
                {
                    displayBuffer[cursorX, cursorY] = new CharInfo(input[i], Color.DefaultForeground, Color.DefaultBackground);
                }
                cursorX++;
            }
        }

        internal void WriteText(RenderContext context, Rect bounds, string input, Color fgColor, Color bgColor)
        {
            
        }

        internal void ForceWriteText(RenderContext context, Rect bounds, string input, Color fgColor, Color bgColor)
        {
            //int freeSpace = bounds.width = input.Length;
            SetCursor(bounds.x, bounds.y);
            for (int i = 0; i < input.Length; i++)
            {
                if (cursorX >= bounds.width)
                {
                    return;
                }
                displayBuffer[cursorX, cursorY] = new CharInfo(input[i], fgColor, bgColor);
                cursorX++;
            }
        }
        internal void FillArea(RenderContext context, Rect bounds, Color fillColor)
        {
            
        }
        internal void FillEmptyArea(RenderContext context, Rect bounds, Color fillColor)
        {

        }
        internal void DrawWindowFrame(RenderContext context, Rect bounds, Color borderColor, WindowFrame frameMap, string? windowTitle)
        {
            SetCursor(bounds.x, bounds.y);
            Write(frameMap.fragmentMap[WindowFrameFragment.TopLeft], borderColor);

            for (int i = 1; i < bounds.width - 1; i++)
            {
                Write(frameMap.fragmentMap[WindowFrameFragment.Horizontal], borderColor);
            }
            Write(frameMap.fragmentMap[WindowFrameFragment.TopRight], borderColor);

            for (int j = 1; j < bounds.height - 1; j++)
            {
                SetCursor(bounds.x, bounds.y + j);
                Write(frameMap.fragmentMap[WindowFrameFragment.Vertical], borderColor);
                SetCursor(bounds.x + bounds.width - 1, bounds.y + j);
                Write(frameMap.fragmentMap[WindowFrameFragment.Vertical], borderColor);
            }

            SetCursor(bounds.x, bounds.y + bounds.height - 1);
            Write(frameMap.fragmentMap[WindowFrameFragment.BottomLeft], borderColor);
            for (int i = 1; i < bounds.width - 1; i++)
            {
                Write(frameMap.fragmentMap[WindowFrameFragment.Horizontal], borderColor);
            }

            Write(frameMap.fragmentMap[WindowFrameFragment.BottomRight], borderColor);

            if (windowTitle is not null)
            {
                Rect windowTitleArea = new Rect(bounds.x+2, bounds.y, bounds.width-3, 1);
                ForceWriteText(context, windowTitleArea, windowTitle, borderColor, Color.DefaultBackground);
            }
        }
    }
}
