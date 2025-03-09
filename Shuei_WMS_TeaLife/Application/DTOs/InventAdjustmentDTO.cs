using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class InventAdjustmentDTO : InventAdjustment
    {
        public List<InventAdjustmentsLineDTO> InventAdjustmentLines { get; set; }
        public string? LocationName { get; set; }
        public string? BinCode { get; set; }
    }
}
