using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;
namespace Atlas.Core
{
    internal class Renderer : IRenderer
    {
        private int loopIteration = 0;
        private RenderTreeBuilder treeBuilder;
        
        //private ConcurrentQueue<IRenderable> renderQueue = new ConcurrentQueue<IRenderable>();
        //private ConcurrentQueue<IRenderable> disposeQueue = new ConcurrentQueue<IRenderable>();
        //private ConcurrentQueue<IRenderable> readyToRender = new ConcurrentQueue<IRenderable>();

        //private ConcurrentDictionary<>
        private List<Task> pendingTasks = new List<Task>();
        //private Dictionary
        private RenderTreeNode node;
        private BaseItem rootItem;
        public Renderer()
        { 
            treeBuilder = new RenderTreeBuilder(this);
            
            node = new RenderTreeNode()
            {
                Value = new Item("Root"),
                Children = new List<RenderTreeNode>()
                {
                    new RenderTreeNode()
                    {
                        Value = new C1(),
                        Children = new List<RenderTreeNode>
                        {
                            new RenderTreeNode(){ Value = new P1_1() },
                            new RenderTreeNode(){ 
                                Value = new P1_2(),
                                Children = new List<RenderTreeNode>
                                { 
                                    new RenderTreeNode() {
                                        Value=new C1_2_1(),
                                        Children = new List<RenderTreeNode>
                                        {
                                            new RenderTreeNode(){ Value = new P1_2_1_1()}
                                        }
                                    }
                                }
                                
                            },
                            new RenderTreeNode(){ Value = new P1_3() }
                        }
                    },
                    new RenderTreeNode() { Value = new P2() },
                    new RenderTreeNode() { Value = new P3() }
                }
            };
            
        }

        public void RenderElement(RenderTreeNode? node, int depth)
        {
            if (node is null)
            {
                return;
            }
            if (node.Value is not IComponent)
            {
                for (int i = 0; i < depth; i++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine(node.Value.Value);
                if (node.Children is not null)
                {
                    depth++;
                }
            }

            //for (int i = 0; i < depth; i++)
            //{
            //    Console.Write(" ");
            //}
            //Console.WriteLine(node.Value.Value);


            if (node.Children != null)
            {
                foreach(RenderTreeNode child in node.Children)
                {
                    RenderElement(child, depth);
                }
            }
        }

        public void Update()
        {

            Console.WriteLine(loopIteration);
            RenderElement(node, 0);
            Console.WriteLine("==============");

            treeBuilder.AddContent(new C1());
            treeBuilder.AddContent(new P2());
            treeBuilder.AddContent(new P3());

            RenderElement(treeBuilder.tree, 0);
            loopIteration++;
            //BOXING

            //var completedTasks = pendingTasks.Where(t => t.IsCompleted).ToList();
            //foreach (var completedTask in completedTasks)
            //{
            //    pendingTasks.Remove(completedTask);
            //    completedTask.Dispose();
            //}

            //while (readyToRender.TryDequeue(out var item))
            //{
            //    item.BuildRenderTree(treeBuilder);
            //}

            //while (renderQueue.TryDequeue(out var item))
            //{
            //    var task = Task.Run(async () =>
            //    {
            //        item.OnInitialized();
            //        //_ = item.OnInitializedAsync();
            //        readyToRender.Enqueue(item);
            //    });

            //    pendingTasks.Add(task);
            //}

        }
    }
}
