

namespace Application.DTOs
{
    public class WarehousePickingLineDTO
    {
        public Guid Id { get; set; }
        public string PickNo { get; set; }
        public string ShipmentNo { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string Unit { get; set; }
        public string Lot { get; set; }
        public string Bin { get; set; }
        public double? InstructionsNumber { get; set; }
        public double? PickQty { get; set; }
        public double? ActualQty { get; set; }
        public double? Remaining { get; set; }
        public DateOnly ExpirationDate { get; set; }
    }
}