namespace Application.DTOs.Request.Account
{
    public class CreateRoleRequestDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public List<Permissions> Permissions { get; set; } = new List<Permissions>();
    }
}
