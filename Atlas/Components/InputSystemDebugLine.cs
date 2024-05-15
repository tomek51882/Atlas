using Atlas.Core;
using Atlas.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Components
{
    internal class InputSystemDebugLine : ComponentBase
    {
        public Text text;

        public override void OnInitialized()
        {
            text = new Text();
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(text);
        }
    }
}
