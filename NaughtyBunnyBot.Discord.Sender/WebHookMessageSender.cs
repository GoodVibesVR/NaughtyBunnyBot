using System.Text;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Discord.Sender.Abstractions;
using NaughtyBunnyBot.Discord.Sender.Settings;
using NaughtyBunnyBot.Lovense.Enums;
using Newtonsoft.Json;

namespace NaughtyBunnyBot.Discord.Sender;

public class WebHookMessageSender : IWebHookMessageSender
{
    private readonly HttpClient _client;
    private readonly string _webHookId;
    private readonly string _webHookToken;

    public WebHookMessageSender(IOptions<DiscordWebHookConfig> config)
    {
        _client = new HttpClient();

        _webHookId = config.Value.WebHookId;
        _webHookToken = config.Value.WebHookToken;
    }

    public async Task SendErrorAsync(string message, int errorCode)
    {
        var errorCodeEnum = (LovenseErrorCodesEnum)errorCode;
        var webHook = new
        {
            username = "NaughtyBunnyBot",
            avatar_url = "https://cdn.discordapp.com/attachments/1214757396983971870/1221910536447328286/442b6f470db1727e4a68920a3a5cf99d.png?ex=66144bdd&is=6601d6dd&hm=ccce1962619baebbb95d638d41b2a309d0c04fdfdedc25717446fc8b2264bfb9&",
            embeds = new List<object>
            {
                new
                {
                    title = "Lovense error occurred",
                    description = $"{message}\nLovense API responded with the following error code: {errorCodeEnum.ToString().ToUpper()} ({errorCode})",
                    color = int.Parse("E7421F", System.Globalization.NumberStyles.HexNumber)
                }
            }
        };


        var endPoint = $"https://canary.discord.com/api/webhooks/{_webHookId}/{_webHookToken}";
        var content = new StringContent(JsonConvert.SerializeObject(webHook), Encoding.UTF8, "application/json");

        await _client.PostAsync(endPoint, content);
    }

    public async Task SendErrorAsync(string message)
    {
        var webHook = new
        {
            username = "NaughtyBunnyBot",
            avatar_url = "https://cdn.discordapp.com/attachments/1214757396983971870/1221910536447328286/442b6f470db1727e4a68920a3a5cf99d.png?ex=66144bdd&is=6601d6dd&hm=ccce1962619baebbb95d638d41b2a309d0c04fdfdedc25717446fc8b2264bfb9&",
            embeds = new List<object>
            {
                new
                {
                    title = "General exception occurred",
                    description = message,
                    color = int.Parse("E7421F", System.Globalization.NumberStyles.HexNumber)
                }
            }
        };


        var endPoint = $"https://canary.discord.com/api/webhooks/{_webHookId}/{_webHookToken}";
        var content = new StringContent(JsonConvert.SerializeObject(webHook), Encoding.UTF8, "application/json");

        await _client.PostAsync(endPoint, content);
    }
}