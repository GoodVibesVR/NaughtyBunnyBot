using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace NaughtyBunnyBot.Lovense.Requests
{
    public class WebCommandPatternRequest
    {
        public List<string>? UserIds { get; set; }
        public string? Rule { get; set; }
        public string? Strength { get; set; }
        public int Seconds { get; set; }
    }
}
