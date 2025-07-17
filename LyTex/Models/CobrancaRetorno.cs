using Newtonsoft.Json;

namespace LyTex.Models;

public class CobrancaRetorno
{
    [JsonProperty("status")] public string Status { get; set; }
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("_clientId")] public string ClientId { get; set; }
    [JsonProperty("dueDate")] public DateTimeOffset DueDate { get; set; }
    [JsonProperty("paymentMethods")] public PaymentMethodsRet PaymentMethods { get; set; }
    [JsonProperty("_applicationId")] public string ApplicationId { get; set; }
    [JsonProperty("_recipientId")] public string RecipientId { get; set; }
    [JsonProperty("totalValue")] public long TotalValue { get; set; }
    [JsonProperty("cancelDate")] public DateTimeOffset CancelDate { get; set; }
    [JsonProperty("overduePaymentDate")] public DateTimeOffset OverduePaymentDate { get; set; }
    [JsonProperty("createdAt")] public DateTimeOffset CreatedAt { get; set; }
    [JsonProperty("updatedAt")] public DateTimeOffset UpdatedAt { get; set; }
}

public class PaymentMethodsRet
{
    [JsonProperty("pix")] public Pix Pix { get; set; }
    [JsonProperty("boleto")] public PaymentMethodsBoleto Boleto { get; set; }
    [JsonProperty("creditCard")] public PaymentMethodsCreditCard CreditCard { get; set; }
    [JsonProperty("list")] public List<string> List { get; set; }
}

public class PaymentMethodsBoleto
{
    [JsonProperty("dueDateDays")] public long DueDateDays { get; set; }

    [JsonProperty("enable")] public bool Enable { get; set; }

    [JsonProperty("operator")] public string Operator { get; set; }

    [JsonProperty("_referenceId")] public string ReferenceId { get; set; }

    [JsonProperty("ourNumber")] public string OurNumber { get; set; }

    [JsonProperty("digitableLine")] public string DigitableLine { get; set; }

    [JsonProperty("barcode")] public string Barcode { get; set; }

    [JsonProperty("dueDate")] public DateTimeOffset DueDate { get; set; }

    [JsonProperty("validUntil")] public DateTimeOffset ValidUntil { get; set; }
}

public class PaymentMethodsCreditCard
{
    [JsonProperty("isRatesToPayer")] public bool IsRatesToPayer { get; set; }

    [JsonProperty("maxParcels")] public long MaxParcels { get; set; }

    [JsonProperty("enable")] public bool Enable { get; set; }
}

public class Pix
{
    [JsonProperty("enable")] public bool Enable { get; set; }

    [JsonProperty("operator")] public string Operator { get; set; }

    [JsonProperty("_referenceId")] public string ReferenceId { get; set; }

    [JsonProperty("txId")] public string TxId { get; set; }

    [JsonProperty("qrcode")] public string Qrcode { get; set; }

    [JsonProperty("dueDate")] public DateTimeOffset DueDate { get; set; }

    [JsonProperty("validUntil")] public DateTimeOffset ValidUntil { get; set; }
}