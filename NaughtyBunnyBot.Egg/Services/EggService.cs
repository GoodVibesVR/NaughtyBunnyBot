using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Cache.Services.Abstractions;
using NaughtyBunnyBot.Egg.Dtos;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Egg.Settings;

namespace NaughtyBunnyBot.Egg.Services
{
    public class EggService : IEggService
    {
        private readonly IMemoryCacheService _memoryCache;

        private readonly List<EggDto> _eggs;
        private readonly List<DudDto> _duds;

        public EggService(IOptions<EggConfig> config, IMemoryCacheService memoryCache)
        {
            _memoryCache = memoryCache;

            _eggs = config.Value.Eggs;
            _duds = config.Value.Duds;
        }

        public EggDto GetRandomEgg()
        {
            var random = new Random();
            var next = random.Next(_eggs.Count);

            return _eggs[next];
        }

        public DudDto GetRandomDud()
        {
            var random = new Random();
            var next = random.Next(_duds.Count);

            return _duds[next];
        }

        public EggDto? GetEggByName(string name)
        {
            return _eggs.FirstOrDefault(x => x.Name == name);
        }

        public EggHuntDto? GetEggHuntForGuild(string guildId)
        {
            var eggHunt = _memoryCache.Get<EggHuntDto>(guildId);
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
}
