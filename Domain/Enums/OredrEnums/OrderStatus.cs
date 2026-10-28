using System.Text.Json.Serialization;

namespace Domain.Enums.OredrEnums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending=1,
        Shipped,
        Delivered,
        Cancelled
    }
}
