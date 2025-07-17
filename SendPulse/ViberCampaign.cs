using Newtonsoft.Json;

namespace SendPulse;

public class ViberCampaignButton
{
    [JsonProperty("link")] public string Link;

    [JsonProperty("text")] public string Text;
}

public class ViberCampaignImage
{
    [JsonProperty("link")] public string Link;
}

public class ViberCampaignResendSms
{
    [JsonProperty("sms_sender_name")] public string SenderName;

    [JsonProperty("status")] public bool Status;

    [JsonProperty("sms_text")] public string Text;
}

public enum ViberCampaignMessageType
{
    Marketing = 2,
    Transactional = 3
}

public class ViberCampaignAdditional
{
    [JsonProperty("button")] public ViberCampaignButton Button;

    [JsonProperty("image")] public ViberCampaignImage Image;

    [JsonProperty("resend_sms")] public ViberCampaignResendSms ResendSms;
}

public class ViberCampaign
{
    [JsonProperty("additional")] public ViberCampaignAdditional Additional;

    [JsonProperty("address_book")] public uint AddressBook;

    [JsonProperty("message")] public string Message = "";

    [JsonProperty("message_live_time")] public uint MessageLiveTime = 60;

    [JsonProperty("message_type")] public ViberCampaignMessageType MessageType = ViberCampaignMessageType.Marketing;

    [JsonProperty("task_name")] public string Name;

    [JsonProperty("recipients")] public string[] Recipients = { };

    [JsonProperty("send_date")] [JsonConverter(typeof(ViberDateTimeConverter))]
    public DateTime SendDate = DateTime.Now;

    [JsonProperty("sender_id")] public uint SenderId;
}