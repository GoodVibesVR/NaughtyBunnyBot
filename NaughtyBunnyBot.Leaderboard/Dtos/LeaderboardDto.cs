namespace NaughtyBunnyBot.Leaderboard.Dtos
{
    public class LeaderboardDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string GuildId { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
