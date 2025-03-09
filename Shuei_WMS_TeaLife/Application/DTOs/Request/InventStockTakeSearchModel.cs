namespace Application.DTOs.Request
{
    public class InventStockTakeSearchModel
    {
        public string? StockTakeNo { get; set; }
        public DateOnly? StockTakeFrom { get; set; }
        public DateOnly? StockTakeTo { get; set; }
        public string? Location { get; set; }
        public string? ProductCode { get; set; }
        public int? Tenant { get; set; }
        public EnumInventStockTakeStatus? Status { get; set; }

        public bool AreNull()
        {
            return StockTakeNo == null &&
                   StockTakeFrom == null &&
                   StockTakeTo == null &&
                   Location == null &&
                   ProductCode == null &&
                   Tenant == null &&
                   Status == null;
        }
    }
}
