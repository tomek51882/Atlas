using Atlas.Types;

namespace Atlas.Extensions
{
    internal static class RectExtensions
    {
        public static Rect AddPadding(this Rect rect, int padding)
        {
            var newRect = rect;
            newRect.width -= padding * 2;
            newRect.height -= padding * 2;
            if (newRect.width < 0)
            {
                newRect.width = 0;
            }
            if (newRect.height < 0)
            {
                newRect.height = 0;
            }
            return newRect;
        }
        public static Rect AddVerticalPadding(this Rect rect, int padding)
        {
            var newRect = rect;
            newRect.height -= padding * 2;
            if (newRect.height < 0)
            {
                newRect.height = 0;
            }
            return newRect;
        }
        public static Rect AddHorizontalPadding(this Rect rect, int padding)
        {
            var newRect = rect;
            newRect.width -= padding * 2;
            if (newRect.width < 0)
            {
                newRect.width = 0;
            }
            return newRect;
        }

        public static Rect RelativeToAbsolute(this Rect rect, Rect parentRect)
        {
            rect.x += parentRect.x;
            rect.y += parentRect.y;

            return rect;
        }

        public static Rect Move(this Rect rect, Vector2Int offset)
        {
            rect.x += offset.x;
            rect.y += offset.y;

            return rect;
        }
        public static Rect Move(this Rect rect, int x, int y)
        {
            rect.x += x;
            rect.y += y;

            return rect;
        }
    
    }
}
