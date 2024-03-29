namespace NaughtyBunnyBot.Egg.Abstractions
{
    public interface IEggHunt
    {
        Task StartEggHuntForGuildAsync(string guildId);
    }
}
