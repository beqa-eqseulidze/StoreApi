using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ProductTag
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
