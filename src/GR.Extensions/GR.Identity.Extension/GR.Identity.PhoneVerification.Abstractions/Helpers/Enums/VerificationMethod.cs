using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GR.Identity.PhoneVerification.Abstractions.Helpers.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VerificationMethod
    {
        Sms,
        Call
    }
}
