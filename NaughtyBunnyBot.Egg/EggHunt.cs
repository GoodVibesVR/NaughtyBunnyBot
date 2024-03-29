using Discord;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Cache.Services.Abstractions;
using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Discord.Sender.Abstractions;
using NaughtyBunnyBot.Egg.Abstractions;
using NaughtyBunnyBot.Egg.Dtos;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Egg.Settings;
using NaughtyBunnyBot.Lovense.Dtos;
using NaughtyBunnyBot.Lovense.Services.Abstractions;

namespace NaughtyBunnyBot.Egg
{
    public class EggHunt : IEggHunt
    {
        private readonly ILogger<EggHunt> _logger;
        private readonly IEggHuntService _eggHuntService;
        private readonly IEggService _eggService;
        private readonly IApprovedChannelsService _channelsService;
        private readonly IDiscordMessageSender _messageSender;
        private readonly IMemoryCacheService _cacheService;
        private readonly ILovenseService _lovenseService;

        private readonly Random _random;
        private readonly EggHuntConfig _config;

        public EggHunt(ILogger<EggHunt> logger, IEggHuntService eggHuntService, 
            IEggService eggService, IApprovedChannelsService channelService,
            IDiscordMessageSender messageSender, IMemoryCacheService cacheService, 
            ILovenseService lovenseService, IOptions<EggHuntConfig> config)
        {
            _logger = logger;
            _eggHuntService = eggHuntService;
            _eggService = eggService;
            _channelsService = channelService;
            _messageSender = messageSender;
            _cacheService = cacheService;
            _lovenseService = lovenseService;

            _random = new Random();
            _config = config.Value;
        }

        public async Task StartEggHuntForGuildAsync(string guildId)
        {
            var hunt = _eggHuntService.GetEggHuntForGuild(guildId);
            if (hunt is { Enabled: true }) return;

            _cacheService.Set($"{guildId}-hunt-ongoing", false);

            _eggHuntService.EnableEggHuntForGuild(guildId);
            _logger.LogDebug($"Egg hunt started for guild with ID {guildId}");

            var timer = new PeriodicTimer(TimeSpan.FromSeconds(_config.TimerSeconds));
            var messageId = string.Empty;
            while (await timer.WaitForNextTickAsync())
            {
                hunt = _eggHuntService.GetEggHuntForGuild(guildId);
                if (hunt is not { Enabled: true })
                {
                    _logger.LogDebug($"Egg hunt is no longer ongoing for {guildId}. Stopping game loop...");
                    return;
                }

                var ongoing = _cacheService.Get<bool>($"{guildId}-hunt-ongoing");
                if (ongoing)
                {
                    _logger.LogDebug("Hunt is still ongoing. No need to drop a new egg now...");
                    continue;
                }

                if (hunt.Participants.Count == 0)
                {
                    _logger.LogDebug("No participants. Not dropping any eggs.");
                    continue;
                }

                if (_random.Next(_config.Probability) > _config.Chance)
                {
                    _logger.LogDebug("Probability to drop egg not hit...");
                    continue;
                }

                var channels = await _channelsService.GetApprovedChannelByGuildAsync(guildId);
                var channelsArr = channels.ToArray();
                var eggChannel = _random.Next(channelsArr.Length);

                for (var i = 0; i < channelsArr.Length; i++)
                {
                    IUserMessage? message;
                    if (i == eggChannel)
                    {
                        message = await BuildEggEmbedAsync(_eggService.GetRandomEgg(), channelsArr[i].ChannelId);
                    }
                    else
                    {
                        message = await BuildDudEmbedAsync(_eggService.GetRandomDud(), channelsArr[i].ChannelId);
                    }

                    _cacheService.Set($"{message!.Id}-participant-count", hunt.Participants.Count);
                    _cacheService.Set($"{guildId}-hunt-ongoing", true);
                }

                await StartVibeLoop(guildId, hunt.Participants);
            }
        }

        public Task StopEggHuntForGuild(string guildId)
        {
            _eggHuntService.DisableEggHuntForGuild(guildId);
            _logger.LogDebug($"Egg hunt stopped for guild with ID {guildId}, no more eggs will be dropped, any existing eggs can still be collected.");

            return Task.CompletedTask;
        }

        private async Task StartVibeLoop(string guildId, List<string> participants)
        {
            var strength = 1;
            for (var i = 0; i < _config.VibeLoopSeconds; i++)
            {
                var ongoing = _cacheService.Get<bool>($"{guildId}-hunt-ongoing");
                if (!ongoing) return;

                if (i % 3 == 0)
                {
                    strength = strength + 1 > 20 ? 20 : strength + 1;
                }

                await _lovenseService.CommandAsync(participants, new WebCommandDto()
                {
                    Strength = strength,
                    Seconds = 4
                });

                await Task.Delay(1000);
            }

            _cacheService.Set($"{guildId}-hunt-ongoing", false);
        }

        private async Task<IUserMessage?> BuildEggEmbedAsync(EggDto egg, string channelId)
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

            return await _messageSender.SendMessageToChannelAsync(channelId, embedBuilder, componentBuilder);
        }

        private async Task<IUserMessage?> BuildDudEmbedAsync(DudDto dud, string channelId)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(dud.Name)
                .WithDescription(dud.Description)
                .WithImageUrl(dud.ImageUrl)
                .WithColor(Color.Purple)
                .WithCurrentTimestamp()
                .WithFooter("NaughtyBunnyBot - Made by @miwca and @kitty_cass");

            return await _messageSender.SendMessageToChannelAsync(channelId, embedBuilder);
        }
    }
}
