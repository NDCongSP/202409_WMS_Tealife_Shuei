using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class InventoryInfoBinLotDetails
    {
        /// <summary>
        /// TenantId.
        /// </summary>
        public int? CompanyId { get; set; }
        public string? WarehouseCode { get; set; }
        public string? BinCode { get; set; }
        public string? LotNo { get; set; }
        public DateOnly? Expired { get; set; }

        public double InventoryStock { get; set; } = 0;
        public double OnOrder { get; set; } = 0;
        public double AvailableStock { get; set; } = 0;
    }
}
