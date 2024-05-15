using Atlas.Types;

namespace Atlas.Interfaces.Renderables
{
    internal interface IScrollable : IPrimitive
    {
        Vector2Int ScrollOffset { get; }
        List<IRenderable> ChildrenInRect { get; }

        void ScrollUp();
        void ScrollDown();
    }
}
