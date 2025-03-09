namespace Application.DTOs.Request.Products
{
    public class InventoryAdjustmentDetailsModel
    {
        public string? Location { get; set; }
        public string? Bin { get; set; }
        public string? BundleCode { get; set; }
        public string? LotNo { get; set; }

        public string? ProductCode { get; set; }
        public int? TenantId { get; set; }


        

    }
}
