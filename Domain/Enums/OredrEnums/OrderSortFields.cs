
using System.Text.Json.Serialization;

namespace Domain.Enums.OredrEnums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderSortFields
    {
        OrderDate,
        Amount,
        Status
    }
}
