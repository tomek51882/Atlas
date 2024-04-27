using Atlas.Core;
using Atlas.Interfaces;

namespace Atlas.Components
{
    internal class C1 : BaseItem, IComponent
    {
        public C1()
        {
            Value = "C1";
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(new P1_1());
            builder.AddContent(new P1_2());
            builder.AddContent(new P1_3());
        }
    }
}
