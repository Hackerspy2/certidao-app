using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Web.Models;

public class Certidao
{
    [JsonProperty("requestId")] public string RequestId { get; set; }
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("mensagem")] public string Mensagem { get; set; }

    [JsonProperty("cpfCnpj")] public string CpfCnpj { get; set; }

    [JsonProperty("nome")] public string Nome { get; set; }

    [JsonProperty("numeroCertidao")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long NumeroCertidao { get; set; }

    [JsonProperty("dataHora")] public object DataHora { get; set; }

    [JsonProperty("codigoSeguranca")] public string CodigoSeguranca { get; set; }

    [JsonProperty("pdf")] public string Pdf { get; set; }

    [JsonProperty("errors")]
    public Dictionary<string, string[]> Errors { get; set; } 
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            }
        };
    }
}


internal class ParseStringConverter : JsonConverter
{
    public static readonly ParseStringConverter Singleton = new();

    public override bool CanConvert(Type t)
    {
        return t == typeof(long) || t == typeof(long?);
    }

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        long l;
        if (long.TryParse(value, out l)) return l;
        throw new Exception("Cannot unmarshal type long");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }

        var value = (long)untypedValue;
        serializer.Serialize(writer, value.ToString());
    }
}