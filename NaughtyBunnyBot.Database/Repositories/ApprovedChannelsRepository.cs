using System.Data.SqlClient;
using Dapper;
using NaughtyBunnyBot.Database.Repositories.Abstractions;
using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Repositories;

public class ApprovedChannelsRepository : IApprovedChannelsRepository
{
    private readonly string _connectionString;

    public ApprovedChannelsRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<ApprovedChannelDto?> GetApprovedChannelAsync(string guildId, string channelId)
    {
        const string sql = @"
SELECT Id,
    GuildId,
    ChannelId
FROM ApprovedChannels
WHERE GuildId = @GuildId
AND ChannelId = @ChannelId
";

        await using var connection = new SqlConnection(_connectionString);
        return connection.Query<ApprovedChannelDto>(sql, new
        {
            GuildId = guildId,
            ChannelId = channelId
        }).FirstOrDefault();
    }

    public async Task<IEnumerable<ApprovedChannelDto>> GetApprovedChannelByGuildAsync(string guildId)
    {
        const string sql = @"
SELECT Id,
    GuildId,
    ChannelId
FROM ApprovedChannels
WHERE GuildId = @GuildId
";

        await using var connection = new SqlConnection(_connectionString);
        return connection.Query<ApprovedChannelDto>(sql, new
        {
            GuildId = guildId
        });
    }

    public async Task<ApprovedChannelDto> AddApprovedChannelAsync(string guildId, string channelId)
    {
        var id = Guid.NewGuid();

        const string sql = @"
INSERT INTO ApprovedChannels (
	Id, 
	GuildId,
    ChannelId
)
VALUES (
	@Id,
	@GuildId,
    @ChannelId
)

SELECT Id,
    GuildId,
    ChannelId
FROM ApprovedChannels
WHERE Id = @Id
";

        await using var connection = new SqlConnection(_connectionString);
        return connection.Query<ApprovedChannelDto>(sql, new
        {
            GuildId = guildId,
            ChannelId = channelId,
            Id = id
        }).First();
    }

    public async Task RemoveApprovedChannelAsync(Guid id)
    {
        const string sql = @"
DELETE FROM ApprovedChannels
WHERE Id = @Id
";

        await using var connection = new SqlConnection(_connectionString);
        connection.Query(sql, new
        {
            Id = id
        });
    }

    public async Task RemoveApprovedChannelByGuildAsync(string guildId)
    {
        const string sql = @"
DELETE FROM ApprovedChannels
WHERE GuildId = @GuildId
";

        await using var connection = new SqlConnection(_connectionString);
        connection.Query(sql, new
        {
            GuildId = guildId
        });
    }
}