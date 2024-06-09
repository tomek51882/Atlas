
using Atlas.Components;
using Atlas.Extensions;
using Atlas.Interfaces;
using Atlas.Services;
using Atlas.Types;
using Atlas.Utils;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Core
{
    internal class Engine : BackgroundService
    {
        private readonly IRenderer _renderer;
        private readonly IWindowService _windowService;
        private readonly IInputSystem _inputSystem;
        public Engine(IRenderer renderer, IWindowService windowService, IInputSystem inputSystem, IServiceProvider serviceProvider)
        {
            Console.CursorVisible = false;
            Console.Write("\x1b[?1049h");
            Console.SetCursorPosition(0, 0);
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;

            this._renderer = renderer;
            this._windowService = windowService;
            this._inputSystem = inputSystem;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            double targetInterval = 1000.0 / 60; // 1000ms / 60fps = frame duration
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(targetInterval));

            WindowService ws = Unsafe.As<WindowService>(_windowService);

            ws.CreateWindow<CalibrationComponent>(new Types.Rect(0,0, 40,10), new Types.Windows.WindowOptions
            {
                WindowTitle = "Absolute Bounds (Max Rect size)",
                BorderColor = new Types.Color(0xffa500),
                Frameless = false,
            });
            //ws.CreateWindow<AppListComponent>(Rect.Zero, new Types.Windows.WindowOptions
            //{
            //    WindowTitle = "Apps",
            //    WindowShortcut = new ConsoleKeyInfo().FromKey("A"),
            //    BorderColor = new Types.Color(0xFF0000)
            //});

            //ws.CreateWindow<FileExplorer>(new Types.Rect(40, 0, 86, 24), "File Explorer", new Types.Windows.WindowOptions { WindowShortcut = new ConsoleKeyInfo().FromKey("E") });

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _inputSystem.Update();
                    _renderer.Update();
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.Write("\x1b[?1049l");
                Console.Clear();
                Console.Write($"A terrible exception has occurred:\n{ex.Message}");

                Debugger.Break();
                Environment.Exit(1);
            }
        }

        public override void Dispose()
        {
            Console.Write("\x1b[?1049l");
            base.Dispose();
        }
    }
}
