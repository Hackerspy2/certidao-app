using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Refit;

namespace Transfera
{
    public interface ITransferaApi
    {
        [Post("/pix/qrcode/collection/immediate")]
        Task<PixResponse?> GerarPix([Body] PixPost pix);

        [Get("/pix/qrcode/{txid}")]
        Task<PixResponse?> Consultar([Query] string txid);

    }

    public interface ITransferaApiAutenticar
    {
        [Post("/authorization")]
        Task<AutentiacacaoReponse?> Autenticar([Body] AutentiacacaoPost post);
    }

    public class TransferaApi : ITransferaApi
    {
        private readonly ITransferaApi cieloApi;
        private readonly HttpClient httpClient;

        public TransferaApi()
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.transfeera.com")
            };
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            };
            cieloApi = RestService.For<ITransferaApi>(httpClient, refitSettings);
        }

        public async Task<PixResponse?> GerarPix(PixPost post)
        {
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "DOCUMENTOS CERTIDAO NEGATIVA LTDA (julionevesgestor@gmail.com)");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", post.Token);
            post.Token = null;
            var response = await cieloApi.GerarPix(post);
            return response;
        }
        public async Task<PixResponse?> Consultar(string infos)
        {
            var dados = infos.Split('|');
            httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "DOCUMENTOS CERTIDAO NEGATIVA LTDA (julionevesgestor@gmail.com)");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",dados[0]);
            var response = await cieloApi.Consultar(dados[1]);
            return response;
        }
    }

    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly string _token;

        public AuthenticatedHttpClientHandler(string token)
        {
            _token = token;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
    public class TransferaApiAutenticar : ITransferaApiAutenticar
    {
        private readonly ITransferaApiAutenticar cieloApi;
        private readonly HttpClient httpClient;
        public TransferaApiAutenticar()
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri("https://login-api.transfeera.com")
            };
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                })
            };
            cieloApi = RestService.For<ITransferaApiAutenticar>(httpClient, refitSettings);
        }

        public async Task<AutentiacacaoReponse?> Autenticar(AutentiacacaoPost post)
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent", "DOCUMENTOS CERTIDAO NEGATIVA LTDA (julionevesgestor@gmail.com)");
            var response = await cieloApi.Autenticar(post);
            return response;
        }
    }

}
