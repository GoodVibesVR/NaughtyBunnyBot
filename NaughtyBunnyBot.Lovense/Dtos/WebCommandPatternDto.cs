using NaughtyBunnyBot.Lovense.Enums;

namespace NaughtyBunnyBot.Lovense.Dtos;

public class WebCommandPatternDto
{
    public LovenseCommandEnum Command { get; set; }
    public string? Rule { get; set; }
    public string? Strength { get; set; }
    public int Seconds { get; set; }
    public string? Toy { get; set; }
}