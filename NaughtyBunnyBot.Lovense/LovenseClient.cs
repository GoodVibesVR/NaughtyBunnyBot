using NaughtyBunnyBot.Lovense.Settings;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Lovense.Abstractions;
using NaughtyBunnyBot.Lovense.Exceptions;
using NaughtyBunnyBot.Lovense.Requests;
using NaughtyBunnyBot.Lovense.Responses;

namespace NaughtyBunnyBot.Lovense
{
    public class LovenseClient : ILovenseClient
    {
        private readonly string _clientName;
        private readonly string? _developerToken;
        private readonly IHttpClientFactory _httpClientFactory;

        public LovenseClient(IHttpClientFactory httpClientFactory, IOptions<LovenseConfig> lovenseConfig)
        {
            _clientName = lovenseConfig.Value.ClientName ?? "LovenseClient";
            _developerToken = lovenseConfig.Value.DeveloperToken;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<WebCommandResponseV2?> GetQrCodeAsync(WebGetQrCodeRequest request)
        {
            const string path = "lan/getQrCode";
            request.Token = _developerToken;
            var result = await PostAsync<WebCommandResponseV2>(path,
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            return result?.Code != 0 ? null : result;
        }

        public async Task<WebCommandResponseV2?> CommandAsync(WebCommandRequest request)
        {
            const string path = "lan/v2/command";

            var result = await PostAsync<WebCommandResponseV2>(path,
                new StringContent(JsonConvert.SerializeObject(new
                {
                    token = _developerToken,
                    uid = string.Join(",", request.UserIds!),
                    command = "Function",
                    action = $"All:{request.Strength}",
                    timeSec = request.Seconds
                }), Encoding.UTF8, "application/json"));

            return result;
        }

        public async Task<WebCommandResponseV2?> CommandPatternAsync(WebCommandPatternRequest request)
        {
            const string path = "lan/v2/command";
            var result = await PostAsync<WebCommandResponseV2>(path,
                new StringContent(JsonConvert.SerializeObject(new
                {
                    token = _developerToken,
                    uid = string.Join(",", request.UserIds!),
                    command = "Pattern",
                    rule = request.Rule,
                    strength = request.Strength,
                    timeSec = request.Seconds,
                }), Encoding.UTF8, "application/json"));

            return result;
        }

        private async Task<T?> PostAsync<T>(string path, HttpContent content)
        {
            var httpClient = _httpClientFactory.CreateClient(_clientName);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(path, content);
            var json = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<T>(json);

            throw new GeneralLovenseException(!string.IsNullOrEmpty(json) ? json : "No json data provided",
                Convert.ToInt32(response.StatusCode));
        }
    }
}
