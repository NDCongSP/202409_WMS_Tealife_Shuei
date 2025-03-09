namespace Application.DTOs.Request.Account
{
    public class AssignUserRoleRequestDTO
    {
        public string UserName { get; set; }
        public List<CreateRoleRequestDTO> Roles { get; set; }
    }
}
