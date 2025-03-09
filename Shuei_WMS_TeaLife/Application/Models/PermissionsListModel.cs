

namespace Application.Models
{
    public class PermissionsListModel : Permissions
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
