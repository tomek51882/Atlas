
using Atlas.Components;
using Atlas.Interfaces;

namespace Atlas.Core.Render
{
    internal class RenderTreeNode
    {
        public BaseItem Value { get; set; }
        public RenderTreeNode? Parent { get; set; }
        public List<RenderTreeNode> Children { get; set; }

        public RenderTreeNode()
        {
            Children = new List<RenderTreeNode>();
        }
        public RenderTreeNode(BaseItem item)
        {
            Value = item;
            Children = new List<RenderTreeNode>();
        }

        public RenderTreeNode AddChildren(RenderTreeNode child)
        {
            
            Children.Add(child);
            return this;
        }

    }
}
