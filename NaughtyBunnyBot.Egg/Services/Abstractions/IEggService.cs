using NaughtyBunnyBot.Egg.Dtos;

namespace NaughtyBunnyBot.Egg.Services.Abstractions
{
    public interface IEggService
    {
        EggDto GetRandomEgg();
        DudDto GetRandomDud();
        EggDto? GetEggByName(string name);
    }
}
