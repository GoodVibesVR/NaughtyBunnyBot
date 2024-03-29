using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Database.Services.Abstractions;

namespace NaughtyBunnyBot.Discord.Services
{
    public class ButtonInteractionService : IButtonInteractionService
    {
        private readonly ILogger<EnableCommandService> _logger;
        private readonly DiscordSocketClient _discordClient;
        private readonly ILovenseService _lovenseService;
        private readonly IEggService _eggService;
        private readonly ILeaderboardService _leaderboardService;

        public ButtonInteractionService(ILogger<EnableCommandService> logger, DiscordSocketClient discordClient,
            ILovenseService lovenseService, IEggService eggService, ILeaderboardService leaderboardService)
        {
            _logger = logger;
            _discordClient = discordClient;
            _lovenseService = lovenseService;
            _eggService = eggService;
            _leaderboardService = leaderboardService;
        }

        public async Task JoinButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(ephemeral: true);
            var guildId = component.GuildId.ToString() ?? "0";

            // uToken isn't being used as we aren't verifying it.
            var qrCodeDetails = _lovenseService.GenerateQrCodeAsync(guildId, guildId, guildId);
            if (qrCodeDetails == null || qrCodeDetails.Result == null)
            {
                await component.FollowupAsync("Failed to generate QR code. Please try again later.", ephemeral: true);
                return;
            }

            var qrCode = qrCodeDetails.Result.ImageUrl;
            var qrCodeUniqueCode = qrCodeDetails.Result.UniqueCode;

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Lovense QR Code")
                .WithDescription(@$"
Scan the QR code to connect your toy.

Or Connect via the Code:
**Unique Code:** {qrCodeUniqueCode}"
                )
                .WithImageUrl(qrCode)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            await component.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
        }

        public async Task LeaveButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync();

            var userId = component.User.Id;
            var userName = component.User.Username;

            await component.FollowupAsync("Successfully left.", ephemeral: true);
        }

        public async Task InvalidButtonHandler(SocketMessageComponent component)
        {
            await component.RespondAsync("Invalid button interaction.", ephemeral: true);
        }


        public async Task FindEggButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(ephemeral: true);

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Easter Egg")
                .WithDescription("You found the Easter Egg! 🥚")
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            await component.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);

            // ---------------------------------------
            // Award their points

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _leaderboardService.UpLeaderboardEntryScore(
                component.GuildId!.Value.ToString(), component.User.Id.ToString()
            );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // ---------------------------------------
            // Give the person the pattern

            var eggName = component.Data.CustomId.Replace("find-", string.Empty);
            var currentEgg = _eggService.GetEggByName(eggName);
            if (currentEgg is null) {
                await component.FollowupAsync("Easter Egg not found? Something bad has happened...", ephemeral: true);
                return;
            }

            await _lovenseService.CommandPatternAsync(
                component.User.Id.ToString(),
                new Lovense.Dtos.WebCommandPatternDto()
                {
                    Rule = "V:1;F:v;S:250#",
                    Strength = currentEgg.Pattern,
                    Seconds = 12,
                }
            );
        }
    }
}