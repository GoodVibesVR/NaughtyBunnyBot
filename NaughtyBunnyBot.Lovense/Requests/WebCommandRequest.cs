using NaughtyBunnyBot.Lovense.Enums;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace NaughtyBunnyBot.Lovense.Requests
{
    public class WebCommandRequest
    {
        [JsonProperty("uid")]
        public string? UserId { get; set; }

        [JsonProperty("command")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LovenseCommandEnum Command { get; set; }

        [Range(0, 20)]
        [JsonProperty("v")]
        public int Value1 { get; set; }

        [Range(0, 20)]
        [JsonProperty("v")]
        public int Value2 { get; set; }

        [JsonProperty("sec")]
        public int Seconds { get; set; }

        [JsonProperty("toy")]
        public string? Toy { get; set; }
    }
}
