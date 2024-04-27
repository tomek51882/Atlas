using Atlas.Core;
using Atlas.Primitives;

namespace Atlas.Components
{
    internal class TestComponent : ComponentBase
    {
        private Container container;

        public override void OnInitialized()
        {
            container = new Container();
            container.Rect = new Types.Rect(12, 12, 2, 2);
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
