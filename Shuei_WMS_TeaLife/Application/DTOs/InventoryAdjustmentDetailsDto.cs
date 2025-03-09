using Mapster;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class InventoryAdjustmentDetailsDto
    {
      
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public bool PerSet { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public double? StockQuantity { get; set; }
        public double AdjustmentQuantity { get; set; }
        public double FinalStockQuantity { get; set; }
        public string? Reasons { get; set; }
        public string? LotNo { get; set; }
        public EnumProductType? ProductType { get; set; }

    }
}
