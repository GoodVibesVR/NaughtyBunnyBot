using NaughtyBunnyBot.Lovense.Enums;

namespace NaughtyBunnyBot.Discord.Sender.Abstractions;

public interface IWebHookMessageSender
{
    Task SendErrorAsync(string message, int errorCode);
    Task SendErrorAsync(string message);
}