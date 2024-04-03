using NaughtyBunnyBot.Lovense.Dtos;
using NaughtyBunnyBot.Lovense.Requests;
using NaughtyBunnyBot.Lovense.Responses;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Cache.Services.Abstractions;
using NaughtyBunnyBot.Lovense.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;

namespace NaughtyBunnyBot.Lovense.Services
{
    public class LovenseService : ILovenseService
    {
        private readonly ILogger<LovenseService> _logger;
        private readonly ILovenseClient _lovenseClient;
        private readonly IMemoryCacheService _cacheService;

        public LovenseService(ILogger<LovenseService> logger, ILovenseClient lovenseClient, IMemoryCacheService cacheService)
        {
            _logger = logger;
            _lovenseClient = lovenseClient;
            _cacheService = cacheService;
        }

        public async Task<GenerateQrCodeResultDto?> GenerateQrCodeAsync(string userId, string username, string userToken)
        {
            var result = _cacheService.Get<WebCommandResponseV2?>($"{userId}-qr-code");
            if (result == null) 
            {
                result = await _lovenseClient.GetQrCodeAsync(new WebGetQrCodeRequest()
                {
                    UserId = userId,
                    Username = username,
                    UserToken = userToken
                });

                if (result != null)
                {
                    _cacheService.Set($"{userId}-qr-code", result);
                }
            }

            if (result == null)
            {
                return null;
            }

            return new GenerateQrCodeResultDto()
            {
                ImageUrl = result.Data!.QrCode,
                Result = true,
                UniqueCode = result.Data.Code
            };
        }

        public async Task<WebCommandResponseV2?> CommandAsync(List<string> userIds, WebCommandDto command)
        {
            _logger.LogDebug($"Received a command request: {JsonConvert.SerializeObject(command)}");

            return await _lovenseClient.CommandAsync(new WebCommandRequest()
            {
                UserIds = userIds,
                Strength = command.Strength,
                Seconds = command.Seconds,
            });
        }

        public async Task<WebCommandResponseV2?> CommandPatternAsync(List<string> userIds, WebCommandPatternDto command)
        {
            _logger.LogDebug($"Received a command pattern request: {JsonConvert.SerializeObject(command)}");

            return await _lovenseClient.CommandPatternAsync(new WebCommandPatternRequest()
            {
                UserIds = userIds,
                Strength = command.Strength,
                Rule = command.Rule,
                Seconds = command.Seconds,
            });
        }
    }
}
