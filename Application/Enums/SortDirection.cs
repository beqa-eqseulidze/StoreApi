
using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter (typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        Asc = 0,
        Desc = 1
    }
}
