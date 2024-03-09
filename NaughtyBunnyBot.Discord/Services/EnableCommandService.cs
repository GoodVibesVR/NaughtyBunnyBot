using NaughtyBunnyBot.Common.Extensions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace NaughtyBunnyBot.Discord.Services
{
    public class EnableCommandService : IEnableCommandService
    {
        private readonly ILogger<EnableCommandService> _logger;
        private readonly DiscordSocketClient _discordClient;

        public EnableCommandService(ILogger<EnableCommandService> logger, DiscordSocketClient discordClient)
        {
            _logger = logger;
            _discordClient = discordClient;
        }

        public async Task HandleEnableCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Enable command is not implemented yet.");
        }

        public async Task HandleDisableCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Disable command is not implemented yet.");
        }
    }
}