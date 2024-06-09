using Atlas.Core.Render;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System.Runtime.InteropServices;

namespace Atlas.Utils
{
    internal struct RecalculationRect
    {
        public int width;
        public int height;
        public bool isFixed;

        public RecalculationRect(int width, int height, bool isFixed)
        {
            this.width = width;
            this.height = height;
            this.isFixed = isFixed;
        }
    }
    internal enum Recalculation
    {
        Column, Row
    }
    
    internal ref struct RectRecalculationContext
    {
        private UnsafeStructArray<RecalculationRect> _rects;

        private int _fixedWidth;
        private int _fixedHeight;
        private int _percentageWidth;
        private int _percentageHeight;
        private int _adjustmentIndex;
        private int _adjustmentUsedSpace;

        public RectRecalculationContext()
        {
            _rects = new UnsafeStructArray<RecalculationRect>(4);
        }
        internal void Fill(List<RenderTreeNode> children)
        {
            //TODO: Width

            foreach (var child in children)
            {
                if (child.Value is IPrimitive primitive && primitive.StyleProperties.Height is not null)
                {
                    if (primitive.StyleProperties.Height.Value.unit == Unit.Char)
                    {
                        Add(new RecalculationRect(primitive.Rect.width, primitive.Rect.height, true));
                    }
                    else if (primitive.StyleProperties.Height.Value.unit == Unit.Percent)
                    {
                        Add(new RecalculationRect(primitive.Rect.width, primitive.Rect.height, false));
                    }
                }
                else if (child.Value is IComponent component && child.Children.Count > 0)
                {
                    Fill(child.Children);
                }
            }
        }
        internal void AdjustToRect(Rect rect)
        {
            if (_fixedHeight + _percentageHeight <= rect.height)
            {
                return;
            }
            int availableSpace = rect.height - _fixedHeight;

            double[] heights = new double[_rects.Count];
            bool[] isFixed = new bool[_rects.Count];

            for (int i = 0; i < _rects.Count; i++)
            {
                if (_rects[i].isFixed)
                {
                    heights[i] = _rects[i].height;
                    isFixed[i] = true;
                }
                else
                {
                    heights[i] = availableSpace * ((double)_rects[i].height / _percentageHeight);
                    isFixed[i] = false;
                }
            }

            int sumRounded = 0;

            for (int i = 0; i < _rects.Count; i++)
            {
                if (_rects[i].isFixed == false)
                {
                    _rects[i].height = (int)Math.Round(heights[i]);
                }

                sumRounded += _rects[i].height;
            }
            int difference = availableSpace - sumRounded + _fixedHeight;
            for (int i = 0; i < Math.Abs(difference); i++)
            {
                int index = difference > 0 ? FindMaxFractionIndex(heights, isFixed) : FindMinFractionIndex(heights, isFixed);
                if (_rects[index].isFixed)
                {
                    continue;
                }

                _rects[index].height += difference > 0 ? 1 : -1;
                heights[index] += difference > 0 ? -1 : 1;
            }
        }
        internal void ApplyAdjustment(List<RenderTreeNode> children)
        {
            foreach (var child in children)
            {
                if (child.Value is IPrimitive primitive && primitive.StyleProperties.Height is not null)
                {
                    primitive.Rect = new Rect(primitive.Rect.x, primitive.Rect.y + _adjustmentUsedSpace, 
                        primitive.Rect.width, 
                        _rects[_adjustmentIndex++].height
                    );
                    _adjustmentUsedSpace += primitive.Rect.height;
                }
                else if (child.Value is IComponent component && child.Children.Count > 0)
                {
                    ApplyAdjustment(child.Children);
                }
            }
        }
        internal void Add(RecalculationRect item)
        {
            _rects.Add(item);

            if (item.isFixed)
            {
                _fixedWidth += item.width;
                _fixedHeight += item.height;
            }
            else
            {
                _percentageWidth += item.width;
                _percentageHeight += item.height;
            }
        }
        internal void Dispose()
        {
            _rects.Dispose();
        }
        private int FindMaxFractionIndex(double[] values, bool[] ignoreArray)
        {
            int index = 0;
            double maxFraction = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (ignoreArray[i])
                {
                    continue;
                }
                double fraction = values[i] - Math.Floor(values[i]);
                if (fraction > maxFraction)
                {
                    maxFraction = fraction;
                    index = i;
                }
            }
            return index;
        }
        private int FindMinFractionIndex(double[] values, bool[] ignoreArray)
        {
            int index = 0;
            double minFraction = 1;
            for (int i = 0; i < values.Length; i++)
            {
                if (ignoreArray[i])
                {
                    continue;
                }
                double fraction = values[i] - Math.Floor(values[i]);
                if (fraction < minFraction)
                {
                    minFraction = fraction;
                    index = i;
                }
            }
            return index;
        }
    }
}
