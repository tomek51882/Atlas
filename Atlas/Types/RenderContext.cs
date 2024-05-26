using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;

namespace Atlas.Types
{
    internal ref struct RenderContext
    {
        private IPrimitive? parent;
        public Rect ParentAbsoluteBounds { get; set; }
        public Rect ParentRelativeBounds { get; set; }
        //public Rect CurrentAbsoluteBounds { get; set; }
        //public Rect CurrentRelativeBounds { get; set; }
        public StyleProperties CurrentStyleProperties { get; set; }
        public bool __ExperimentalInvertColors { get; set; } = false;

        public IPrimitive Parent
        {
            get
            {
                return parent;
            }
            set
            {
                ParentRelativeBounds = new Rect(0,0, value.Rect.width, value.Rect.height);
                ParentAbsoluteBounds = value.Rect.RelativeToAbsolute(ParentAbsoluteBounds);
                if (value.StyleProperties?.Padding is not null)
                {
                    ParentRelativeBounds = ParentRelativeBounds.AddPadding(value.StyleProperties.Padding.Value);
                    ParentAbsoluteBounds = ParentAbsoluteBounds
                        .AddPadding(value.StyleProperties.Padding.Value)
                        .Move(value.StyleProperties.Padding.Value, value.StyleProperties.Padding.Value);
                }
                //StyleProperties = value.StyleProperties;
                parent = value;
            }
        }
        public int ChildIndex { get; set; }

        public RenderContext(IPrimitive initialParent)
        {
            ParentRelativeBounds = initialParent.Rect;
            Parent = initialParent;
        }
    }
}
