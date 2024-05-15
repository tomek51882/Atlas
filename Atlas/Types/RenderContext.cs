using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;

namespace Atlas.Types
{
    internal ref struct RenderContext
    {
        private IPrimitive? parent;
        public Rect AbsoluteBounds { get; set; }
        public Rect RelativeBounds { get; set; }
        //public StyleProperties StyleProperties { get; set; } = new StyleProperties();

        public bool __ExperimentalInvertColors { get; set; } = false;

        public IPrimitive Parent
        {
            get
            {
                return parent;
            }
            set
            {
                RelativeBounds = new Rect(0,0, value.Rect.width, value.Rect.height);
                AbsoluteBounds = value.Rect.RelativeToAbsolute(AbsoluteBounds);
                if (value.StyleProperties?.Padding is not null)
                {
                    RelativeBounds = RelativeBounds.AddPadding(value.StyleProperties.Padding.Value);
                    AbsoluteBounds = AbsoluteBounds
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
            RelativeBounds = initialParent.Rect;
            Parent = initialParent;
        }
    }
}
