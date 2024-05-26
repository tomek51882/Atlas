
using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;

namespace Atlas.Types
{
    internal ref struct StylingContext
    {
        private IPrimitive? parent;
        
        public StyleProperties CurrentNodeStyles { get; set; }
        public Rect RectZero { get; } = Rect.Zero;

        public IPrimitive? Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        public StylingContext(IPrimitive parent) 
        {
            Parent = parent;
        }
    }
}
