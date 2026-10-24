using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskItemSortBy
    {
        DueDate = 0,
        Title = 1,
        Description = 2,
        Status = 3,
        CreatedAt = 4,
        Id = 5
    }
}
