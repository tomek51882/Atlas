﻿using Atlas.Core.Render;
using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System.Runtime.CompilerServices;

namespace Atlas.Core
{
    internal class RenderTreeBuilder
    {
        private Renderer Renderer;
        private RenderTreeNode? currentNode;
        private RenderTreeNode parentNode;
        private Dictionary<IRenderable, RenderTreeNode> lookupNodes;
        private long treeGeneration = 0;

        internal RenderTreeNode RenderTree { get; private set; }
        internal IPrimitive Root => Unsafe.As<IPrimitive>(RenderTree.Value);

        public RenderTreeBuilder(Renderer renderer)
        {
            Renderer = renderer;
            RenderTree = new RenderTreeNode(new VirtualRoot());
            RenderTree.IsInitialized = true;

            parentNode = RenderTree;
            lookupNodes = new Dictionary<IRenderable, RenderTreeNode>();
        }
        public void AddContent(IRenderable? renderable)
        {
            if (renderable is null)
            {
                return;
            }

            if (lookupNodes.TryGetValue(renderable, out RenderTreeNode? node))
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

                //NOTE: May seriously hit performance
                //TODO: Enqueue task in renderer to execute in free time
                var parent = currentNode.Parent;
                while (parent is not null)
                {
                    parent.NeedsRectRecalculation = true;
                    parent = parent.Parent;
                }
                
                lookupNodes.Add(renderable, currentNode);
            }

            currentNode.Generation = treeGeneration;

            RenderTreeNode preRecursionParent = parentNode;

            if (renderable is IRenderableContainer container)
            {
                parentNode = currentNode;

                foreach (var child in container.Children)
                {
                    AddContent(child);
                }
            }
            else if (renderable is IWindowable window)
            {
                parentNode = currentNode;
                AddContent(window.Component);
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
            //Update only if something changed
            //if (RenderTree.IsDirty == false)
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
                //Debugger.Break();
                var unmountNode = lookupNodes[key];
                unmountNode?.Parent?.Children.Remove(unmountNode);
                if (unmountNode?.Value is not null)
                {
                    Renderer.EnqueueDisposal(unmountNode.Value);
                }
                lookupNodes.Remove(key);
            }
            treeGeneration++;
        }
    }

    file class VirtualRoot : IPrimitive, IRenderableContainer
    {
        public List<IRenderable> Children { get; } = new List<IRenderable>();
        public Rect Rect { get; set; }
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();

        internal VirtualRoot()
        {
            //Temp
            //Rect = new Rect(0,0,Console.BufferWidth, Console.BufferHeight);
            StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, UnitValue<int>.Unit.Percent));
            StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, UnitValue<int>.Unit.Percent));
        }

        public void AddElement(IRenderable child)
        {
            throw new NotImplementedException();
        }

        public void RemoveElement(IRenderable child)
        {
            throw new NotImplementedException();
        }
    }
}
