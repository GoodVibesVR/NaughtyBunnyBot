using NaughtyBunnyBot.Egg.Dtos;

namespace NaughtyBunnyBot.Egg.Services.Abstractions;

public interface IEggHuntService
{
    EggHuntDto? GetEggHuntForGuild(string guildId);
    EggHuntDto EnableEggHuntForGuild(string guildId);
    EggHuntDto DisableEggHuntForGuild(string guildId);
    bool AddParticipantToEggHunt(string guildId, string userId);
    bool RemoveParticipantFromEggHunt(string guildId, string userId);
}