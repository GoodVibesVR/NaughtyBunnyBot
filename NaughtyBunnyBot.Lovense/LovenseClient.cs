using NaughtyBunnyBot.Lovense.Settings;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using NaughtyBunnyBot.Lovense.Exceptions;
using NaughtyBunnyBot.Lovense.Requests;
using NaughtyBunnyBot.Lovense.Responses;

namespace NaughtyBunnyBot.Lovense
{
    public class LovenseClient
    {
        private readonly string? _apiRoot;
        private readonly string? _developerToken;

        public LovenseClient(IOptions<LovenseConfig> lovenseConfig)
        {
            _apiRoot = lovenseConfig.Value.ApiRoot;
            _developerToken = lovenseConfig.Value.DeveloperToken;
        }

        public async Task<WebCommandResponseV2?> GetQrCodeAsync(WebGetQrCodeRequest request)
        {
            const string path = "lan/getQrCode";
            request.Token = _developerToken;
            var result = await PostAsync<WebCommandResponseV2>(path,
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            if (result?.Code != 0)
            {
                return null;
            }

            return result;
        }

        public async Task<WebCommandResponseV2?> CommandAsync(WebCommandRequest request)
        {
            const string path = "lan/v2/command";
            //var @params = BuildCommandSpecificParameters(request);

            var result = await PostAsync<WebCommandResponseV2>(path,
                new StringContent(JsonConvert.SerializeObject(new
                {
                    token = _developerToken,
                    uid = request.UserId,
                    command = "Function",
                    action = $"{request.Command.ToString()}:{request.Value1}",
                    timeSec = request.Seconds,
                    apiVer = 1,
                    toy = request.Toy
                }), Encoding.UTF8, "application/json"));

            return result;
        }

        private async Task<T?> PostAsync<T>(string path, HttpContent content)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri($"{_apiRoot}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync(path, content);
            var json = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<T>(json);

            throw new GeneralLovenseException(!string.IsNullOrEmpty(json) ? json : "No json data provided",
                Convert.ToInt32(response.StatusCode));
        }

        private async Task<GetQrCodeResponse> GetQrCodeImageAsync(WebCommandResponseV2 informationResponse)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync(informationResponse.Data!.QrCode);
            var responseStream = await response.Content.ReadAsStreamAsync();
            var dataStream = new MemoryStream();
            await responseStream.CopyToAsync(dataStream);
            dataStream.Position = 0;

            return new GetQrCodeResponse()
            {
                DataType = response.Content.Headers.ContentType?.ToString(),
                Data = dataStream,
                Code = informationResponse.Data.Code
            };
        }
    }
}
