namespace Application.DTOs
{
    public class OrderReportDto
    {
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public string? Status { get; set; }
        public int QtyOrderedIsPending { get; set; }
    }
}
