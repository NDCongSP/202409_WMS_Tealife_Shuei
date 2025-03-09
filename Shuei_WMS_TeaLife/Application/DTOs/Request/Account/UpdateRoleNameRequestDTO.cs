using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.Account
{
    public class UpdateRoleNameRequestDTO
    {
        public string CurrentRoleName { get; set; }
        public string NewRoleName { get; set; }
    }
}
