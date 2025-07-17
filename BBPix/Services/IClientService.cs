using BBPix.Errors;

namespace BBPix.Services;

public interface IClientService
{
   
    public abstract Task<Tuple<Dictionary<string, dynamic>, PixException>> Post(
        string url,
        Dictionary<string, string>? headers = null,
        object item = null);
   
    //public abstract Task<Tuple<Dictionary<string, dynamic>, PixException>> Get(
    //    string url,
    //    Dictionary<string, string>? headers = null,
    //    Dictionary<string, string>? queryParameters = null);
}