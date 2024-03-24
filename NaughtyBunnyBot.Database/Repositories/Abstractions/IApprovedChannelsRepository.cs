using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Repositories.Abstractions;

public interface IApprovedChannelsRepository
{
    Task<ApprovedChannelDto?> GetApprovedChannelAsync(string guildId, string channelId);
    Task<IEnumerable<ApprovedChannelDto>> GetApprovedChannelByGuildAsync(string guildId);
    Task<ApprovedChannelDto> AddApprovedChannelAsync(string guildId, string channelId);
    Task RemoveApprovedChannelAsync(Guid id);
    Task RemoveApprovedChannelByGuildAsync(string guildId);
}