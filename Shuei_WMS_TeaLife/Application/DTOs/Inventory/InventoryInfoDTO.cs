using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class InventoryInfoDTO : Product
    {
        public double InventoryStock { get; set; } = 0;
        public double OnOrder { get; set; } = 0;
        public double InventoryAvailable { get; set; } = 0;
    }
}
