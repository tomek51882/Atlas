using Atlas.Core;
using Atlas.Core.Render;
using Atlas.Interfaces;
using Atlas.Services;
using Atlas.Systems;
using Atlas.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

ArgsParser.Parse(args);
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logger) =>
    {
        logger.ClearProviders();
    })
    .ConfigureAppConfiguration((context, configuration) => {

    })
    .ConfigureServices((context, services) => {
        services.AddSingleton<IRenderer, Renderer>();
        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IInputSystem, InputSystem>();
        services.AddSingleton<IComponentActivatorService, ComponentActivatorService>();
        services.AddSingleton<IAppsService, AppsService>();
        services.AddHostedService<Engine>();
    });

IHost app = builder.Build();

//     ___   __  __          
//    /   | / /_/ /___ ______
//   / /| |/ __/ / __ `/ ___/
//  / ___ / /_/ / /_/ (__  ) 
// /_/  |_\__/_/\__,_/____/
//

await app.RunAsync();