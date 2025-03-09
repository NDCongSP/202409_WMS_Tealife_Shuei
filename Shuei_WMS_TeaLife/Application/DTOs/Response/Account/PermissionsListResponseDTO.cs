namespace Application.DTOs.Response.Account
{
    public class PermissionsListResponseDTO : Permissions
    {
        public List<GetRoleResponseDTO> AssignedToRoles { get; set; } = new List<GetRoleResponseDTO>();
    }
}
