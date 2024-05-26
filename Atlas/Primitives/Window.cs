using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using Atlas.Types.Windows;

namespace Atlas.Primitives
{
    internal class Window : IPrimitive, IWindowable
    {
        private string? _title;
        public Rect Rect { get; set; }
        public bool IsFocused { get; set; }
        public string? Title {
            get 
            {
                if (WindowShortcut.Key == ConsoleKey.None)
                {
                    return _title;
                }
                else
                {
                    // [L]
                    // [C-R]
                    char keyChar = (WindowShortcut.Modifiers & ConsoleModifiers.Shift) != 0 ? char.ToUpper(WindowShortcut.KeyChar) : char.ToLower(WindowShortcut.KeyChar);
                    string prefix = (WindowShortcut.Modifiers & ConsoleModifiers.Control) != 0 ? "C-" : "";
                    return $"[{prefix}{keyChar}] {_title}";
                }
            }
            set 
            {
                _title = value;
            }

        }
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();
        public IComponent? Component { get; set; }
        public string WindowId { get; }
        //public WindowShortcut WindowShortcut { get; init; }
        public ConsoleKeyInfo WindowShortcut { get; init; }

        public Window(Rect rect, IComponent component, string windowId, ConsoleKeyInfo shortcut)
        {
            Rect = rect;
            AddElement(component);
            WindowId = windowId;
            WindowShortcut = shortcut;
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
            //key.
            Component?.OnKeyPressed(key);
        }
    }
}
