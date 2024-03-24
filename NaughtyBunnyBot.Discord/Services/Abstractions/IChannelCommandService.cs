using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface IChannelCommandService
{
    Task HandleAddChannelCommandAsync(SocketSlashCommand command);
    Task HandleRemoveChannelCommandAsync(SocketSlashCommand command);

    Task HandleListChannelsCommandAsync(SocketSlashCommand command);
}