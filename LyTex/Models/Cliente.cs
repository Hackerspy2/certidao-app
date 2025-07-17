using Newtonsoft.Json;

namespace LyTex.Models;

public class Cliente
{
    [JsonProperty("referenceId")] public string ReferenceId { get; set; }
    [JsonProperty("groups")] public Group[] Groups { get; set; }
    [JsonProperty("address")] public Address Address { get; set; }
    [JsonProperty("cellphone")] public string Cellphone { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("cpfCnpj")] public string CpfCnpj { get; set; }
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("treatmentPronoun")] public string TreatmentPronoun { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("_recipientId")] public string RecipientId { get; set; }
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("updatedAt")] public DateTime? UpdatedAt { get; set; }
    [JsonProperty("createdAt")] public DateTime? CreatedAt { get; set; }
}

public class Address
{
    [JsonProperty("complement")] public string Complement { get; set; }
    [JsonProperty("number")] public long Number { get; set; }
    [JsonProperty("street")] public string Street { get; set; }
    [JsonProperty("zone")] public string Zone { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("state")] public string State { get; set; }
    [JsonProperty("zip")] public long Zip { get; set; }
}

public class Group
{
    [JsonProperty("_clientGroupId")] public string ClientGroupId { get; set; }
    [JsonProperty("_groupId")] public string GroupId { get; set; }
}

public class ConsultaCliente
{
    [JsonProperty("results")] public List<Result> Results { get; set; }
    [JsonProperty("paginate")] public Paginate Paginate { get; set; }
}

public class Paginate
{
    [JsonProperty("perPage")] public long PerPage { get; set; }
    [JsonProperty("page")] public long Page { get; set; }
    [JsonProperty("pages")] public long Pages { get; set; }
    [JsonProperty("total")] public long Total { get; set; }
}

public class Result
{
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("cpfCnpj")] public string CpfCnpj { get; set; }
    [JsonProperty("cellphone")] public string Cellphone { get; set; }
    [JsonProperty("treatmentPronoun")] public string TreatmentPronoun { get; set; }
    [JsonProperty("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}