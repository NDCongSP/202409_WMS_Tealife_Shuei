namespace Application.DTOs.Request
{
    public class HhtUpdateStatusRequestDTO
    {
        public EnumEntitiesName EntityName { get; set; }
        public Guid Id { get; set; }
        public EnumHHTStatus HHTStatus { get; set; }
        public string? HHTInfo { get; set; }
    }
}
