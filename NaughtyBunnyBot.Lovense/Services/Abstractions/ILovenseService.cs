using NaughtyBunnyBot.Lovense.Dtos;
using NaughtyBunnyBot.Lovense.Responses;

namespace NaughtyBunnyBot.Lovense.Services.Abstractions
{
    public interface ILovenseService
    {
        Task<GenerateQrCodeResultDto?> GenerateQrCodeAsync(string userId, string username, string userToken);
        Task<WebCommandResponseV2?> CommandAsync(string userId, WebCommandDto command);
    }
}
