using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Repositories.Abstractions
{
    public interface ILeaderboardRepository
    {
        Task<LeaderboardDto> CreateLeaderboardEntryAsync(string guildId, string userId);
        Task<IEnumerable<LeaderboardDto>> GetGuildTopLeaderboardAsync(string guildId, int top);
        Task<LeaderboardDto?> GetLeaderboardEntryByGuildAndUserAsync(string guildId, string userId);
        Task<LeaderboardDto> UpdateLeaderboardEntryAsync(Guid id, int score);
    }
}
