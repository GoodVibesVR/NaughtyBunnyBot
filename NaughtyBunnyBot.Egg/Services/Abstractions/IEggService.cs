using NaughtyBunnyBot.Egg.Dtos;

namespace NaughtyBunnyBot.Egg.Services.Abstractions
{
    public interface IEggService
    {
        EggDto GetRandomEgg();
        DudDto GetRandomDud();
        EggDto? GetEggByName(string name);
        bool AddParticipantToEggHunt(string guildId, string userId);
        bool RemoveParticipantFromEggHunt(string guildId, string userId);
        bool IsParticipantInEggHunt(string guildId, string userId);
    }
}
