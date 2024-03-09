using Newtonsoft.Json;

namespace NaughtyBunnyBot.Lovense.Requests;

public class WebGetQrCodeRequest
{
    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("uid")]
    public string? UserId { get; set; }

    [JsonProperty("uname")]
    public string? Username { get; set; }

    [JsonProperty("utoken")]
    public string? UserToken { get; set; }

    [JsonProperty("v")]
    public int ApiVersion => 2;
}