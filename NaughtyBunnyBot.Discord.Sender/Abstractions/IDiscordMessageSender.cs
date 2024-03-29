using Discord;

namespace NaughtyBunnyBot.Discord.Sender.Abstractions
{
    public interface IDiscordMessageSender
    {
        Task SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder, ComponentBuilder componentBuilder);
        Task SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder);
    }
}
