using Atlas.Attributes;
using Atlas.Components;
using Atlas.Core.Render;
using Atlas.Extensions;
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
        private readonly IServiceProvider serviceProvider;
        private readonly IComponentActivatorService componentActivatorService;


        private readonly InputSystemDebugLine inputSystemDebugLine;
        private readonly StatusBar statusBar;
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
            (_, statusBar) = CreateWindow<StatusBar>(new Types.Rect(0, 26, 126, 1), "", new WindowOptions { Frameless = true });
            (_, inputSystemDebugLine) = CreateWindow<InputSystemDebugLine>(new Types.Rect(0, 25, 126, 1), "Input Debug", new WindowOptions { Frameless = true });
        }

        public (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle) where T : IComponent, new()
        {
            return CreateWindow<T>(windowRect, windowTitle, null);
        }
        public (string windowId, T windowComponent) CreateWindow<T>(Rect windowRect, string windowTitle, WindowOptions? options) where T : IComponent, new()
        {
            if (FocusedWindow is not null)
            {
                FocusedWindow.IsFocused = false;
            }

            var component = new T();
            componentActivatorService.ResolveDependencies(component);

            if (options.HasValue == false)
            {
                options = WindowOptions.Default;
            }

            FocusedWindow = new Window(windowRect, component, ShortIdGenerator.Generate(), options.Value);
            FocusedWindow.IsFocused = true;
            FocusedWindow.Title = windowTitle;
            OpenedWindows.Add(FocusedWindow.WindowId, FocusedWindow);

            if (options.HasValue) //kinda pointless
            {
                var windowBorderColor = new Color(0xFF00FF);
                if (options.Value.WindowShortcut.Key != ConsoleKey.None)
                {
                    WindowsShortcuts.Add(new KeyShortcut(options.Value.WindowShortcut), FocusedWindow);
                }
                if (options.Value.Frameless == false)
                {
                    FocusedWindow.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);
                }
                if (options.Value.BorderColor.validColor)
                {
                    windowBorderColor = options.Value.BorderColor;
                }
                FocusedWindow.StyleProperties.Color = new Core.Styles.StyleProperty<Color>(windowBorderColor);
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
            WindowsShortcuts.Remove(new KeyShortcut(window.Options.WindowShortcut));
            Unsafe.As<Renderer>(renderer).UnmountRenderable(window);

            if (OpenedWindows.Count == 0)
            {
                return;
            }

            FocusedWindow = OpenedWindows.Last().Value;
            FocusedWindow.IsFocused = true;
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

            if(key.Key == ConsoleKey.Q) {
                CreateWindow<FileExplorer>(new Types.Rect(40, 0, 86, 24), "File Explorer", new Types.Windows.WindowOptions { WindowShortcut = new ConsoleKeyInfo().FromKey("E") });
            }

            if (WindowsShortcuts.TryGetValue(key, out var window))
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
