using Mapster;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class InventoryHistoryDto
    {
        public InventoryHistoryDto()
        {
        }


        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        /// <summary>
        /// Quantity from table Product
        /// </summary>
        public int? StockAvailableQuantity { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public EnumProductStatus? ProductStatus { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string? LocationName { get; set; }
        public string? BinCode { get; set; }
        public string? Lot { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? BinId { get; set; }

        public bool TotalRow { get; set; }
        public DateOnly? ExpirationDate { get; set; }


        /// <summary>
        /// Quantity from wh trans
        /// </summary>
        public int StockAvailableQuantityTrans { get; set; }

        public double Quantity { get; set; }
        public int SupplierId { get; set; }

        public int TenantId { get; set; }

        public string? TenantName { get; set; }

    }
}
