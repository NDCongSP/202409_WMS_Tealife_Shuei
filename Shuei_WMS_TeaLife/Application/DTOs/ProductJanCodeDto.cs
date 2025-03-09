using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductJanCodeDto : ProductJanCode
    {
        public bool IsDelete { get; set; } = false;
    }
}
