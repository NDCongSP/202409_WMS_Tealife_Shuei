

namespace Application.DTOs.Response.Account
{
    public class GetUserWithRoleResponseDTO
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public EnumStatus? Status { get; set; }
        public List<GetRoleResponseDTO> Roles { get; set; } = new List<GetRoleResponseDTO>();
        //public string? RoleName { get; set; }
        //public string? RoleId { get; set; }
    }
}
