
using Atlas.Interfaces.Renderables;
using Atlas.Services;
using Atlas.Types;

namespace Atlas.Interfaces
{
    internal interface IWindowService
    {
        (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle) where T : IComponent, new();
        void CloseFocusedWindow();
        void CloseWindow(string windowId);
    }
}
