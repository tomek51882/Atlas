using Atlas.Core;

namespace Atlas.Interfaces.Renderables
{
    internal interface IComponent : IRenderable
    {
        
        void BuildRenderTree(RenderTreeBuilder builder);
        void OnInitialized();
        Task OnInitializedAsync();
        void OnKeyPressed(ConsoleKeyInfo keyInfo);
    }
}
