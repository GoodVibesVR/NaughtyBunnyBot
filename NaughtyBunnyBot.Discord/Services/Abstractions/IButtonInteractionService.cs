using Discord.WebSocket;

namespace NaughtyBunnyBot.Discord.Services.Abstractions;

public interface IButtonInteractionService
{
    Task JoinButtonHandler(SocketMessageComponent component);
    Task LeaveButtonHandler(SocketMessageComponent component);
    Task InvalidButtonHandler(SocketMessageComponent component);
    Task FindEggButtonHandler(SocketMessageComponent component);
    Task SendTestEggButtonHandler(SocketMessageComponent component);
}