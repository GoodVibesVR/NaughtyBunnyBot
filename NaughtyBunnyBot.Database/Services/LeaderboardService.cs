using NaughtyBunnyBot.Database.Repositories.Abstractions;
using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;

        public LeaderboardService(ILeaderboardRepository leaderboardRepository)
        {
            _leaderboardRepository = leaderboardRepository;
        }

        public async Task<IEnumerable<LeaderboardDto>> GetGuildTopLeaderboardAsync(string guildId)
        {
            return await _leaderboardRepository.GetGuildTopLeaderboardAsync(guildId, 10);
        }

        public async Task<LeaderboardDto?> GetLeaderboardEntryByGuildAndUser(string guildId, string userId)
        {
            return await _leaderboardRepository.GetLeaderboardEntryByGuildAndUserAsync(guildId, userId);
        }

        public async Task<LeaderboardDto> UpLeaderboardEntryScore(string guildId, string userId)
        {
            var leaderboardEntry = await _leaderboardRepository.GetLeaderboardEntryByGuildAndUserAsync(guildId, userId) ??
                                   await _leaderboardRepository.CreateLeaderboardEntryAsync(guildId, userId);

            return await _leaderboardRepository.UpdateLeaderboardEntryAsync(leaderboardEntry.Id,
                leaderboardEntry.Score++);
        }
    }
}
