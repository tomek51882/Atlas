using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;

namespace Atlas.Core
{
    internal class RenderTreeBuilder
    {
        private Renderer Renderer;
        private RenderTreeNode currentNode;
        private RenderTreeNode parentNode;

        public RenderTreeNode tree;
        public RenderTreeBuilder(Renderer renderer)
        {
            Renderer = renderer;
            tree = new RenderTreeNode(new Root());
            parentNode = tree;
        }

        public void AddContent(BaseItem item)
        {
            currentNode = new RenderTreeNode(item);
            currentNode.Parent = parentNode;

            if (parentNode is not null)
            { 
                parentNode.AddChildren(currentNode);
            }

            RenderTreeNode? prevParent = null;

            if(item is IComponent || item is IRenderableContainer)
            {
                prevParent = parentNode;
                parentNode = currentNode;
            }

            item.BuildRenderTree(this);
            if (prevParent is not null)
            {
                parentNode = prevParent;
            }
        }
    }
}
