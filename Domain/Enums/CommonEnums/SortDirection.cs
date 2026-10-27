using System.Text.Json.Serialization;

namespace Domain.Enums.CommonEnums
{
    [JsonConverter (typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        Asc = 0,
        Desc = 1
    }
}
