using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;
using System.Diagnostics;

namespace Atlas.Core
{
    internal class RenderTreeBuilder
    {
        private Renderer Renderer;
        private RenderTreeNode currentNode;
        private RenderTreeNode parentNode;
        private Dictionary<IRenderable, RenderTreeNode> lookupNodes;
        private long treeGeneration = 0;

        public RenderTreeNode RenderTree { get; private set; }
        public RenderTreeBuilder(Renderer renderer)
        {
            Renderer = renderer;
            RenderTree = new RenderTreeNode(new VirtualRoot());
            RenderTree.IsInitialized = true;
            parentNode = RenderTree;
            lookupNodes = new Dictionary<IRenderable, RenderTreeNode>();
        }

        public void AddContent(IRenderable renderable)
        {
            if (lookupNodes.TryGetValue(renderable, out RenderTreeNode node))
            {
                currentNode = node;
                currentNode.IsNew = false;
            }
            else
            {
                currentNode = new RenderTreeNode(renderable);
                currentNode.IsNew = true;
                currentNode.Parent = parentNode;
                parentNode.Children.Add(currentNode);
                
                lookupNodes.Add(renderable, currentNode);
            }

            currentNode.Generation = treeGeneration;

            RenderTreeNode preRecursionParent = parentNode;

            if (renderable is IPrimitiveContainer container)
            {
                parentNode = currentNode;

                foreach (var child in container.Children)
                {
                    AddContent(child);
                }
            }
            else if (renderable is IComponent component)
            {
                parentNode = currentNode;
                if (currentNode.IsNew && currentNode.IsInitialized == false)
                {
                    currentNode.IsInitialized = true;
                    Renderer.EnqueueComponentInitialization(component);
                }
                component.BuildRenderTree(this);
            }

            parentNode = preRecursionParent;
        }
        internal void UpdateTree(List<IRenderable> children)
        {
            //if (!RenderTree.IsDirty)
            //{
            //    return;
            //}

            foreach (var component in children)
            {
                AddContent(component);
            }

            var unmountNodes = lookupNodes
                .Where(x => x.Value.Generation != treeGeneration)
                .Select(x => x.Key)
                .ToList();

            foreach (var key in unmountNodes)
            {
                Debugger.Break();
                var unmountNode = lookupNodes[key];
                unmountNode?.Parent?.Children.Remove(unmountNode);
                if (unmountNode?.Value is IComponent component)
                {
                    Renderer.EnqueueComponentDisposal(component);
                }
                lookupNodes.Remove(key);
            }
            treeGeneration++;
        }
    }

    file class VirtualRoot : IRenderable, IPrimitiveContainer
    {
        public List<IRenderable> Children { get; } = new List<IRenderable>();
    }
}
