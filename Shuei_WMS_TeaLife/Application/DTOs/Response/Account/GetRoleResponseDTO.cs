using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response.Account
{
    public class GetRoleResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Permissions> Permissions { get; set; } = new List<Permissions>();
    }
}
