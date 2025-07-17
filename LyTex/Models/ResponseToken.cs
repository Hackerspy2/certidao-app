using Newtonsoft.Json;

namespace LyTex.Models;

internal class ResponseToken
{
    [JsonProperty("accessToken")] public string AccessToken { get; set; }

    [JsonProperty("refreshToken")] public string RefreshToken { get; set; }

    [JsonProperty("expireAt")] public DateTime ExpireAt { get; set; }

    [JsonProperty("refreshExpireAt")] public DateTime RefreshExpireAt { get; set; }
}