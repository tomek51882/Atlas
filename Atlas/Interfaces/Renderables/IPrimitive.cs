using Atlas.Core.Styles;
using Atlas.Types;

namespace Atlas.Interfaces.Renderables
{
    internal interface IPrimitive : IRenderable
    {
        Rect Rect { get; set; }
        StyleProperties StyleProperties { get; }
    }
}
