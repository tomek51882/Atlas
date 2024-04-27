using Atlas.Core;
using Atlas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Components
{
    internal class C1_2_1 : BaseItem, IComponent
    {
        public C1_2_1() {
            Value = "C1_2_1";
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(new P1_2_1_1());
        }
    }
}
