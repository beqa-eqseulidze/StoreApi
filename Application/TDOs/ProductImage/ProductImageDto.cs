namespace Application.TDOs.ProductImage
{
    public record ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public bool IsMain { get; set; }
    }
}
