
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
        private IRenderer _renderer;
        private IWindowService _windowService;
        public Engine(IRenderer renderer, IWindowService windowService)
        {
            Console.Write("\x1b[?1049h");
            Console.CursorVisible = false;

            this._renderer = renderer;
            this._windowService = windowService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            double targetInterval = 1000.0 / 60.0;

            WindowService ws = Unsafe.As<WindowService>(_windowService);
            //ws.CreateWindow<TestComponent>();
            //windowService

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //==========================================================================
                    //stopwatch.Restart();

                    ////inputController.Update();
                    ////componentsController.Update();
                    ////diagnosticsController.Update();
                    _renderer.Update();

                    //stopwatch.Stop();

                    //int delay = (int)(targetInterval - stopwatch.ElapsedMilliseconds);
                    //if (delay > 0)
                    //{
                    //    await Task.Delay(delay);
                    //}

                    //==========================================================================
                    //await Task.Delay(200);

                    var key = Console.ReadKey(true);
                }
            }
            catch (Exception ex)
            {
                Console.Write("\x1b[?1049l");
                Console.Clear();
                Console.Write($"A terrible exception has occurred:\n{ex.Message}");
                Environment.Exit(1);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Console.Write("\x1b[?1049l");
        }
    }
}
