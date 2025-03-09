using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class InventoryInfoBinLotSearchRequestDTO
    {
        /// <summary>
        /// TenantId.
        /// </summary>
        public int CompanyId { get; set; } = 0;
        public List<ProductCodeSearch> ProductCodes { get; set; }

    }

    public class ProductCodeSearch
    {
        public string ProductCode { get; set; }
    }
}
