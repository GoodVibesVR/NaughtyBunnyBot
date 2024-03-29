using NaughtyBunnyBot.Cache.Services.Abstractions;
using NaughtyBunnyBot.Egg.Dtos;
using NaughtyBunnyBot.Egg.Services.Abstractions;

namespace NaughtyBunnyBot.Egg.Services;

public class EggHuntService : IEggHuntService
{
    private readonly IMemoryCacheService _memoryCache;

    public EggHuntService(IMemoryCacheService memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public EggHuntDto? GetEggHuntForGuild(string guildId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        return eggHunt;
    }

    public EggHuntDto EnableEggHuntForGuild(string guildId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        if (eggHunt is { Enabled: true }) return eggHunt;

        eggHunt = new EggHuntDto()
        {
            Enabled = true
        };

        _memoryCache.Set(guildId, eggHunt);
        return eggHunt;
    }

    public EggHuntDto DisableEggHuntForGuild(string guildId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        switch (eggHunt)
        {
            case null:
                return new EggHuntDto() { Enabled = false };
            case { Enabled: false }:
                return eggHunt;
        }

        eggHunt = new EggHuntDto()
        {
            Enabled = false
        };

        _memoryCache.Set(guildId, eggHunt);
        return eggHunt;
    }

    public bool AddParticipantToEggHunt(string guildId, string userId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        if (eggHunt is not { Enabled: true })
        {
            return false;
        }

        var participant = eggHunt.Participants.FirstOrDefault(p => p == userId);
        if (participant != null)
        {
            return false;
        }

        eggHunt.Participants.Add(userId);
        _memoryCache.Set(guildId, eggHunt);

        return true;
    }

    public bool RemoveParticipantFromEggHunt(string guildId, string userId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        if (eggHunt is not { Enabled: true })
        {
            return false;
        }

        var participant = eggHunt.Participants.FirstOrDefault(p => p == userId);
        if (participant != null)
        {
            return false;
        }

        eggHunt.Participants.Remove(userId);
        _memoryCache.Set(guildId, eggHunt);

        return true;
    }

    public bool IsParticipantInEggHunt(string guildId, string userId)
    {
        var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
        if (eggHunt is not { Enabled: true })
        {
            return false;
        }

        return eggHunt.Participants.Contains(userId);
    }
}