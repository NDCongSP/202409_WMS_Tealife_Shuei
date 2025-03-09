namespace Application.DTOs.Request
{
    public class WarehouseReceiptSearchModel
    {
        public string? ReceiptNo { get; set; }
        public string? ReferenceNo { get; set; }
        public int? TenantId { get; set; }
        public int? SupplierId { get; set; }
        public int? ScheduledArrivalNumber { get; set; }
        public DateOnly? ExpectedDate { get; set; } //range-datepicker
        public Guid Location { get; set; }
        public string? ProductCode { get; set; }
        public int? ArrivalNo { get; set; }
        public EnumReceiptOrderStatus? Status { get; set; }
    }
}
