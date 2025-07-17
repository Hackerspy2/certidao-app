using Newtonsoft.Json;

namespace Transfera
{
    public class AutentiacacaoPost
    {
        [JsonProperty("grant_type")] public string GrantType { get; set; } = "client_credentials";
        [JsonProperty("client_id")]public string Clientid { get; set; }
        [JsonProperty("client_secret")]public string ClientSecret { get; set; }
    }
    public class AutentiacacaoReponse
    {
        [JsonProperty("access_token")]public string AccessToken { get; set; }
        [JsonProperty("error")]public string Error { get; set; }
        [JsonProperty("message")]public string Message { get; set; }
        [JsonProperty("expires_in")]public int ExpiresIn { get; set; }
        [JsonProperty("statusCode")]public int StatusCode { get; set; }
    }
}
