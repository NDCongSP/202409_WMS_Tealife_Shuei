using FBT.ShareModels.WMS;
using Mapster;

namespace Application.DTOs
{
    public class InventStockTakeDto
    {
        public Guid Id { get; set; }
        public string? StockTakeNo { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? TransactionDate { get; set; }
        public EnumInventStockTakeStatus Status { get; set; } = EnumInventStockTakeStatus.Open;
        public string? PersonInCharge { get; set; }
        public int TenantId { get; set; }
        public string? CreateOperatorId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UpdateOperatorId { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public List<InventStockTakeLineDto> InventStockTakeLines { get; set; } = new();

        public string? PersonInChargeName { get; set; }
        public string? LocationName { get; set; }
        public string? TenantFullName { get; set; }

        public InventStockTakeDto()
        {
        }

        public InventStockTakeDto(InventStockTake StockTake, string picName, string locationName, string tenantFullName, List<InventStockTakeLineDto> lines)
        {
            Id = StockTake.Id;
            StockTakeNo = StockTake.StockTakeNo;
            Description = StockTake.Description;
            Location = StockTake.Location;
            LocationName = locationName;
            TenantId = StockTake.TenantId;
            TenantFullName = tenantFullName;
            TransactionDate = StockTake.TransactionDate;
            Status = StockTake.Status;
            PersonInCharge = StockTake.PersonInCharge;
            CreateOperatorId = StockTake.CreateOperatorId;
            CreateAt = StockTake.CreateAt;
            UpdateOperatorId = StockTake.UpdateOperatorId;
            UpdateAt = StockTake.UpdateAt;
            IsDeleted = StockTake.IsDeleted;
            PersonInChargeName = picName;
            InventStockTakeLines = lines;
        }

        public InventStockTakeDto(InventStockTake StockTake)
        {
            Id = StockTake.Id;
            StockTakeNo = StockTake.StockTakeNo;
            Description = StockTake.Description;
            Location = StockTake.Location;
            TenantId = StockTake.TenantId;
            TransactionDate = StockTake.TransactionDate;
            Status = StockTake.Status;
            PersonInCharge = StockTake.PersonInCharge;
            CreateOperatorId = StockTake.CreateOperatorId;
            CreateAt = StockTake.CreateAt;
            UpdateOperatorId = StockTake.UpdateOperatorId;
            UpdateAt = StockTake.UpdateAt;
            IsDeleted = StockTake.IsDeleted;
            InventStockTakeLines = [];
        }
    }

    public class InventStockTakeLineDto
    {
        public Guid Id { get; set; }
        public string? StockTakeNo { get; set; }
        public string? ProductCode { get; set; }
        public double? ExpectedQty { get; set; }
        public double? ActualQty { get; set; }
        public int? UnitId { get; set; }
        public EnumInventStockTakeStatus Status { get; set; } = EnumInventStockTakeStatus.Open;
        public string? CreateOperatorId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UpdateOperatorId { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public string? ProductName { get; set; }
        public string? UnitName { get; set; }
        public double QtyDifference
        {
            get
            {
                if (ExpectedQty.HasValue && ActualQty.HasValue)
                {
                    return ExpectedQty.Value - ActualQty.Value;
                }
                return 0;
            }
        }
        public string? Location { get; set; }
        public int? TenantId { get; set; }
        public InventStockTakeLineDto()
        {
        }
        public InventStockTakeLineDto(InventStockTakeLine StockTakeLine, string productName, string unitName)
        {
            Id = StockTakeLine.Id;
            StockTakeNo = StockTakeLine.StockTakeNo;
            ProductCode = StockTakeLine.ProductCode;
            ProductName = productName;
            ExpectedQty = StockTakeLine.ExpectedQty;
            ActualQty = StockTakeLine.ActualQty;
            UnitId = StockTakeLine.UnitId;
            UnitName = unitName;
            Status = StockTakeLine.Status;
            CreateOperatorId = StockTakeLine.CreateOperatorId;
            CreateAt = StockTakeLine.CreateAt;
            UpdateOperatorId = StockTakeLine.UpdateOperatorId;
            UpdateAt = StockTakeLine.UpdateAt;
        }
    }
}
