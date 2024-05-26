using Atlas.Attributes;
using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Interfaces;
using Atlas.Interfaces.Renderables;
using Atlas.Primitives;
using Atlas.Types;
using Atlas.Types.Windows;
using Atlas.Utils;
using System.Runtime.CompilerServices;

namespace Atlas.Services
{
    internal class WindowService : IWindowService
    {
        private readonly IRenderer renderer;
        private readonly IInputSystem inputSystem;
        private readonly InputSystemDebugLine inputSystemDebugLine;
        private readonly IServiceProvider serviceProvider;
        private readonly IComponentActivatorService componentActivatorService;

        private Window? FocusedWindow;
        private Dictionary<string, Window> OpenedWindows { get; set; } = new Dictionary<string, Window>();
        private Dictionary<KeyShortcut, Window> WindowsShortcuts = new Dictionary<KeyShortcut, Window>();

        public WindowService(IRenderer renderer, IInputSystem inputSystem, IServiceProvider serviceProvider, IComponentActivatorService componentActivatorService)
        {
            this.renderer = renderer;
            this.inputSystem = inputSystem;
            this.serviceProvider = serviceProvider;
            this.componentActivatorService = componentActivatorService;

            this.inputSystem.OnKeyPress += HandleKeyPress;
            (_, inputSystemDebugLine) = CreateWindow<InputSystemDebugLine>(new Types.Rect(0, 24, 126, 3), "Input Debug");
        }

        public (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle) where T : IComponent, new()
        {
            return CreateWindow<T>(windowRect, windowTitle, new ConsoleKeyInfo('\0', ConsoleKey.None, false, false, false));
        }
        public (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle, ConsoleKeyInfo shortcut) where T : IComponent, new()
        {
            if (FocusedWindow is not null)
            {
                FocusedWindow.IsFocused = false;
            }

            var component = new T();
            componentActivatorService.ResolveDependencies(component);

            FocusedWindow = new Window(windowRect, component, ShortIdGenerator.Generate(), shortcut);
            FocusedWindow.IsFocused = true;
            FocusedWindow.Title = windowTitle;
            FocusedWindow.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);
            OpenedWindows.Add(FocusedWindow.WindowId, FocusedWindow);

            if (shortcut.Key != ConsoleKey.None)
            {
                WindowsShortcuts.Add(new KeyShortcut(shortcut), FocusedWindow);
            }

            Unsafe.As<Renderer>(renderer).MountRenderable(FocusedWindow);
            return (FocusedWindow.WindowId, component);
        }

        public void CloseFocusedWindow()
        {
            if (FocusedWindow is null)
            {
                return;
            }
            var window = FocusedWindow;
            OpenedWindows.Remove(window.WindowId);
            if (OpenedWindows.Count == 0)
            {
                return;
            }

            FocusedWindow = OpenedWindows.Last().Value;
            FocusedWindow.IsFocused = true;
            Unsafe.As<Renderer>(renderer).UnmountRenderable(window);
        }

        public void CloseWindow(string windowId)
        {
            if (FocusedWindow?.WindowId == windowId)
            {
                CloseFocusedWindow();
                return;
            }

            if (OpenedWindows.TryGetValue(windowId, out Window? window))
            {
                OpenedWindows.Remove(windowId);
                Unsafe.As<Renderer>(renderer).UnmountRenderable(window);
            }
        }

        private void HandleKeyPress(ConsoleKeyInfo info)
        {
            var key = new KeyShortcut(info);
            if (key.Key == ConsoleKey.Escape)
            {
                CloseFocusedWindow();
                return;
            }
            if(WindowsShortcuts.TryGetValue(key, out var window)) 
            {
                if (FocusedWindow is not null)
                {
                    FocusedWindow.IsFocused = false;
                }
                FocusedWindow = window;
                FocusedWindow.IsFocused = true;

            }

            //

            FocusedWindow?.OnKeyPressed(info);
        }

    }

}
