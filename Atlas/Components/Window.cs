using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Types;

namespace Atlas.Components
{
    internal partial class Window : IPrimitive, IPrimitiveContainer, IWindowable
    {
        public Rect Rect { get; set; }
        public List<IRenderable> Children { get; set; } = new List<IRenderable>();
        public bool IsFocused { get; set; }
        public string? Title { get; set; }

        public Window(Rect rect, IRenderable renderable)
        {
            Rect = rect;
            Children.Add(renderable);
        }
    }
}
