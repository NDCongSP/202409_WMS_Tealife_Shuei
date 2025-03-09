namespace Application.DTOs
{
    public class OrderSelectList
    {
        public string OrderId { get; set; } = default!;
        public string? TrackingNo { get; set; }
        public int OrderDispatchId { get; set; }
    }
}
