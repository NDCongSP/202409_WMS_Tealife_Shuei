using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PermissionsInRoleModel : Permissions
    {
        /// <summary>
        /// dùng để hiển thị ở bước tạo Role, chọn permission.
        /// </summary>
        public bool IsSelected { get; set; } = false;
    }
}
