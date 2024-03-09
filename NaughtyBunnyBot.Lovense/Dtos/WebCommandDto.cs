using NaughtyBunnyBot.Lovense.Enums;

namespace NaughtyBunnyBot.Lovense.Dtos;

public class WebCommandDto
{
    public LovenseCommandEnum Command { get; set; }
    public int Value1 { get; set; }
    public int Value2 { get; set; }
    public int Seconds { get; set; }
    public string? Toy { get; set; }
}