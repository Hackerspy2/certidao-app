using BBPix.Errors;
using BBPix.Models;
using BBPix.Repository;
using BBPix.Services;

namespace BBPix;

public enum Ambiente
{
    Producao,
    Homologacao
}

public class PixBB
{
    private readonly Ambiente _ambiente;
    private string _apiUrl;
    private string _apiUrlPix;
    private readonly string _basicKey;
    private readonly ClientService _client;
    private readonly string _developerApplicationKey;
    private string _tokenUrl;

    public PixBB(Ambiente ambiente, string basicKey, string developerApplicationKey)
    {
        _ambiente = ambiente;
        _basicKey = basicKey;
        _developerApplicationKey = developerApplicationKey;
        _client = new ClientService();
        _changeAmbiente();
    }

    private void _changeAmbiente()
    {
        switch (_ambiente)
        {
            case Ambiente.Producao:
                _tokenUrl = "https://oauth.bb.com.br/oauth/token";
                _apiUrl = "https://api.bb.com.br/pix/v1";
                break;
            case Ambiente.Homologacao:
                _tokenUrl = "https://oauth.sandbox.bb.com.br/oauth/token";
                _apiUrl = "https://api.hm.bb.com.br/pix/v1";
                _apiUrlPix = "https://api.sandbox.bb.com.br/pix/v2";
                break;
        }
    }

    public async Task<Tuple<string, PixException?>> GetToken()
    {
        var repository = new TokenRepository(_client);
        return await repository.GetToken(_tokenUrl, _basicKey);
    }
    
    public async Task<Tuple<Dictionary<string, dynamic>, PixException>> GeraPix(string url, object queryParameters)
    {
        var token = await GetToken();
        var dados = await new ClientService().Post($"{_apiUrlPix}{url}?gw-dev-app-key={_developerApplicationKey}", new Dictionary<string, string>() { { "Bearer", token.Item1 } }, queryParameters);
        return dados;
    }
    
    public async Task<Tuple<Dictionary<string, dynamic>, PixException>> ConsultaPix(string url, object queryParameters)
    {
        var token = await GetToken();
        var dados = await new ClientService().Put($"{_apiUrlPix}{url}?gw-dev-app-key={_developerApplicationKey}", new Dictionary<string, string>() { { "Bearer", token.Item1 } }, queryParameters);
        return dados;
    }
}