using Atlas.Core;
using Atlas.Interfaces.Renderables;
using Atlas.Primitives;

namespace Atlas.Components
{
    internal class TestComponent : ComponentBase
    {
        private Container container;

        //NOTE: Debug Parent Window is of size 40x10
        public override void OnInitialized()
        {
            container = new Container();
            container.Rect = new Types.Rect(0, 0, 40,10);
            container.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);

            for(int i =0; i<5 ; i++)
            {
                var t = new Text($"Text{i}");
                t.Rect = new Types.Rect(0,0, t.Rect.width, t.Rect.height);
                container.Children.Add(t);

                
            }
        }
        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if(container != null)
            {
                builder.AddContent(container);
            }
        }
    }
}
