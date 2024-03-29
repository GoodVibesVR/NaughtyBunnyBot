using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface IScoreCommandService
{
    Task HandleLeaderboardCommandAsync(SocketSlashCommand command);
    Task HandleProfileCommandAsync(SocketSlashCommand command);
}