using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Database.Services.Abstractions;
using System.Runtime.Caching.Hosting;
using NaughtyBunnyBot.Cache.Services.Abstractions;
using NaughtyBunnyBot.Egg.Settings;
using Microsoft.Extensions.Options;

namespace NaughtyBunnyBot.Discord.Services
{
    public class ButtonInteractionService : IButtonInteractionService
    {
        private readonly ILogger<EnableCommandService> _logger;
        private readonly DiscordSocketClient _discordClient;
        private readonly ILovenseService _lovenseService;
        private readonly IEggService _eggService;
        private readonly IEggHuntService _eggHuntService;
        private readonly ILeaderboardService _leaderboardService;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly int configMaxClaimed;
        

        public ButtonInteractionService(ILogger<EnableCommandService> logger, DiscordSocketClient discordClient,
            ILovenseService lovenseService, IEggService eggService, IEggHuntService eggHuntService, 
            ILeaderboardService leaderboardService, IMemoryCacheService memoryCacheService, IOptions<EggHuntConfig> config
        )
        {
            _logger = logger;
            _discordClient = discordClient;
            _lovenseService = lovenseService;
            _eggService = eggService;
            _eggHuntService = eggHuntService;
            _leaderboardService = leaderboardService;
            _memoryCacheService = memoryCacheService;
            
            configMaxClaimed = config.Value.MaxClaimed;
        }

        public async Task JoinButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(ephemeral: true);

            //var guildId = component.GuildId.ToString() ?? "0";
            var uId = component.User.Id.ToString();

            // uToken isn't being used as we aren't verifying it.
            var qrCodeDetails = await _lovenseService.GenerateQrCodeAsync(uId, uId, uId);
            if (qrCodeDetails == null)
            {
                await component.FollowupAsync("Failed to generate QR code. Please try again later.", ephemeral: true);
                return;
            }

            var qrCode = qrCodeDetails.ImageUrl;
            var qrCodeUniqueCode = qrCodeDetails.UniqueCode;

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Lovense QR Code")
                .WithDescription(@$"
Scan the QR code to connect your toy.

Or Connect via the Code:
**Unique Code:** {qrCodeUniqueCode}"
                )
                .WithImageUrl(qrCode)
                .WithColor(215, 42, 119)    // Lovense Pink
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            _eggHuntService.AddParticipantToEggHunt(component.GuildId.ToString()!, component.User.Id.ToString());
            await component.FollowupAsync(embed: embedBuilder.Build(), ephemeral: true);
        }

        public async Task LeaveButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync();

            _eggHuntService.RemoveParticipantFromEggHunt(component.GuildId.ToString()!, component.User.Id.ToString());

            await component.FollowupAsync("Successfully left, we hope you join us again later.", ephemeral: true);
        }

        public async Task InvalidButtonHandler(SocketMessageComponent component)
        {
            await component.RespondAsync("Invalid button interaction.", ephemeral: true);
        }

        public async Task SendTestEggButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(ephemeral: true);

            var randomEgg = _eggService.GetRandomEgg();

            var embedBuilder = new EmbedBuilder()
                .WithTitle(randomEgg.Name)
                .WithDescription(randomEgg.Description)
                .WithImageUrl(randomEgg.ImageUrl)
                .WithColor(Color.Purple)
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            var ComponentBuilder = new ComponentBuilder()
                .WithButton("Collect Egg", $"find-{randomEgg.Name}", ButtonStyle.Success);

            await component.FollowupAsync(
                embed: embedBuilder.Build(),
                components: ComponentBuilder.Build(),
                ephemeral: true
            );
        }

        public async Task FindEggButtonHandler(SocketMessageComponent component)
        {
            await component.DeferAsync(ephemeral: true);

            // Check if the user is not participating in the event
            var isParticipating = _eggHuntService.IsParticipantInEggHunt(component.GuildId.ToString()!, component.User.Id.ToString());
            if (!isParticipating)
            {
                await component.FollowupAsync("You are not participating in the Easter Egg Hunt.", ephemeral: true);
                return;
            }


            // Check if the message is older than 10 minutes
            // This is a safety check to prevent people from claiming eggs after the Cache possibily has expired to know if the egg has been fully claimed.
            if (DateTimeOffset.UtcNow - component.Message.CreatedAt > TimeSpan.FromMinutes(10))
            {
                await component.FollowupAsync("This Easter Egg has expired.", ephemeral: true);

                await DisableFindEggButton(component);
                return;
            }


            // Check if the egg is fully claimed
            var claimedCount = _memoryCacheService.Get<int>($"{component.Message.Id}-claimed");
            if (claimedCount >= configMaxClaimed)
            {
                await component.FollowupAsync("All Easter Eggs have been claimed.", ephemeral: true);

                await DisableFindEggButton(component);
                return;
            }

            _memoryCacheService.Set($"{component.Message.Id}-claimed", claimedCount + 1);



            // Check if the user has already redeemed this egg
            var cacheKey = $"{component.GuildId}-{component.User.Id}-{component.Message.Id}";
            var cacheValue = _memoryCacheService.Get<bool>(cacheKey);

            if (cacheValue)
            {
                await component.FollowupAsync("You have already redeemed this egg.", ephemeral: true);
                return;
            }

            _memoryCacheService.Set(cacheKey, true);

            // ---------------------------------------
            // Send the easter egg message

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Easter Egg")
                .WithDescription("You found the Easter Egg! 🥚")
                .WithColor(Color.LighterGrey)
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
            if (currentEgg is null)
            {
                await component.FollowupAsync("Easter Egg not found? Something bad has happened...", ephemeral: true);
                return;
            }

            await _lovenseService.CommandPatternAsync(
               new List<string>(){ component.User.Id.ToString() },
                new Lovense.Dtos.WebCommandPatternDto()
                {
                    Rule = "V:1;F:v;S:250#",
                    Strength = currentEgg.Pattern,
                    Seconds = 12,
                }
            );
        }

        private async Task DisableFindEggButton(SocketMessageComponent component)
        {
            var componentBuilder = new ComponentBuilder()
                .WithButton("No more eggs left.", "______", ButtonStyle.Danger, disabled: true)
                .Build();

            // Get the channel
            var channel = await _discordClient.GetChannelAsync(component.ChannelId??0);

            // Get the message
            if (channel is ITextChannel textChannel)
            {
                var message = await textChannel.GetMessageAsync(component.Message.Id);
                if (message is IUserMessage userMessage)
                {
                    await userMessage.ModifyAsync(m => {
                        m.Components = componentBuilder;
                    });
                }
            }
        }
    }
}