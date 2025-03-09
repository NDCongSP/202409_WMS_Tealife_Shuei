namespace Application.DTOs.Request
{
    public class ReceivePlanSearchModel
    {
        public int? SupplierId { get; set; }
        public string? ProductCode { get; set; }
        public int? ArrivalNo { get; set; }
        public DateTime? ReceiveDateFrom { get; set; }
        public DateTime? ReceiveDateTo { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
