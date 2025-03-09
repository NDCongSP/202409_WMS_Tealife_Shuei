

using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderLineDto : GenericEntity
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        public string ReceiptNo { get; set; } = string.Empty;

        public string ProductCode { get; set; }

        public string? UnitName { get; set; }

        public double? OrderQty { get; set; }

        public double? TransQty { get; set; }

        public string? Bin { get; set; }

        public string? LotNo { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public bool? Putaway { get; set; }

        public int UnitId { get; set; }
        public string? ProductName { get; set; }
        public int? StockAvailableQuantity { get; set; }
        public int? NumberOfPossibleUses { get; set; }
        public int? ShelvedNumber { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
        public double? ArrivalNo { get; set; }

        public Guid? ReceiptLineIdParent { get;set; }
    }
}
