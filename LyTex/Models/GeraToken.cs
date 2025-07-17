using Newtonsoft.Json;

namespace LyTex.Models;

internal class GeraToken
{
    [JsonProperty("grantType")] public string GrantType { get; set; }

    [JsonProperty("clientId")] public string ClientId { get; set; }

    [JsonProperty("clientSecret")] public string ClientSecret { get; set; }

    [JsonProperty("scopes")] public string[] Scopes { get; set; }
}