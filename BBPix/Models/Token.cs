using System.Text.Json;

namespace BBPix.Models;

public class Token
{
   
    public Token(string accessToken, string tokenType, int expiresIn, string scope)
    {
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        Scope = scope;
    }

    /// <summary>
    ///     The access token string.
    /// </summary>
    public string AccessToken { get; }

    
    public string TokenType { get; }

    
    public int ExpiresIn { get; }

    
    public string Scope { get; }

    
    public Dictionary<string, dynamic> ToDictionary()
    {
        return new Dictionary<string, dynamic>
        {
            { "access_token", AccessToken },
            { "token_type", TokenType },
            { "expires_in", ExpiresIn },
            { "scope", Scope }
        };
    }

    
    public static Token FromDictionary(Dictionary<string, dynamic> dictionary)
    {
        return new Token(
            dictionary["access_token"].ToString(),
            dictionary["token_type"].ToString(),
            int.Parse(dictionary["expires_in"]),
            dictionary["scope"].ToString()
        );
    }

    
    public string ToJson()
    {
        return JsonSerializer.Serialize(ToDictionary());
    }

    
    public static Token FromJson(string json)
    {
        return FromDictionary(JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json));
    }
}