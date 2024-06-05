using Atlas.Extensions;
using Atlas.Types;
using Atlas.Types.Windows;
using System.Text;

namespace Atlas.Core.Render
{
    internal class DisplayBuffer
    {
        private CharInfo[,] displayBuffer;
        private int[] zIndexMap;

        private int width;
        private int height;
        private int cursorX;
        private int cursorY;

        private Color lastFgColor = Color.DefaultForeground;
        private Color lastBgColor = Color.DefaultBackground;
        private StringBuilder stringBuilder = new StringBuilder();

        //private Memory<CharInfo> buffer;

        internal DisplayBuffer(int width, int height)
        {
            this.width = width;
            this.height = height;
            cursorX = 0;
            cursorY = 0;
            displayBuffer = new CharInfo[width, height];
            zIndexMap = new int[width * height];
            Console.Clear();
            Console.SetCursorPosition(cursorX, cursorY);
            //buffer = new Memory<CharInfo>();
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
        }

        internal void SetCursor(int cursorX, int cursorY)
        {
            this.cursorX = cursorX;
            this.cursorY = cursorY;
        }

        internal void DrawBox(RenderContext context, Rect rect)
        {
            SetCursor(rect.x, rect.y);
            for (int y = 0; y < rect.height; y++)
            {
                for (int x = 0; x < rect.width; x++)
                {
                    var temp = new Vector2Int(rect.x + x, rect.y + y);
                    Write(context, '╳');
                }
                SetCursor(rect.x, rect.y + y + 1);
            }
        }
        internal void Write(RenderContext context, char input)
        {
            Write(context, input, Color.DefaultForeground, Color.DefaultBackground);
        }
        internal void Write(RenderContext context, char input, Color fgColor)
        {
            Write(context, input, fgColor, Color.DefaultBackground);
        }
        internal void Write(RenderContext context, char input, Color fgColor, Color bgColor)
        {
            if (cursorX >= width || cursorX < 0 || cursorY >= height || cursorY < 0)
            {
                return;
            }
            if (displayBuffer[cursorX, cursorY].Char == '\0' || HasHigherZIndex(context))
            {
                displayBuffer[cursorX, cursorY] = new CharInfo(input, fgColor, bgColor);
            }
            cursorX++;
        }

        internal void WriteText(RenderContext context, Rect bounds, string input, Color fgColor, Color bgColor)
        {
            if (input is null)
            {
                return;
            }
            bounds = bounds.RelativeToAbsolute(context.ParentAbsoluteBounds);

            SetCursor(bounds.x, bounds.y);
            for (int i = 0; i < input.Length; i++)
            {
                if (cursorX >= bounds.x + bounds.width || cursorX >= context.ParentAbsoluteBounds.x + context.ParentAbsoluteBounds.width)
                {
                    return;
                }

                if (displayBuffer[cursorX, cursorY].Char == '\0' || HasHigherZIndex(context))
                {
                    if (context.__ExperimentalInvertColors)
                    {
                        displayBuffer[cursorX, cursorY] = new CharInfo(input[i], bgColor, fgColor);
                    }
                    else
                    {
                        displayBuffer[cursorX, cursorY] = new CharInfo(input[i], fgColor, bgColor);
                    }
                }
                cursorX++;

            }
        }

        internal void ForceWriteText(RenderContext context, Rect bounds, string input, Color fgColor, Color bgColor)
        {
            //int freeSpace = bounds.width = input.Length;
            bounds = bounds.RelativeToAbsolute(context.ParentAbsoluteBounds);
            SetCursor(bounds.x, bounds.y);
            for (int i = 0; i < input.Length; i++)
            {
                if (cursorX >= bounds.x + bounds.width || cursorX >= context.ParentAbsoluteBounds.x + context.ParentAbsoluteBounds.width)
                {
                    return;
                }

                //if (IsZIndexHigher(context) == false)
                //{
                //    return;
                //}
                displayBuffer[cursorX, cursorY] = new CharInfo(input[i], fgColor, bgColor);
                cursorX++;
            }
        }
        internal void FillArea(RenderContext context, Rect bounds, Color fillColor)
        {

        }
        internal void FillEmptyArea(RenderContext context, Rect bounds, Color fillColor)
        {
            bounds = bounds.RelativeToAbsolute(context.ParentAbsoluteBounds);
            for (int y = bounds.y; y < bounds.y + bounds.height; y++)
            {
                if (y < 0)
                {
                    continue;
                }
                if (y > height)
                {
                    break;
                }

                for (int x = bounds.x; x < bounds.x + bounds.width; x++)
                {
                    if (displayBuffer[x, y].Char == '\0' || HasHigherZIndex(context))
                    {
                        displayBuffer[x, y].Char = ' ';
                        if (context.__ExperimentalInvertColors)
                        {

                            displayBuffer[x, y].FgColor = fillColor;
                            displayBuffer[x, y].BgColor = Color.DefaultForeground;
                        }
                        else
                        {
                            displayBuffer[x, y].FgColor = Color.DefaultForeground;
                            displayBuffer[x, y].BgColor = fillColor;
                        }
                    }
                }
            }
        }
        internal void DrawWindow(RenderContext context, Color borderColor, WindowFrame frameMap, string? windowTitle)
        {
            //bounds = bounds.RelativeToAbsolute(context.ParentAbsoluteBounds);
            SetCursor(context.CurrentRect.x, context.CurrentRect.y);
            Write(context,frameMap.fragmentMap[WindowFrameFragment.TopLeft], borderColor);

            for (int i = 1; i < context.CurrentRect.width - 1; i++)
            {
                Write(context, frameMap.fragmentMap[WindowFrameFragment.Horizontal], borderColor);
            }
            Write(context, frameMap.fragmentMap[WindowFrameFragment.TopRight], borderColor);

            for (int j = 1; j < context.CurrentRect.height - 1; j++)
            {
                SetCursor(context.CurrentRect.x, context.CurrentRect.y + j);
                Write(context, frameMap.fragmentMap[WindowFrameFragment.Vertical], borderColor);
                SetCursor(context.CurrentRect.x + context.CurrentRect.width - 1, context.CurrentRect.y + j);
                Write(context, frameMap.fragmentMap[WindowFrameFragment.Vertical], borderColor);
            }

            SetCursor(context.CurrentRect.x, context.CurrentRect.y + context.CurrentRect.height - 1);
            Write(context, frameMap.fragmentMap[WindowFrameFragment.BottomLeft], borderColor);
            for (int i = 1; i < context.CurrentRect.width - 1; i++)
            {
                Write(context, frameMap.fragmentMap[WindowFrameFragment.Horizontal], borderColor);
            }

            Write(context, frameMap.fragmentMap[WindowFrameFragment.BottomRight], borderColor);

            if (windowTitle is not null)
            {
                Rect windowTitleArea = new Rect(context.CurrentRect.x + 2, context.CurrentRect.y, context.CurrentRect.width - 3, 1);
                ForceWriteText(context, windowTitleArea, windowTitle, borderColor, Color.DefaultBackground);
            }
        }

        private int Translate(int x, int y)
        {
            return y * width + x;
        }
        private (int x, int y) Translate(int idx)
        {
            return (idx % width, idx / width);
        }

        private bool HasHigherZIndex(RenderContext context)
        {
            int currentZIndex = context.CurrentStyleProperties?.ZIndex?.Value ?? 0;
            int idx = Translate(cursorX, cursorY);
            if (currentZIndex >= zIndexMap[idx])
            {
                return true;
            }
            return false;
        }
    }
}
