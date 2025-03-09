namespace Application.DTOs.Request.Products
{
    public class InventoryInformationSearchModel
    {
        public string? Location { get; set; }
        public string? Bin { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Supplier { get; set; }
        public string? Category { get; set; }

        public string? Tenant { get; set; }

        public bool LocationDetail { get; set; }
        public bool BinDetail { get; set; }
        public bool LotExpirationDetail { get; set; }
    }
}
