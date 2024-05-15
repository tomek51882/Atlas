
using Atlas.Components;
using Atlas.Interfaces;
using Atlas.Services;
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
            //ws.CreateWindow<TestComponent>();
            ws.CreateWindow<TestListComponent>(new Types.Rect(0, 0, 40, 10), "List Test");
            ws.CreateWindow<TestSelectComponent>(new Types.Rect(0, 10, 40, 14), "Select Test");
            ws.CreateWindow<FileExplorer>(new Types.Rect(40, 0, 86, 24), "🤔");

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _inputSystem.Update();
                    _renderer.Update();
                }
                //while (!stoppingToken.IsCancellationRequested)
                //{
                //    _renderer.Update();
                //    var key = Console.ReadKey(true);
                //}
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
