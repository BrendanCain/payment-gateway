using System.Text.Json.Serialization;

namespace Com.Checkout.PaymentGateway.Models
{
    /// <summary>
    /// Enumeration of possible states of a payment after it's been received by the bank
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentStatus
    {
        //TODO - this needs more payment knowledge to implement properly
        Pending,
        Succeeded,
        Failed
    }
}
