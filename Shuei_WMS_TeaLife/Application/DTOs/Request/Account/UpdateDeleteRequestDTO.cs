using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.Account
{
    /// <summary>
    /// Model dùng dề truyền vào ID để update hay delete row trong bảng.
    /// </summary>
    public class UpdateDeleteRequestDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
