using Atlas.Core.Styles;
using Atlas.Extensions;
using Atlas.Interfaces.Renderables;

namespace Atlas.Types
{
    internal ref struct RenderContext
    {
        public Rect ParentAbsoluteBounds { get; set; }
        //public Rect ParentRelativeBounds { get; set; }
        public Rect CurrentRect { get; set; }
        //public Rect CurrentRelativeBounds { get; set; }
        public StyleProperties CurrentStyleProperties { get; set; }
        public bool __ExperimentalInvertColors { get; set; } = false;

        public IPrimitive Parent { get; set; }
        public int ChildIndex { get; set; }

        public RenderContext(IPrimitive initialParent)
        {
            //ParentRelativeBounds = initialParent.Rect;
            Parent = initialParent;
        }
    }
}
