using Atlas.Core;
using Atlas.Interfaces;
using Atlas.Services;
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
        services.AddHostedService<Engine>();
    });

IHost app = builder.Build();

await app.RunAsync();