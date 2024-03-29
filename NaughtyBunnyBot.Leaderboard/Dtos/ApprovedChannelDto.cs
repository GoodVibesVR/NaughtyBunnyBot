namespace NaughtyBunnyBot.Leaderboard.Dtos;

public class ApprovedChannelDto
{
    public Guid Id { get; set; }
    public string GuildId { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
}