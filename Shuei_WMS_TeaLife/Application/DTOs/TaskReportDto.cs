namespace Application.DTOs
{
    public class TaskReportDto
    {
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public int Responsibility { get; set; }
        public int TaskIsOnHold { get; set; }
    }
}
