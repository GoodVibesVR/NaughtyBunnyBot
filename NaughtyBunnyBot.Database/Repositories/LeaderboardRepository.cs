using System.Data.SqlClient;
using Dapper;
using NaughtyBunnyBot.Database.Repositories.Abstractions;
using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Repositories
{
    public class LeaderboardRepository : ILeaderboardRepository
    {
        private readonly string _connectionString;

        public LeaderboardRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<LeaderboardDto> CreateLeaderboardEntryAsync(string guildId, string userId)
        {
            var id = Guid.NewGuid();

            const string sql = @"
INSERT INTO Leaderboard (
	Id, 
    UserId,
	GuildId
)
VALUES (
	@Id,
    @UserId,  
	@GuildId
)

SELECT Id,
    UserId,
    GuildId,
    Score
FROM Leaderboard
WHERE Id = @Id
";

            await using var connection = new SqlConnection(_connectionString);
            return connection.Query<LeaderboardDto>(sql, new
            {
                Id = id,
                UserId = userId,
                GuildId = guildId
            }).First();
        }

        public async Task<IEnumerable<LeaderboardDto>> GetGuildTopLeaderboardAsync(string guildId, int top)
        {
            const string sql = @"
SELECT TOP(@Top)
    Id,
    UserId,
    GuildId,
    Score
FROM Leaderboard
WHERE GuildId = @GuildId
ORDER BY Score DESC
";

            await using var connection = new SqlConnection(_connectionString);
            return connection.Query<LeaderboardDto>(sql, new
            {
                GuildId = guildId,
                Top = top
            });
        }

        public async Task<LeaderboardDto?> GetLeaderboardEntryByGuildAndUserAsync(string guildId, string userId)
        {
            const string sql = @"
SELECT Id,
    UserId,
    GuildId,
    Score
FROM Leaderboard
WHERE GuildId = @GuildId
AND UserId = @UserId
";

            await using var connection = new SqlConnection(_connectionString);
            return connection.Query<LeaderboardDto>(sql, new
            {
                GuildId = guildId,
                UserId = userId
            }).FirstOrDefault();
        }

        public async Task<LeaderboardDto> UpdateLeaderboardEntryAsync(Guid id, int score)
        {
            const string sql = @"
UPDATE Leaderboard
SET Score = @Score
WHERE Id = @Id

SELECT Id,
    UserId,
    GuildId,
    Score
FROM Leaderboard
WHERE Id = @Id
";

            await using var connection = new SqlConnection(_connectionString);
            return connection.Query<LeaderboardDto>(sql, new
            {
                Id = id,
                Score = score
            }).First();
        }
    }
}
