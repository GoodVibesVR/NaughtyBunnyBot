using NaughtyBunnyBot.Lovense.Enums;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace NaughtyBunnyBot.Lovense.Requests
{
    public class WebCommandPatternRequest
    {
        [JsonProperty("uid")]
        public string? UserId { get; set; }

        [Range(0, 20)]
        [JsonProperty("v")]
        public string? Rule { get; set; }

        [Range(0, 20)]
        [JsonProperty("v")]
        public string? Strength { get; set; }     // Should probs convert to int[]

        [JsonProperty("sec")]
        public int Seconds { get; set; }

        [JsonProperty("toy")]
        public string? Toy { get; set; }
    }
}
