using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;

namespace Atlas.Components
{
    internal class Window : IPrimitive, IWindowable
    {
        public Rect Rect { get; set; }
        public bool IsFocused { get; set; }
        public string? Title { get; set; }
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();
        public IComponent? Component { get; set; }
        public string WindowId { get; }

        public Window(Rect rect, IComponent component, string windowId)
        {
            Rect = rect;
            AddElement(component);
            WindowId = windowId;
        }

        public void AddElement(IComponent child)
        {
            Component = child;
        }

        public void RemoveElement(IComponent child)
        {
            Component = default;
        }

        public void OnKeyPressed(ConsoleKeyInfo key)
        {
            Component?.OnKeyPressed(key);
        }
    }
}
