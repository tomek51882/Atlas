using Atlas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces
{
    internal interface IComponent : IRenderable
    {
        void BuildRenderTree(RenderTreeBuilder builder);
        void OnInitialized();
        Task OnInitializedAsync();
    }
}
