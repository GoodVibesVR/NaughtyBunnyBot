using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface IEnableCommandService
{
    Task HandleEnableCommandAsync(SocketSlashCommand command);
    Task HandleDisableCommandAsync(SocketSlashCommand command);
}