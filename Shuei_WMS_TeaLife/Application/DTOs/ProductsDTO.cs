using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductsDTO : Product
    {
        public int? InventoryStock { get; set; } = 0;

        public int? OnOrder { get; set; } = 0;
        public int? InventoryAvailable { get; set; } = 0;
        /// <summary>
        /// For slip delivery printing.
        /// For Philippines team use that field to print when linking with shipping company.
        /// </summary>
        public string? SlipDeliveryPrinting { get; set; }
        public string? UnitName { get; set; }
        public string? SupplierName { get; set; }
        public string? CategoryName { get; set; }
        public string? TenantName { get; set; }

        [NotMapped]
        public ImageStorage ImageStorage { get; set; } = new ImageStorage();
        [NotMapped]
        public List<ProductJanCode> ProductJanCode { get; set; } = new List<ProductJanCode>();
    }
}
