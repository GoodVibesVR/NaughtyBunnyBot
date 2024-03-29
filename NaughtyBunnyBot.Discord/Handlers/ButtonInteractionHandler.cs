using NaughtyBunnyBot.Discord.Services.Abstractions;
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
        if (component.Data.CustomId.IndexOf("find-", StringComparison.Ordinal) == 0) {
            await _buttonInteractionService.FindEggButtonHandler(component);
            return;
        }

        switch (component.Data.CustomId)
        {
            case "join":
                await _buttonInteractionService.JoinButtonHandler(component);
                break;

            case "leave":
                await _buttonInteractionService.LeaveButtonHandler(component);
                break;

            case "test-egg":
                await _buttonInteractionService.SendTestEggButtonHandler(component);
                break;


            default:
                await _buttonInteractionService.InvalidButtonHandler(component);
                break;
        }
    }
}