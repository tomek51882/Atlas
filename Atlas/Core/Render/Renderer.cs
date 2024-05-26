using Atlas.Interfaces;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using Atlas.Types.Windows;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Atlas.Core.Render
{
    internal class Renderer : IRenderer
    {
        private RenderTreeBuilder treeBuilder;
        private List<IRenderable> mountedRenderables = new List<IRenderable>();
        private ConcurrentQueue<IComponent> initializationQueue = new ConcurrentQueue<IComponent>();
        private ConcurrentQueue<IComponent> disposeQueue = new ConcurrentQueue<IComponent>();
        private List<Task> pendingTasks = new List<Task>();

        private DisplayBuffer displayBuffer;

        public Renderer()
        {
            treeBuilder = new RenderTreeBuilder(this);
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            displayBuffer = new DisplayBuffer(width, height);
        }

        private void RenderElement(RenderContext context, RenderTreeNode node)
        {
            if (node.Value is IPrimitive renderable)
            {
                context.CurrentStyleProperties = renderable.StyleProperties;

                if (renderable is IWindowable window)
                {
                    if (!(window.Options.Frameless == true))
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
            }

            if (node.Children?.Count > 0)
            {
                if (node.Value is IPrimitive primitive)
                {
                    context.Parent = primitive;
                }

                foreach (RenderTreeNode child in node.Children)
                {
                    if (child.Value is IPrimitive childPrimitive && childPrimitive.Rect.IsInside(context.ParentRelativeBounds) == false)
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
            var borderColor = window.StyleProperties.Color?.Value ?? new Color(0xee7f00);
            displayBuffer.DrawWindow(context, window.Rect, borderColor, frameMap, $" {window.Title} ");
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

            RenderContext context = new RenderContext(treeBuilder.RenderTreeRoot);
            RenderElement(context, treeBuilder.RenderTree);
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

        public void EnqueueComponentDisposal(IComponent component)
        {
            if (component is IDisposable disposable)
            {
                disposable.Dispose();
            }
            disposeQueue.Enqueue(component);
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
