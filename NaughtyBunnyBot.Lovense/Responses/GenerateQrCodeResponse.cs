using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyBunnyBot.Lovense.Responses
{
    public class GenerateQrCodeResponse
    {
        [JsonProperty("imageKey")]
        public string? ImageKey { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("uniqueCode")]
        public int? UniqueCode { get; set; }
    }

    public class GetQrCodeResponse
    {
        public string? DataType { get; set; }
        public int? Code { get; set; }
        public MemoryStream? Data { get; set; }
    }

    public class WebCommandResponse
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
    }

    public class WebCommandResponseV2
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("data")]
        public WebCommandResponseV2Data? Data { get; set; }
    }

    public class WebCommandResponseV2Data
    {
        [JsonProperty("qr")]
        public string? QrCode { get; set; }

        [JsonProperty("code")]
        public int? Code { get; set; }
    }
}
