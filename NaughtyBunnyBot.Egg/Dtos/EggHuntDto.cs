namespace NaughtyBunnyBot.Egg.Dtos;

public class EggHuntDto
{
    public bool Enabled { get; set; }
    public List<string> Participants { get; set; } = new();
}