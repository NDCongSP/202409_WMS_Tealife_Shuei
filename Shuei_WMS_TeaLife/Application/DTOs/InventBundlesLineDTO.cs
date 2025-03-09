namespace Application.DTOs
{
    public class InventBundlesLineDTO : InventBundleLine
    {
        public int TenantId { get; set; }

        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public double? ProductQuantity { get; set; }
        public double? StockAvailable { get; set; }
        public string? LocationName { get; set; }       

    }
}
