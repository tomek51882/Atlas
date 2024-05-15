
namespace Atlas.Interfaces.Renderables
{
    internal interface IRenderableContainer
    {
        List<IRenderable> Children { get; }

        //void AddElement(IRenderable child);
        //void RemoveElement(IRenderable child);
    }
}
