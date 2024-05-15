
using Atlas.Interfaces.Renderables;
using Atlas.Types;

namespace Atlas.Interfaces
{
    internal interface IWindowService
    {
        (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle) where T : IComponent, new();
    }
}
