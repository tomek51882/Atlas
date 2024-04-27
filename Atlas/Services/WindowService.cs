using Atlas.Components;
using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Types;
using System.Runtime.CompilerServices;

namespace Atlas.Services
{
    internal class WindowService : IWindowService
    {
        private readonly IRenderer renderer;
        private Window? FocusedWindow;

        public WindowService(IRenderer renderer)
        {
            this.renderer = renderer;
        }

        internal void CreateWindow<T>() where T : ComponentBase, new()
        {
            FocusedWindow = new Window(new Rect(0,0,40,10), new T());
            FocusedWindow.IsFocused = true;
            FocusedWindow.Title = "123";
            Unsafe.As<Renderer>(renderer).MountRenderable(FocusedWindow);
        }
    }
}
