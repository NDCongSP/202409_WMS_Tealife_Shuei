using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class InventoryInfoBinLot : Product
    {
        public string? LotNo { get; set; }
        public string? LocationName { get; set; }
        public string? BinCode { get; set; }
        public DateOnly? Expired { get; set; }

        public double InventoryStock { get; set; } = 0;
        public double OnOrder { get; set; } = 0;
        public double AvailableStock { get; set; } = 0;
        /// <summary>
        /// TenantId in warehouseTran table.
        /// </summary>
        public int? TenantId { get; set; }
    }
}
