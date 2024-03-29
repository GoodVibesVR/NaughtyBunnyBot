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
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
