using NaughtyBunnyBot.Lovense.Dtos;
using NaughtyBunnyBot.Lovense.Requests;
using NaughtyBunnyBot.Lovense.Responses;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using NaughtyBunnyBot.Lovense.Abstractions;
using NaughtyBunnyBot.Lovense.Services.Abstractions;

namespace NaughtyBunnyBot.Lovense.Services
{
    public class LovenseService : ILovenseService
    {
        private readonly ILogger<LovenseService> _logger;
        private readonly ILovenseClient _lovenseClient;

        public LovenseService(ILogger<LovenseService> logger, ILovenseClient lovenseClient)
        {
            _logger = logger;
            _lovenseClient = lovenseClient;
        }

        public async Task<GenerateQrCodeResultDto?> GenerateQrCodeAsync(string userId, string username, string userToken)
        {
            var result = await _lovenseClient.GetQrCodeAsync(new WebGetQrCodeRequest()
            {
                UserId = userId,
                Username = username,
                UserToken = userToken
            });
            
            if (result == null)
            {
                return null;
            }

            return new GenerateQrCodeResultDto()
            {
                ImageUrl = result.Data!.QrCode,
                Result = true,
                UniqueCode = result.Code
            };
        }

        public async Task<WebCommandResponseV2?> CommandAsync(string userId, WebCommandDto command)
        {
            _logger.LogDebug($"Received a command request: {JsonConvert.SerializeObject(command)}");

            return await _lovenseClient.CommandAsync(new WebCommandRequest()
            {
                UserId = userId,
                Command = command.Command,
                Value1 = command.Value1,
                Value2 = command.Value2,
                Seconds = command.Seconds,
                Toy = command.Toy
            });
        }

        public async Task<WebCommandResponseV2?> CommandPatternAsync(string userId, WebCommandPatternDto command)
        {
            _logger.LogDebug($"Received a command pattern request: {JsonConvert.SerializeObject(command)}");

            return await _lovenseClient.CommandPatternAsync(new WebCommandPatternRequest()
            {
                UserId = userId,
                Strength = command.Strength,
                Rule = command.Rule,
                Seconds = command.Seconds,
                Toy = command.Toy
            });
        }

        //public async Task HandleCallBackAsync(LovenseCallbackDto callback)
        //{
        //    var session = await _sessionService.GetSessionAsync(callback.Uid!);
        //    if (session == null)
        //    {
        //        return;
        //    }

        //    if (callback.UToken != session.UserToken)
        //    {
        //        _logger.LogDebug($"Session token and callback token differs for session {callback.Uid}");
        //        return;
        //    }

        //    if (!string.IsNullOrEmpty(callback.Domain))
        //    {
        //        callback.Domain = callback.Domain.Replace(".lovense.club", "").Replace("-", ".");
        //    }

        //    await _toysCacheClient.SetAsync(callback.Uid, callback); // TODO: Do we actually need this one?
        //    await _toysCacheClient.PublishMessageAsync(ChannelConstants.SignalRSendCallbackToClientChannelConstant, callback);
        //}
    }
}
