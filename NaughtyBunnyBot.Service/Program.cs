using Microsoft.Extensions.Logging.EventLog;
using NaughtyBunnyBot.Service.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options =>
    {
        if (OperatingSystem.IsWindows())
        {
            options.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.RegisterDependencies(configuration);

        if (OperatingSystem.IsWindows())
        {
            services.Configure<EventLogSettings>(config =>
            {
                if (OperatingSystem.IsWindows())
                {
                    config.LogName = "NaughtyBunnyBot";
                    config.SourceName = "NaughtyBunnyBot Source";
                }
            });
        }
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
