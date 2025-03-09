using Domain.Entity.WMS.Authentication;

namespace WebUI
{
    public class Roles
    {
        public string Name { get; set; }
        public List<RoleToPermission> Permissions { get; set; } = new List<RoleToPermission>();
    }
}
