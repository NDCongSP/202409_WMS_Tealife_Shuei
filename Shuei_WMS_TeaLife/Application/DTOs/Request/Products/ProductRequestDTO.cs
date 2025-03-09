namespace Application.DTOs.Request.Products
{
    public class ProductRequestDTO
    {
        public int ProductId { get; set; }
        public string? ProductImage { get; set; }
        public string? Weight { get; set; }
        public string? ProductShortName { get; set; }
        public int CategoryId { get; set; }
        public string? MakerManagementCode { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ProductPrice { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public string? ProductCategory { get; set; }
        public string? ProductMaker { get; set; }
        public string? Depth { get; set; }
        public string? Height { get; set; }
        public string? Currency { get; set; }
        public string? StandardPrice { get; set; }
        public bool WarehouseProcessingFlag { get; set; }
        public string? ProductURL { get; set; }
        public string? FileNameImage { get; set; }
    }
}
