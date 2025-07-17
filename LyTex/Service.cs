using System.Net.Http.Headers;
using System.Text;
using LyTex.Models;
using Newtonsoft.Json;

namespace LyTex;

public class Service
{
    private string _bearer;
    public HttpClient Cliente;

    public Service(bool sandBox)
    {
        Url = sandBox ? "https://sandbox-api-pay.lytex.com.br" : "https://api-pay.lytex.com.br";
        UrlToken = sandBox ? "https://sandbox-auth-pay.lytex.com.br" : "https://auth-pay.lytex.com.br";
    }

    public string Url { get; set; }
    public string UrlToken { get; set; }

    protected HttpClient GetClient()
    {
        var client = new HttpClient();

        if (string.IsNullOrWhiteSpace(_bearer) == false)
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearer}");

        return client;
    }

    protected StringContent CreateJsonPostContent(object obj)
    {
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public async Task GerarToken(string clientId, string clientSecret, string[] escopo)
    {
        Cliente = GetClient();
        var json = new GeraToken
        {
            GrantType = "clientCredentials",
            ClientId = clientId,
            ClientSecret = clientSecret,
            Scopes = escopo
        };
        var content = CreateJsonPostContent(json);
        Cliente.DefaultRequestHeaders.Accept.Clear();
        Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await Cliente.PostAsync($"{UrlToken}/v1/oauth/obtain_token", content);
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var retorno = JsonConvert.DeserializeObject<ResponseToken>(result);
            _bearer = retorno.AccessToken;
        }
    }

    public async Task<Cliente?> CadastarCliente(string cpf, string nome, string email, string celular)
    {
        Cliente = GetClient();
        var json = new Cliente
        {
            Type = "pf",
            Name = nome,
            CpfCnpj = cpf.Replace(".", "").Replace("-", "").Replace("/", ""),
            Email = email,
            Cellphone = celular.Replace("(", "").Replace(")", "").Replace("-", "").Trim()
        };
        var content = CreateJsonPostContent(json);
        Cliente.DefaultRequestHeaders.Accept.Clear();
        Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await Cliente.PostAsync($"{Url}/v2/clients", content);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return null;
        var retorno = JsonConvert.DeserializeObject<Cliente>(result);
        return retorno;
    }

    public async Task<CobrancaRetorno?> GerarCobranca(string clienteId, Item[] itens)
    {
        Cliente = GetClient();
        var json = new Cobranca
        {
            ClientId = clienteId,
            Items = itens,
            DueDate = DateTime.Now,
            PaymentMethods = new PaymentMethods { Pix = new ManagedSubscription { Enable = true }, Boleto = new ManagedSubscription(){ Enable = true}, CartaoCredito = new ManagedSubscription(){ Enable = false}},
            Notifications = new Notification[]{new Notification{ Event = "invoicePaid", Channel = "whatsApp" }, new Notification{ Event = "invoicePaid", Channel = "sms" } },
            CancelOverdueDays = 1,
        };
        var content = CreateJsonPostContent(json);
        Cliente.DefaultRequestHeaders.Accept.Clear();
        Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await Cliente.PostAsync($"{Url}/v2/invoices", content);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return null;
        var retorno = JsonConvert.DeserializeObject<CobrancaRetorno>(result);
        return retorno;
    }
    
    public async Task<CobrancaRetorno?> ConsultarCobranca(string id)
    {
        Cliente = GetClient();
        Cliente.DefaultRequestHeaders.Accept.Clear();
        Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await Cliente.GetAsync($"{Url}/v2/invoices/{id}");
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return null;
        var retorno = JsonConvert.DeserializeObject<CobrancaRetorno>(result);
        return retorno;
    }
    
    public async Task<ConsultaCliente?> ConsultarCliente(string cpf)
    {
        Cliente = GetClient();
        Cliente.DefaultRequestHeaders.Accept.Clear();
        Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await Cliente.GetAsync($"{Url}/v2/clients/?search={cpf}");
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode) return null;
        var retorno = JsonConvert.DeserializeObject<ConsultaCliente>(result);
        return retorno;
    }
}