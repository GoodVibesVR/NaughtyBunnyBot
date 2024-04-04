using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface IEggHuntSlashCommandService
{
    Task JoinSlashCommandHandlerAsync(SocketSlashCommand command);
    Task LeaveSlashCommandHandlerAsync(SocketSlashCommand command);
}