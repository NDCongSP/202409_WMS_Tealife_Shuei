using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class ImageStorageSearchRequestDTO
    {
        public List<string> ResourceId { get; set; }
        public EnumImageStorageType Type { get; set; }
    }
}
