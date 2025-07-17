using Newtonsoft.Json;

namespace Transfera;

public class PixConsuta
{
    public string Token { get; set; }
    public string TxId { get; set; }
}

public class PixPost
{
    [JsonProperty("payer")] public Payer Payer { get; set; }

    [JsonProperty("value_change_mode")] public string ValueChangeMode { get; set; } = "VALOR_FIXADO";

    [JsonProperty("pix_key")] public string PixKey { get; set; }
    [JsonProperty("expiration")] public int Expiration { get; set; } = 86400;

    [JsonProperty("original_value")] public string OriginalValue { get; set; }

    [JsonProperty("payer_question")] public string PayerQuestion { get; set; }

    [JsonProperty("reject_unknown_payer")] public bool RejectUnknownPayer { get; set; } = true;
    public string Token { get; set; }
}

public partial class Data
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("payer")]
    public Payer Payer { get; set; }

    [JsonProperty("amount")]
    public long Amount { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonProperty("next_action")]
    public NextAction NextAction { get; set; }

    [JsonProperty("payment_method_details")]
    public PaymentMethodDetails PaymentMethodDetails { get; set; }

    [JsonProperty("return_url")]
    public object ReturnUrl { get; set; }

    [JsonProperty("error")]
    public object Error { get; set; }

    [JsonProperty("bussiness_account_holder")]
    public BussinessAccountHolder BussinessAccountHolder { get; set; }
}

public partial class PaymentMethodDetails
{
    [JsonProperty("institution_id")]
    public Guid InstitutionId { get; set; }
}

public partial class BussinessAccountHolder
{
}

public partial class NextAction
{
    [JsonProperty("redirect_to_url")]
    public RedirectToUrl RedirectToUrl { get; set; }
}

public partial class RedirectToUrl
{
    [JsonProperty("url")]
    public Uri Url { get; set; }
}

public class Payer
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("document")] public string Document { get; set; }
}
public class PixResponse
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("qrcode_type")]
    public string QrcodeType { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("txid")]
    public string Txid { get; set; }

    [JsonProperty("integration_id")]
    public string IntegrationId { get; set; }

    [JsonProperty("pix_key")]
    public PixKey PixKey { get; set; }

    [JsonProperty("expiration")]
    public long Expiration { get; set; }

    [JsonProperty("original_value")]
    public long OriginalValue { get; set; }

    [JsonProperty("value_change_mode")]
    public string ValueChangeMode { get; set; }

    [JsonProperty("withdraw")]
    public Change Withdraw { get; set; }

    [JsonProperty("change")]
    public Change Change { get; set; }

    [JsonProperty("payer_question")]
    public string PayerQuestion { get; set; }

    [JsonProperty("additional_info")]
    public AdditionalInfo[] AdditionalInfo { get; set; }

    [JsonProperty("payer")]
    public Payer Payer { get; set; }

    [JsonProperty("reject_unknown_payer")]
    public bool RejectUnknownPayer { get; set; }

    [JsonProperty("emv_payload")]
    public string EmvPayload { get; set; }

    [JsonProperty("image_base64")]
    public string ImageBase64 { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
public partial class AdditionalInfo
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("value")]
    public string Value { get; set; }
}

public partial class Change
{
}

public partial class PixKey
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("key_type")]
    public string KeyType { get; set; }
}