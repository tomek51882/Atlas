using Atlas.Core;
using Atlas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Components
{
    internal class P1_2 : BaseItem, IRenderableContainer
    {
        public P1_2()
        {
            Value = "P1_2";
        }
        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(new C1_2_1());
        }
    }
}
