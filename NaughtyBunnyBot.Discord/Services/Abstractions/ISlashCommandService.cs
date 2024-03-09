using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface ISlashCommandService
{
    Task BuildSlashCommandsAsync();
    Task HandleTestCommandAsync(SocketSlashCommand command);
}