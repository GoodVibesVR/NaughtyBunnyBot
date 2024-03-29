using NaughtyBunnyBot.Common.Extensions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Egg.Abstractions;

namespace NaughtyBunnyBot.Discord.Services
{
    public class EnableCommandService : IEnableCommandService
    {
        private readonly ILogger<EnableCommandService> _logger;
        private readonly IEggHunt _eggHunt;

        public EnableCommandService(ILogger<EnableCommandService> logger, IEggHunt eggHunt)
        {
            _logger = logger;
            _eggHunt = eggHunt;
        }

        public async Task HandleEnableCommandAsync(SocketSlashCommand command)
        {
            // Send a message with an attached button with the ID `join` and the label `Join`
            var builder = new ComponentBuilder()
                .WithButton("Join",  "join", ButtonStyle.Primary)
                //.WithButton("Test Egg", "test-egg", ButtonStyle.Secondary) // Test spawn an egg
                .WithButton("Leave", "leave", ButtonStyle.Danger);

            #pragma warning disable CS4014
            Task.Run(() => _eggHunt.StartEggHuntForGuildAsync(command.GuildId.ToString()!));
            #pragma warning restore CS4014

            await command.RespondAsync("Join the hunt for the mysterious eggs around the Discord server!", components: builder.Build());
        }

        public async Task HandleDisableCommandAsync(SocketSlashCommand command)
        {
            Task.Run(() => _eggHunt.StopEggHuntForGuildAsync(command.GuildId.ToString()!));

            await command.RespondAsync("Egg hunt has been disabled for this server.");
        }
    }
}