using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Egg.Dtos;
using NaughtyBunnyBot.Egg.Services.Abstractions;
using NaughtyBunnyBot.Egg.Settings;

namespace NaughtyBunnyBot.Egg.Services
{
    public class EggService : IEggService
    {
        private readonly List<EggDto> _eggs;

        public EggService(IOptions<EggConfig> config)
        {
            _eggs = config.Value.Eggs;
        }

        public EggDto GetRandomEgg()
        {
            var random = new Random();
            var next = random.Next(_eggs.Count);

            return _eggs[next];
        }
    }
}
