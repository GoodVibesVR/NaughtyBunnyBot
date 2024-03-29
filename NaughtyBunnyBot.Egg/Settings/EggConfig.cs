using NaughtyBunnyBot.Egg.Dtos;

namespace NaughtyBunnyBot.Egg.Settings
{
    public class EggConfig
    {
        public List<EggDto> Eggs { get; set; } = new List<EggDto>();
        public List<DudDto> Duds { get; set; } = new List<DudDto>();
    }
}
