using NaughtyBunnyBot.Database.Repositories.Abstractions;
using NaughtyBunnyBot.Database.Services.Abstractions;
using NaughtyBunnyBot.Leaderboard.Dtos;

namespace NaughtyBunnyBot.Database.Services;

public class ApprovedChannelsService : IApprovedChannelsService
{
    private readonly IApprovedChannelsRepository _approvedChannelsRepository;

    public ApprovedChannelsService(IApprovedChannelsRepository approvedChannelsRepository)
    {
        _approvedChannelsRepository = approvedChannelsRepository;
    }

    public async Task<ApprovedChannelDto?> GetApprovedChannelAsync(string guildId, string channelId)
    {
        var approvedChannel = await _approvedChannelsRepository.GetApprovedChannelAsync(guildId, channelId);
        return approvedChannel ?? null;
    }

    public async Task<IEnumerable<ApprovedChannelDto>> GetApprovedChannelByGuildAsync(string guildId)
    {
        return await _approvedChannelsRepository.GetApprovedChannelByGuildAsync(guildId);
    }

    public async Task<ApprovedChannelDto> AddApprovedChannelAsync(string guildId, string channelId)
    {
        return await _approvedChannelsRepository.AddApprovedChannelAsync(guildId, channelId);
    }

    public async Task RemoveApprovedChannelAsync(string guildId, string channelId)
    {
        var approvedChannel = await _approvedChannelsRepository.GetApprovedChannelAsync(guildId, channelId);
        if (approvedChannel == null) return;

        await _approvedChannelsRepository.RemoveApprovedChannelAsync(approvedChannel.Id);
    }

    public async Task RemoveApprovedChannelByGuildAsync(string guildId)
    {
        await _approvedChannelsRepository.GetApprovedChannelByGuildAsync(guildId);
    }
}