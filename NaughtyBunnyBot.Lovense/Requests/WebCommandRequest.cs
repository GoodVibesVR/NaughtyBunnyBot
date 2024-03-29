using NaughtyBunnyBot.Lovense.Enums;

namespace NaughtyBunnyBot.Lovense.Requests
{
    public class WebCommandRequest
    {
        public List<string>? UserIds { get; set; }
        public int Strength { get; set; }
        public int Seconds { get; set; }
    }
}
