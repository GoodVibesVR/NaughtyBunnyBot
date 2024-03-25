using NaughtyBunnyBot.Egg.Dtos;

namespace NaughtyBunnyBot.Egg.Services.Abstractions
{
    public interface IEggService
    {
        EggDto GetRandomEgg();

        EggDto? GetEggByName(string name);
    }
}
