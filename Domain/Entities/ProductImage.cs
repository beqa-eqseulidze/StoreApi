using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public bool IsMain { get; set; } = false;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
