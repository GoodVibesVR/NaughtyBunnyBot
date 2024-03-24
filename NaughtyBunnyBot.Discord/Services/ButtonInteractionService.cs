using NaughtyBunnyBot.Common.Extensions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace NaughtyBunnyBot.Discord.Services
{
    public class ButtonInteractionService : IButtonInteractionService
    {
        private readonly ILogger<EnableCommandService> _logger;
        private readonly DiscordSocketClient _discordClient;
        private readonly ILovenseService _lovenseService;

        public ButtonInteractionService(ILogger<EnableCommandService> logger, DiscordSocketClient discordClient, ILovenseService lovenseService)
        {
            _logger = logger;
            _discordClient = discordClient;
            _lovenseService = lovenseService;
        }

        public async Task JoinButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync();

            var guildID = component.GuildId.ToString() ?? "0";

            // uToken isn't being used as we aren't verifying it.
            var qrCodeDetails = _lovenseService.GenerateQrCodeAsync(guildID, guildID, guildID); 

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

            await component.FollowupAsync(embed: embedBuilder.Build());
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
    }
}