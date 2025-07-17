using Newtonsoft.Json;

namespace LyTex.Models;

public class Cobranca
{
    [JsonProperty("_id")] public string Id { get; set; }
    [JsonProperty("_clientId")] public string ClientId { get; set; }
    [JsonProperty("client")] public Cliente Client { get; set; }

    [JsonProperty("items")] public Item[] Items { get; set; }

    [JsonProperty("dueDate")] public DateTimeOffset DueDate { get; set; }

    [JsonProperty("paymentMethods")] public PaymentMethods PaymentMethods { get; set; }

    [JsonProperty("managedSubscription")] public ManagedSubscription ManagedSubscription { get; set; }

    [JsonProperty("split")] public Split Split { get; set; }

    [JsonProperty("discount")] public WelcomeDiscount Discount { get; set; }

    [JsonProperty("_billingRuleId")] public object BillingRuleId { get; set; }

    [JsonProperty("notifications")] public Notification[] Notifications { get; set; }

    [JsonProperty("referenceId")] public string ReferenceId { get; set; }

    [JsonProperty("cancelOverdueDays")] public long CancelOverdueDays { get; set; }

    [JsonProperty("overduePaymentDays")] public long OverduePaymentDays { get; set; }

    [JsonProperty("mulctAndInterest")] public MulctAndInterest MulctAndInterest { get; set; }

    [JsonProperty("documents")] public Document[] Documents { get; set; }

    [JsonProperty("observation")] public string Observation { get; set; }

    [JsonProperty("partnerId")] public string PartnerId { get; set; }
}

public class WelcomeDiscount
{
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("discounts")] public DiscountElement[] Discounts { get; set; }
}

public class DiscountElement
{
    [JsonProperty("value")] public long Value { get; set; }

    [JsonProperty("validUntil")] public DateTimeOffset ValidUntil { get; set; }
}

public class Document
{
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("url")] public string Url { get; set; }

    [JsonProperty("viewMode")] public string ViewMode { get; set; }
}

public class Item
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("quantity")] public long Quantity { get; set; }
    [JsonProperty("value")] public long Value { get; set; }
}

public class ManagedSubscription
{
    [JsonProperty("enable")] public bool Enable { get; set; }
}

public class MulctAndInterest
{
    [JsonProperty("enable")] public bool Enable { get; set; }
    [JsonProperty("mulct")] public Interest Mulct { get; set; }
    [JsonProperty("interest")] public Interest Interest { get; set; }
}

public class Interest
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("value")] public double Value { get; set; }
}

public class Notification
{
    [JsonProperty("event")] public string Event { get; set; }
    [JsonProperty("channel")] public string Channel { get; set; }
    [JsonProperty("beforeOverdue")] public BeforeOverdue BeforeOverdue { get; set; }
    [JsonProperty("afterOverdue")] public AfterOverdue AfterOverdue { get; set; }
}

public class AfterOverdue
{
    [JsonProperty("daysInit")] public long DaysInit { get; set; }
    [JsonProperty("daysInterval")] public long DaysInterval { get; set; }
    [JsonProperty("daysDuration")] public long DaysDuration { get; set; }
}

public class BeforeOverdue
{
    [JsonProperty("days")] public long Days { get; set; }
}

public class PaymentMethods
{
    [JsonProperty("pix")] public ManagedSubscription Pix { get; set; }

    [JsonProperty("boleto")] public ManagedSubscription Boleto { get; set; }
    [JsonProperty("creditCard")] public ManagedSubscription CartaoCredito { get; set; }
}

public class Split
{
    [JsonProperty("taxationMode")] public string TaxationMode { get; set; }

    [JsonProperty("recipients")] public Recipient[] Recipients { get; set; }
}

public class Recipient
{
    [JsonProperty("_recipientId")] public string RecipientId { get; set; }

    [JsonProperty("commission")] public Interest Commission { get; set; }

    [JsonProperty("originalCommission")] public Interest OriginalCommission { get; set; }

    [JsonProperty("cpfCnpj")] public string CpfCnpj { get; set; }
}