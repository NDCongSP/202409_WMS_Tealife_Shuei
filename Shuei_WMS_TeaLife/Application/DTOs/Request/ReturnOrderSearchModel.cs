namespace Application.DTOs.Request
{
    public class ReturnOrderSearchModel
    {
        public DateOnly? ReturnOrderFrom { get; set; }
        public DateOnly? ReturnOrderTo { get; set; }
        public EnumReturnOrderStatus? Status { get; set; }
        public string? ReceiptNo { get; set; }

        public string? ShipmentNo { get; set; }
        public string? ReturnOrderNo { get; set; }
    }
}
