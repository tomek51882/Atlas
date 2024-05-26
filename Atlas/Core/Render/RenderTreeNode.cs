
using Atlas.Components;
using Atlas.Interfaces.Renderables;

namespace Atlas.Core.Render
{
    internal class RenderTreeNode
    {
        public IRenderable Value { get; set; }
        public RenderTreeNode? Parent { get; set; }
        public List<RenderTreeNode> Children { get; set; }

        public bool IsNew { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsInitialized { get; set; } = false;
        public bool NeedsRectRecalculation { get; set; } = true;
        public long Generation { get; set; }

        public RenderTreeNode(IRenderable item)
        {
            Value = item;
            Children = new List<RenderTreeNode>();
        }

    }
}
