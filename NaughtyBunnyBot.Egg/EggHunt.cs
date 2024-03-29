using Discord;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Discord.Sender.Abstractions;
using NaughtyBunnyBot.Egg.Abstractions;
using NaughtyBunnyBot.Egg.Dtos;
using NaughtyBunnyBot.Egg.Services.Abstractions;

namespace NaughtyBunnyBot.Egg
{
    public class EggHunt : IEggHunt
    {
        private readonly ILogger<EggHunt> _logger;
        private readonly IEggHuntService _eggHuntService;
        private readonly IEggService _eggService;
        private readonly IApprovedChannelsService _channelsService;
        private readonly IDiscordMessageSender _messageSender;

        private readonly Random _random;

        public EggHunt(ILogger<EggHunt> logger, IEggHuntService eggHuntService, 
            IEggService eggService, IApprovedChannelsService channelService,
            IDiscordMessageSender messageSender)
        {
            _logger = logger;
            _eggHuntService = eggHuntService;
            _eggService = eggService;
            _channelsService = channelService;
            _messageSender = messageSender;

            _random = new Random();
        }

        public async Task StartEggHuntForGuildAsync(string guildId)
        {
            var hunt = _eggHuntService.GetEggHuntForGuild(guildId);
            if (hunt is { Enabled: true }) return;

            _eggHuntService.EnableEggHuntForGuild(guildId);
            _logger.LogDebug($"Egg hunt started for guild with ID {guildId}");

            var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));
            while (await timer.WaitForNextTickAsync())
            {
                hunt = _eggHuntService.GetEggHuntForGuild(guildId);
                if (hunt is not { Enabled: true })
                {
                    _logger.LogDebug($"Egg hunt is no longer ongoing for {guildId}. Stopping game loop...");
                    return;
                }

                if (hunt.Participants.Count == 0)
                {
                    _logger.LogDebug("No participants. Not dropping any eggs.");
                    continue;
                }

                if (_random.Next(5) > 1)
                {
                    _logger.LogDebug("Probability to drop egg not hit...");
                    continue;
                }

                var channels = await _channelsService.GetApprovedChannelByGuildAsync(guildId);
                var channelsArr = channels.ToArray();
                var eggChannel = _random.Next(channelsArr.Length);

                for (var i = 0; i < channelsArr.Length; i++)
                {
                    if (i == eggChannel)
                    {
                        await BuildEggEmbed(_eggService.GetRandomEgg(), channelsArr[i].ChannelId);
                        continue;
                    }

                    await BuildDudEmbed(_eggService.GetRandomDud(), channelsArr[i].ChannelId);
                }
            }
        }

        public Task StopEggHuntForGuild(string guildId)
        {
            _eggHuntService.DisableEggHuntForGuild(guildId);
            _logger.LogDebug($"Egg hunt stopped for guild with ID {guildId}, no more eggs will be dropped, any existing eggs can still be collected.");

            return Task.CompletedTask;
        }

        private async Task BuildEggEmbed(EggDto egg, string channelId)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(egg.Name)
                .WithDescription(egg.Description)
                .WithImageUrl(egg.ImageUrl)
                .WithColor(Color.Purple)
                .WithCurrentTimestamp()
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            var componentBuilder = new ComponentBuilder()
                .WithButton("Collect Egg", $"find-{egg.Name}", ButtonStyle.Success);

            await _messageSender.SendMessageToChannelAsync(channelId, embedBuilder, componentBuilder);
        }

        private async Task BuildDudEmbed(DudDto dud, string channelId)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(dud.Name)
                .WithDescription(dud.Description)
                .WithImageUrl(dud.ImageUrl)
                .WithColor(Color.Purple)
                .WithCurrentTimestamp()
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            await _messageSender.SendMessageToChannelAsync(channelId, embedBuilder);
        }
    }
}
