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
                if (Options.WindowShortcut.Key != ConsoleKey.None)
                {
                    // [L]
                    // [C-R]
                    var shortcutKey = Options.WindowShortcut;
                    char keyChar = (shortcutKey.Modifiers & ConsoleModifiers.Shift) != 0 ? char.ToUpper(shortcutKey.KeyChar) : char.ToLower(shortcutKey.KeyChar);
                    string prefix = (shortcutKey.Modifiers & ConsoleModifiers.Control) != 0 ? "C-" : "";
                    return $"[{prefix}{keyChar}] {_title}";
                }
                else
                {
                    return _title;
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
        public WindowOptions Options { get; set; }
        public WindowHints WindowHints { get; set; } // Shortcuts for statusbar

        public Window(Rect rect, IComponent component, string windowId)
        {
            Rect = rect;
            AddElement(component);
            WindowId = windowId;
        }
        public Window(Rect rect, IComponent component, string windowId, WindowOptions options)
        {
            Rect = rect;
            AddElement(component);
            WindowId = windowId;
            Options = options;
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
