using Mapster;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class InventoryHistoryDetailsDto
    {
        public InventoryHistoryDetailsDto()
        {
        }


        public int ProductId { get; set; }

        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string? LocationName { get; set; }
        public string? LocationId { get; set; }
        public string? TenantName { get; set; }
        public int? TenantId { get; set; }
        public string? BinCode { get; set; }

        public string? LotNo { get; set; }
        public double Quantity { get; set; }
        public int SupplierId { get; set; }
        public string TransNumber { get; set; }
        public EnumWarehouseTransType TransType { get; set; }
        public DateOnly? DatePhysical { get; set; }
        public DateTime? CreateAt { get;set; }
        public DateOnly? ExpirationDate { get; set; }
    }
}
