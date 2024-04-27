using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;
using Atlas.Types;
using Atlas.Types.Windows;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Atlas.Core
{
    internal class Renderer : IRenderer
    {
        private RenderTreeBuilder treeBuilder;
        //private Queue<IRenderable> mountQueue = new Queue<IRenderable>();
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
                if (renderable is IWindowable window)
                {
                    RenderWindow(context, window);
                }
            }

            if (node.Children != null)
            {
                foreach (RenderTreeNode child in node.Children)
                {
                    RenderElement(context, child);
                }
            }
        }

        private void RenderWindow(RenderContext context, IWindowable window)
        {
            var frameMap = window.IsFocused ? WindowFrame.WindowFocused : WindowFrame.Window;
            displayBuffer.DrawWindowFrame(context, window.Rect, Color.Red, frameMap, $" {window.Title} ");
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

            RenderContext context = new RenderContext();
            RenderElement(context, treeBuilder.RenderTree);
            displayBuffer.Flush();

            while (disposeQueue.TryDequeue(out var component))
            {
                if (component is IAsyncDisposable asyncDisposable)
                {
                    var task = Task.Factory.StartNew((rawDisposable) =>
                    {
                        var disposable = rawDisposable as IAsyncDisposable;
                        _ = disposable.DisposeAsync();
                    }, asyncDisposable);

                    pendingTasks.Add(task);
                }
            }

            while (initializationQueue.TryDequeue(out var component))
            {
                var task = Task.Run(() =>
                {
                    _ = component.OnInitializedAsync();
                });

                pendingTasks.Add(task);
            }
        }

        public void MountRenderable(IRenderable renderable)
        {
            mountedRenderables.Add(renderable);
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
                    __Debug__RenderRenderableStructure(child, depth+1);
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
