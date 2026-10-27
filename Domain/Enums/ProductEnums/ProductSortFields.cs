using System.Text.Json.Serialization;

namespace Domain.Enums.ProductEnums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductSortFields
    {
        Name,
        Price,        
    }
}
