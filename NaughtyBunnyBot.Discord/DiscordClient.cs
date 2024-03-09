using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Discord.Handlers;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using DiscordConfig = NaughtyBunnyBot.Discord.Settings.DiscordConfig;

namespace NaughtyBunnyBot.Discord
{
    public class DiscordClient : BackgroundService
    {
        private readonly ILogger<DiscordClient> _logger;
        private readonly DiscordConfig _discordSettings;
        private readonly DiscordSocketClient _discordClient;
        private readonly ISlashCommandService _commandService;

        public DiscordClient(ILogger<DiscordClient> logger, IOptions<DiscordConfig> discordSettings, DiscordSocketClient discordClient, 
            SlashCommandHandler commandHandler, ISlashCommandService commandService)
        {
            _logger = logger;
            _discordSettings = discordSettings.Value;
            _commandService = commandService;

            _discordClient = discordClient;
            _discordClient.SlashCommandExecuted += commandHandler.SlashCommandExecutedAsync;
            _discordClient.Log += LogReceivedHandler;
            _discordClient.Ready += _discordClient_Ready; // Not for production
        }

#pragma warning disable CS1998
        private async Task _discordClient_Ready()
#pragma warning restore CS1998
        {
#pragma warning disable CS4014
            _commandService.BuildSlashCommandsAsync();
#pragma warning restore CS4014
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _discordClient.LoginAsync(TokenType.Bot, _discordSettings.Token);
            await _discordClient.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _discordClient.StopAsync();
        }

        private Task LogReceivedHandler(LogMessage arg)
        {
            var message = $"{arg.Source} - {arg.Message}";
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(arg.Exception, message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(arg.Exception, message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(arg.Exception, message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(arg.Exception, message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogDebug(arg.Exception, message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(arg.Exception, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }
    }
}