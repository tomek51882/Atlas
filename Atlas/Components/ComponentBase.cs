using Atlas.Core;
using Atlas.Interfaces;

namespace Atlas.Components
{
    internal abstract class ComponentBase : IComponent
    {
        public virtual void BuildRenderTree(RenderTreeBuilder builder)
        {

        }

        public virtual void OnInitialized()
        {

        }

        public virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
