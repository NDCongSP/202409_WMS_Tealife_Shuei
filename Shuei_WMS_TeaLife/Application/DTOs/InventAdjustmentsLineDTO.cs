namespace Application.DTOs
{
    public class InventAdjustmentsLineDTO : InventAdjustmentLine
    {
        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public double? FinalQty { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        //public bool PerSet { get; set; }
        public double? StockAvailable { get; set; } 
    }
}
