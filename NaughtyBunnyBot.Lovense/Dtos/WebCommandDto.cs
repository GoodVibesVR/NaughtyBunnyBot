using NaughtyBunnyBot.Lovense.Enums;

namespace NaughtyBunnyBot.Lovense.Dtos;

public class WebCommandDto
{
    public LovenseCommandEnum Command { get; set; }
    public int Strength { get; set; }
    public int Seconds { get; set; }
}