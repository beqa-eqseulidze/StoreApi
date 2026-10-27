using System.Text.Json.Serialization;

namespace Domain.Enums.UserEnums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin = 1,
        Customer = 2
    }
}
