using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Primitives
{
    internal class RowLayout : IPrimitive, IRenderableContainer
    {
        private Rect _rect;
        private int contentLength = 0;
        private List<RowSpacer> spacers = new List<RowSpacer>();

        public virtual Rect Rect
        {
            get
            {
                return _rect;
            }
            set
            {
                _rect = value;
                RecalculateLayout();
            }
        }

        public List<IRenderable> Children { get; set; } = new List<IRenderable> ();
        public StyleProperties StyleProperties { get; set; } = new StyleProperties ();

        public void Add(IPrimitive child)
        {
            Children.Add(child);
            if (child is RowSpacer spacer)
            {
                spacers.Add(spacer);
                return;
            }
            else
            {
                contentLength += child.Rect.width;
            }
        }

        public void RecalculateLayout()
        {
            if (spacers.Count == 0 || Children is null)
            {
                return;
            }
            int usedSoFar = 0;
            int spacersUsed = 0;

            foreach (var child in Children)
            {
                if (child is RowSpacer spacer)
                {
                    int spacerWidth = Math.DivRem(Rect.width - contentLength, spacers.Count, out int lastWidth);

                    spacer.Rect = new Rect(usedSoFar + (spacerWidth * spacersUsed), 0, spacer == spacers.Last() ? spacerWidth + lastWidth : spacerWidth, 1);

                    spacersUsed++;
                    usedSoFar += spacer.Rect.width;
                    continue;
                }
                if (child is IPrimitive primitive)
                {
                    primitive.Rect = new Rect(usedSoFar, primitive.Rect.y, primitive.Rect.width, primitive.Rect.height);
                    usedSoFar += primitive.Rect.width;
                }
            }
        }

        public void Remove(IRenderable child)
        {
            throw new NotImplementedException();
        }
    }
}
