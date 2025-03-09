namespace Application.DTOs.Request.Products
{
    public class InventoryInformationDetailsSearchModel
    {
        public string? Location { get; set; }
        public string? Tenant { get; set; }
        public string? Bin { get; set; }
        public string? Lot { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Supplier { get; set; }
        public string? TransType { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public DateOnly? LotExpiredDate { get; set; }

    }
}
