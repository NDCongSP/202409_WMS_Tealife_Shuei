namespace Application.DTOs.Request.Products
{
    public class ProductSearchRequestDTO
    {
        public string? ProductCode { get; set; } = null;
        public int? CategoryId { get; set; } = 0;
        public EnumProductStatus? ProductStatus { get; set; } = EnumProductStatus.All;
        public int? TenantId { get; set; } = 0;
    }
}
