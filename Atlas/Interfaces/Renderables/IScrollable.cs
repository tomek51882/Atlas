using Atlas.Types;

namespace Atlas.Interfaces.Renderables
{
    internal interface IScrollable : IPrimitive
    {
        Vector2Int ScrollOffset { get; }

        void ScrollUp();
        void ScrollDown();
    }
}
