using LyTex.Models;
using Newtonsoft.Json;

namespace Web.Models
{
    public class WebHookPargarme
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
