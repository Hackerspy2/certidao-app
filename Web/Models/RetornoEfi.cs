using Newtonsoft.Json;

namespace Web.Models;

public class RetornoEfi
{
    [JsonProperty("calendario")] public Calendario Calendario { get; set; }

    [JsonProperty("txid")] public string Txid { get; set; }

    [JsonProperty("revisao")] public long Revisao { get; set; }

    [JsonProperty("loc")] public Loc Loc { get; set; }

    [JsonProperty("location")] public string Location { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("devedor")] public Devedor Devedor { get; set; }

    [JsonProperty("valor")] public Valor Valor { get; set; }

    [JsonProperty("chave")] public Guid Chave { get; set; }

    [JsonProperty("solicitacaoPagador")] public Guid SolicitacaoPagador { get; set; }
}

public class Calendario
{
    [JsonProperty("criacao")] public DateTimeOffset Criacao { get; set; }

    [JsonProperty("expiracao")] public long Expiracao { get; set; }
}

public class Devedor
{
    [JsonProperty("cpf")] public string Cpf { get; set; }

    [JsonProperty("nome")] public string Nome { get; set; }
}

public class Loc
{
    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("location")] public string Location { get; set; }

    [JsonProperty("tipoCob")] public string TipoCob { get; set; }

    [JsonProperty("criacao")] public DateTimeOffset Criacao { get; set; }
}

public class Valor
{
    [JsonProperty("original")] public string Original { get; set; }
}