﻿using Atlas.Extensions;
using Atlas.Interfaces;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using Atlas.Types.Windows;
using Atlas.Utils;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Atlas.Core.Render
{
    internal class Renderer : IRenderer
    {
        private RenderTreeBuilder treeBuilder;
        private DisplayBuffer displayBuffer;
        private List<IRenderable> mountedRenderables = new List<IRenderable>();

        private List<Task> pendingTasks = new List<Task>();
        private ConcurrentQueue<IComponent> initializationQueue = new ConcurrentQueue<IComponent>();
        private ConcurrentQueue<IRenderable> disposeQueue = new ConcurrentQueue<IRenderable>();

        public Renderer()
        {
            treeBuilder = new RenderTreeBuilder(this);
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            displayBuffer = new DisplayBuffer(width, height);
        }

        public void Update()
        {
            var completedTasks = pendingTasks.Where(t => t.IsCompleted).ToList();

            foreach (var completedTask in completedTasks)
            {
                pendingTasks.Remove(completedTask);
                completedTask.Dispose();
            }

            treeBuilder.UpdateTree(mountedRenderables);

            StylingContext stylingContext = new StylingContext(treeBuilder.Root);
            RenderContext renderContext = new RenderContext(treeBuilder.Root);

            RecalculateRects(stylingContext, treeBuilder.RenderTree);
            RenderElement(renderContext, treeBuilder.RenderTree);

            displayBuffer.Flush();

            while (disposeQueue.TryDequeue(out var component))
            {
                if (component is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (component is IAsyncDisposable asyncDisposable)
                {
                    var task = Task.Factory.StartNew(static (rawDisposable) =>
                    {
                        var disposable = rawDisposable as IAsyncDisposable;
                        if (disposable is not null)
                        {
                            _ = disposable.DisposeAsync();
                        }

                    }, asyncDisposable);

                    pendingTasks.Add(task);
                }
            }

            while (initializationQueue.TryDequeue(out var component))
            {
                var task = Task.Factory.StartNew(static (rawComponent) =>
                {
                    var component = rawComponent as IComponent;
                    if (component is not null)
                    {
                        _ = component.OnInitializedAsync();
                    }

                }, component);

                pendingTasks.Add(task);
            }
        }

        private void RecalculateRects(StylingContext context, RenderTreeNode node)
        {
            if (node.NeedsRectRecalculation == false)
            {
                return;
            }

            if (node.Value is IPrimitive primitive)
            {
                if (context.Parent == primitive) //VirtualRoot
                {
                    primitive.Rect = new Rect(0, 0, Console.BufferWidth, Console.BufferHeight);
                    node.NeedsRectRecalculation = false;
                }


                context.CurrentNodeStyles = primitive.StyleProperties;
                if (primitive.Rect == context.RectZero)
                {
                    int calculatedWidth = 0;
                    int calculatedHeight = 0;
                    Rect finalRect = new Rect(primitive.Rect.x, primitive.Rect.y, 0,0);

                    if (context.CurrentNodeStyles.Width is not null)
                    {
                        calculatedWidth = context.CurrentNodeStyles.Width.Value.unit switch
                        {
                            UnitValue<int>.Unit.Char => context.CurrentNodeStyles.Width.Value.value,
                            UnitValue<int>.Unit.Percent => context.CurrentNodeStyles.Width.Value.value / 100 * context.Parent.Rect.width,
                            _ => 0
                        };
                    }

                    if (context.CurrentNodeStyles.Height is not null)
                    {
                        calculatedHeight = context.CurrentNodeStyles.Height.Value.unit switch
                        {
                            UnitValue<int>.Unit.Char => context.CurrentNodeStyles.Height.Value.value,
                            UnitValue<int>.Unit.Percent => context.CurrentNodeStyles.Height.Value.value / 100 * context.Parent.Rect.height,
                            _ => 0
                        };
                    }

                    finalRect.width = calculatedWidth;
                    finalRect.height = calculatedHeight;

                    if (context.Parent.StyleProperties.Padding is not null)
                    {
                        finalRect = finalRect
                            .AddPadding(context.Parent.StyleProperties.Padding.Value)
                            .Move(context.Parent.StyleProperties.Padding.Value, context.Parent.StyleProperties.Padding.Value);
                    }

                    primitive.Rect = finalRect;
                }

                context.Parent = primitive;
            }

            if (node.Children?.Count > 0)
            {
                foreach (RenderTreeNode child in node.Children)
                {
                    RecalculateRects(context, child);
                }

                //Part 2 of rect calculation aka autolayout
                if (node.Value is IPrimitive primitive2)
                {
                    if (primitive2.StyleProperties.AutoLayoutDirection?.Value == AutoLayoutDirection.Column)
                    {

                    }
                    else if (primitive2.StyleProperties.AutoLayoutDirection?.Value == AutoLayoutDirection.Row)
                    {

                    }
                }
            }
            node.NeedsRectRecalculation = false;
        }

        private void RenderElement(RenderContext context, RenderTreeNode node)
        {
            if (node.Value is IPrimitive renderable)
            {
                context.CurrentRect = renderable.Rect.RelativeToAbsolute(context.ParentAbsoluteBounds);
                context.CurrentStyleProperties = renderable.StyleProperties;

                if (renderable is IWindowable window)
                {
                    if (window.Options.Frameless != true)
                    {
                        RenderWindow(context, window);
                    }
                }
                if (renderable is IPrimitiveText text)
                {
                    RenderText(context, text);
                }
                if (renderable is IPrimitiveFiller filler)
                {
                    FillArea(context, filler);
                }
                if (renderable is ISelectableItem item && Unsafe.As<ISelectable>(context.Parent).SelectedItem == item)
                {
                    context.__ExperimentalInvertColors = true;
                }
                if (renderable.StyleProperties.Border is not null && renderable.StyleProperties.Border.Value)
                {
                    displayBuffer.DrawWindow(context, renderable.StyleProperties.Color?.Value ?? Color.Red, WindowFrame.Window, null);
                }

                context.Parent = renderable;
                context.ParentAbsoluteBounds = context.CurrentRect;
            }

            if (node.Children?.Count > 0)
            {
                foreach (RenderTreeNode child in node.Children)
                {
                    if (child.Value is IPrimitive childPrimitive && childPrimitive.Rect.IsInside(context.ParentAbsoluteBounds) == false)
                    {
                        continue;
                    }
                    RenderElement(context, child);
                }
            }
        }

        private void FillArea(RenderContext context, IPrimitiveFiller filler)
        {
            displayBuffer.FillEmptyArea(context, filler.Rect, filler.StyleProperties.BackgroundColor?.Value ?? Color.DefaultBackground);
        }

        private void RenderText(RenderContext context, IPrimitiveText text)
        {
            displayBuffer.WriteText(context, text.Rect, text.Value, text.StyleProperties.Color?.Value ?? Color.DefaultForeground, text.StyleProperties.BackgroundColor?.Value ?? Color.DefaultBackground);
        }

        private void RenderWindow(RenderContext context, IWindowable window)
        {
            var frameMap = window.IsFocused ? WindowFrame.WindowFocused : WindowFrame.Window;
            var borderColor = window.StyleProperties.Color?.Value ?? new Color(0xff00ff);
            if (window.IsFocused == false)
            {
                var hslColor = ColorUtils.RGB2HSL(borderColor.R, borderColor.G, borderColor.B);
                hslColor.l /= 2;
                if (hslColor.h == 1)
                {
                    hslColor.h = 0;
                }
                var darkenedBorderColor = ColorUtils.HSL2RGB(hslColor.h, hslColor.s, hslColor.l);
                borderColor = new Color(darkenedBorderColor.r, darkenedBorderColor.g, darkenedBorderColor.b);
            }
            displayBuffer.DrawWindow(context, borderColor, frameMap, $" {window.Title} ");
        }

        public void MountRenderable(IRenderable renderable)
        {
            mountedRenderables.Add(renderable);
        }
        public void UnmountRenderable(IRenderable renderable)
        {
            mountedRenderables.Remove(renderable);
        }
        public void EnqueueComponentInitialization(IComponent component)
        {
            component.OnInitialized();
            initializationQueue.Enqueue(component);
        }
        public void EnqueueDisposal(IRenderable renderable)
        {
            if (renderable is IDisposable disposable)
            {
                disposable.Dispose();
            }
            disposeQueue.Enqueue(renderable);
        }
        private void __Debug__RenderRenderableStructure(RenderTreeNode node, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                Console.Write(" ");
            }

            if (node.Value is IComponent)
            {
                Console.WriteLine("C");
            }

            if (node.Value is IPrimitive)
            {
                Console.WriteLine("P");
            }

            if (node.Children != null)
            {
                foreach (RenderTreeNode child in node.Children)
                {
                    __Debug__RenderRenderableStructure(child, depth + 1);
                }
            }
        }
        private void __Debug__RenderPrimitiveStructure(RenderTreeNode node, int depth)
        {
            if (node.Value is IPrimitive renderable)
            {
                for (int i = 0; i < depth; i++)
                {
                    Console.Write(" ");
                }

                if (node.Value is IComponent)
                {
                    Console.WriteLine("C");
                }

                if (node.Value is IPrimitive)
                {
                    Console.WriteLine("P");
                }
                depth++;
            }

            if (node.Children != null)
            {
                foreach (RenderTreeNode child in node.Children)
                {
                    __Debug__RenderPrimitiveStructure(child, depth);
                }
            }
        }
    }
}
