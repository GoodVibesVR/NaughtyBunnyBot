using NaughtyBunnyBot.Lovense.Dtos;
using NaughtyBunnyBot.Lovense.Responses;

namespace NaughtyBunnyBot.Lovense.Services.Abstractions
{
    public interface ILovenseService
    {
        Task<GenerateQrCodeResultDto?> GenerateQrCodeAsync(string userId, string username, string userToken);
        Task<WebCommandResponseV2?> CommandAsync(List<string> userIds, WebCommandDto command);
        Task<WebCommandResponseV2?> CommandPatternAsync(List<string> userIds, WebCommandPatternDto command);
    }
}
