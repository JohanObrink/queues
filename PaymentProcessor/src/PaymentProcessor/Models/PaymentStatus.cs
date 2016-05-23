using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PaymentProcessor.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PaymentStatus
    {
        Pending,
        Success,
        Fail
    }
}
