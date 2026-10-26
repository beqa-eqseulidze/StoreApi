using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Tag : BaseAuditabelEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
    }
}
