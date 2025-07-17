using BBPix.Errors;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace BBPix.Services;

public class ClientService : IClientService
{

    //public async Task<Tuple<Dictionary<string, dynamic>, PixException>> Get(string url,
    //    Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParameters = null)
    //{
    //    try
    //    {
    //        var _client = GetClient(headers);
    //        var response = await _client.GetAsync(url, options: options, queryParameters: queryParameters);
    //        return Result<Dictionary<string, dynamic>, PixException>.Success(response.Data);
    //    }
    //    catch (DioError e)
    //    {
    //        if (e.Response?.Data != null)
    //            return Result<Dictionary<string, dynamic>, PixException>.Failure(new BBApiException(e.Response.Data));
    //        return Result<Dictionary<string, dynamic>, PixException>.Failure(new BBHttpException(e));
    //    }
    //    catch (Exception e)
    //    {
    //        return Result<Dictionary<string, dynamic>, PixException>.Failure(new BHttpException(e));
    //    }
    //}

    protected StringContent CreateJsonPostContent(object obj)
    {
        var json = JsonConvert.SerializeObject(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    public async Task<Tuple<Dictionary<string, dynamic>, PixException>> Post(string url,
        Dictionary<string, string>? headers = null, object post = null)
    {
        try
        {
            var _client = GetClient(headers);
            var psotData = CreateJsonPostContent(post);
            var response = await _client.PostAsync(url, psotData);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var dados = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                return new Tuple<Dictionary<string, dynamic>, PixException>(dados, null);
            }

            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(result, "erro-post"));
        }
        catch (Exception e)
        {
            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(e.Message, "erro-post"));
        }

        return null;
    }
    
    public async Task<Tuple<Dictionary<string, dynamic>, PixException>> Put(string url,
        Dictionary<string, string>? headers = null, object post = null)
    {
        try
        {
            var _client = GetClient(headers);
            var psotData = CreateJsonPostContent(post);
            var response = await _client.PutAsync(url, psotData);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var dados = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                return new Tuple<Dictionary<string, dynamic>, PixException>(dados, null);
            }

            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(result, "erro-post"));
        }
        catch (Exception e)
        {
            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(e.Message, "erro-post"));
        }

        return null;
    }

    public async Task<Tuple<Dictionary<string, dynamic>, PixException>> PostForm(string url,
        Dictionary<string, string>? headers = null, Dictionary<string, string>? queryPost = null)
    {
        try
        {
            var client = GetClient(headers);
            var formContent = new FormUrlEncodedContent(queryPost);
            var response = await client.PostAsync(url, formContent);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var dados = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(result);
                return new Tuple<Dictionary<string, dynamic>, PixException>(dados, null);
            }

            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(result, "erro-post"));
        }
        catch (Exception e)
        {
            return new Tuple<Dictionary<string, dynamic>, PixException>(null, new PixException(e.Message, "erro-post"));
        }
    }

    protected HttpClient GetClient(Dictionary<string, string>? headers = null)
    {
        var client = new HttpClient();
        if (Equals(headers, null)) return client;

        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Add("accept", "application/json");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        foreach (var header in headers)
        {
            if (header.Key == "Bearer")
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {header.Value}");
            else
                client.DefaultRequestHeaders.Add(header.Key, header.Value);

        }
        return client;
    }
}