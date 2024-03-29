using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Discord.Sender.Abstractions;

namespace NaughtyBunnyBot.Discord.Sender
{
    public class DiscordMessageSender : IDiscordMessageSender
    {
        private readonly ILogger<DiscordMessageSender> _logger;
        private readonly DiscordSocketClient _discordClient;

        public DiscordMessageSender(ILogger<DiscordMessageSender> logger, DiscordSocketClient discordClient)
        {
            _logger = logger;
            _discordClient = discordClient;
        }

        public async Task<IUserMessage?> SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder, 
            ComponentBuilder componentBuilder)
        {
            var channel = await _discordClient.GetChannelAsync(Convert.ToUInt64(channelId)) as IMessageChannel;
            if (channel == null)
            {
                _logger.LogError($"Cannot find channel with ID {channelId}. Cannot send message.");
                return null;
            }

            return await channel.SendMessageAsync(
                embed: embedBuilder.Build(),
                components: componentBuilder.Build());
        }

        public async Task<IUserMessage?> SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder)
        {
            var channel = await _discordClient.GetChannelAsync(Convert.ToUInt64(channelId)) as IMessageChannel;
            if (channel == null)
            {
                _logger.LogError($"Cannot find channel with ID {channelId}. Cannot send message.");
                return null;
            }

            return await channel.SendMessageAsync(embed: embedBuilder.Build());
        }
    }
}
