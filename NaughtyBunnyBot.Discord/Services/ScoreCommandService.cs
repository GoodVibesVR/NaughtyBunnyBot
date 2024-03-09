using NaughtyBunnyBot.Common.Extensions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace NaughtyBunnyBot.Discord.Services
{
    public class ScoreCommandService : IScoreCommandService
    {
        private readonly ILogger<ScoreCommandService> _logger;
        private readonly DiscordSocketClient _discordClient;

        public ScoreCommandService(ILogger<ScoreCommandService> logger, DiscordSocketClient discordClient)
        {
            _logger = logger;
            _discordClient = discordClient;
        }

        public async Task HandleLeaderboardCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Leaderboard command is not implemented yet.");
        }

        public async Task HandleProfileCommandAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Profile command is not implemented yet.");
        }
    }
}
