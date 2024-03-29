using Discord;

namespace NaughtyBunnyBot.Discord.Sender.Abstractions
{
    public interface IDiscordMessageSender
    {
        Task<IUserMessage?> SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder, ComponentBuilder componentBuilder);
        Task<IUserMessage?> SendMessageToChannelAsync(string channelId, EmbedBuilder embedBuilder);
    }
}
