using BBPix.Errors;
using BBPix.Models;
using BBPix.Services;

namespace BBPix.Repository;

public class TokenRepository
{
    /// <summary>
    ///     Instance of the <see cref="ClientService" /> used to make API calls.
    /// </summary>
    private readonly ClientService _client;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TokenRepository" /> class.
    /// </summary>
    /// <param name="client">The <see cref="ClientService" /> instance.</param>
    public TokenRepository(ClientService client)
    {
        _client = client;
    }

    public async Task<Tuple<string, PixException?>> GetToken(string url, string basicKey)
    {
        if (string.IsNullOrEmpty(basicKey)) return null;

        var response = await _client.PostForm(url, new Dictionary<string, string>
        {
            { "Authorization", basicKey },
            //{ "Content-Type", "application/x-www-form-urlencoded" },
        }, new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "scope", "cob.write cob.read cobv.write cobv.read pix.write pix.read" }
        });
        if (response.Item1.ContainsKey("access_token"))
        {
            return new Tuple<string, PixException?>(response.Item1["access_token"], null);
        }

        return new Tuple<string, PixException?>(null, response.Item2);
    }
}