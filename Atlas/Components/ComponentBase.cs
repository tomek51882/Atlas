using Atlas.Core;
using Atlas.Interfaces.Renderables;

namespace Atlas.Components
{
    internal abstract class ComponentBase : IComponent
    {
        public ComponentBase() { }
        public virtual void BuildRenderTree(RenderTreeBuilder builder) { }
        public virtual void OnInitialized() { }
        public virtual void StateHasChanged() { }
        public virtual bool ShouldComponentRender() { return true; }
        public virtual Task OnInitializedAsync() { return Task.CompletedTask; }
        public virtual void OnKeyPressed(ConsoleKeyInfo keyInfo) { }
    }
}
