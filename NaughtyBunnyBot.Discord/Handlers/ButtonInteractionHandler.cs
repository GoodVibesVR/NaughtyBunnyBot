using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Discord.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace NaughtyBunnyBot.Discord.Handlers;

public class ButtonInteractionHandler
{
    private readonly ILogger<ButtonInteractionHandler> _logger;
    private readonly IButtonInteractionService _buttonInteractionService;
    public ButtonInteractionHandler(ILogger<ButtonInteractionHandler> logger, IButtonInteractionService buttonInteractionService)
    {
        _logger = logger;
        _buttonInteractionService = buttonInteractionService;
    }

    public async Task ButtonExecutedAsync(SocketMessageComponent component)
    {
        switch (component.Data.CustomId)
        {
            case "join":
                await _buttonInteractionService.JoinButtonHandler(component);
                break;

            case "leave":
                await _buttonInteractionService.LeaveButtonHandler(component);
                break;

            default:
                await _buttonInteractionService.InvalidButtonHandler(component);
                break;
        }
    }
}