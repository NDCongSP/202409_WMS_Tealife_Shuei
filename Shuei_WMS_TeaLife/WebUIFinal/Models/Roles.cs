

namespace WebUIFinal
{
    public class Roles
    {
        public string Name { get; set; }
        public List<RoleToPermission> Permissions { get; set; } = new List<RoleToPermission>();
    }
}
