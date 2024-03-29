using NaughtyBunnyBot.Cache.Services.Abstractions;

namespace NaughtyBunnyBot.Egg
{
    public class EggHunt
    {
        private readonly IMemoryCacheService _memoryCache;

        public EggHunt(IMemoryCacheService memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task StartEggHuntForGuildAsync(string guildId)
        {

            
        }
    }
}
