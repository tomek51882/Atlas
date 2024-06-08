
using Atlas.Interfaces.Renderables;
using Atlas.Services;
using Atlas.Types;
using Atlas.Types.Windows;

namespace Atlas.Interfaces
{
    internal interface IWindowService
    {
        (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect) where T : IComponent, new();
        (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, WindowOptions? options) where T : IComponent, new();
        void CloseFocusedWindow();
        void CloseWindow(string windowId);
    }
}
