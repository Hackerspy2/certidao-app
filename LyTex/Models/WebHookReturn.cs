using Newtonsoft.Json;

namespace LyTex.Models;

public class WebHookReturn
{
    [JsonProperty("webhookType")] public string WebhookType { get; set; }
    [JsonProperty("data")] public Data Data { get; set; }
    [JsonProperty("signature")] public string Signature { get; set; }

    //[JsonProperty("id")] public Guid Id { get; set; }

    //[JsonProperty("version")] public string Version { get; set; }

    //[JsonProperty("account_id")] public Guid AccountId { get; set; }

    //[JsonProperty("object")] public string Object { get; set; }

    //[JsonProperty("date")] public DateTimeOffset Date { get; set; }
}

public class Data
{
    [JsonProperty("id")] public string InvoiceId { get; set; }
    [JsonProperty("payedAt")] public DateTimeOffset PayedAt { get; set; }
    [JsonProperty("payedValue")] public long PayedValue { get; set; }
    [JsonProperty("paymentMethod")] public string PaymentMethod { get; set; }
    [JsonProperty("referenceId")] public long ReferenceId { get; set; }
    [JsonProperty("status")] public string Status { get; set; }

    //[JsonProperty("id")] public Guid? Id { get; set; }

    //[JsonProperty("value")] public double? Value { get; set; }

    //[JsonProperty("end2end_id")] public string? End2EndId { get; set; }

    //[JsonProperty("receipt_file_url")] public string? ReceiptFileUrl { get; set; }

    //[JsonProperty("txid")] public string? Txid { get; set; }

    //[JsonProperty("integration_id")] public string? IntegrationId { get; set; }

    //[JsonProperty("pix_key")] public string? PixKey { get; set; }

    //[JsonProperty("pix_description")] public string? PixDescription { get; set; }
}