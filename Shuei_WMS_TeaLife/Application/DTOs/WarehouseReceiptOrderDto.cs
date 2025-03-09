using Mapster;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderDto : GenericEntity
    {
        public WarehouseReceiptOrderDto()
        {
        }
        public WarehouseReceiptOrderDto(WarehouseReceiptOrder receipt, List<WarehouseReceiptOrderLine> lines)
        {
            Id = receipt.Id;
            ReceiptNo = receipt.ReceiptNo;
            Location = receipt.Location;
            ExpectedDate = receipt.ExpectedDate;
            TenantId = receipt.TenantId;
            ScheduledArrivalNumber = receipt.ScheduledArrivalNumber;
            DocumentNo = receipt.DocumentNo;
            SupplierId = receipt.SupplierId;
            PersonInCharge = receipt.PersonInCharge;
            ConfirmedBy = receipt.ConfirmedBy;
            ConfirmedDate = receipt.ConfirmedDate;
            Status = receipt.Status;
            ReferenceType = receipt.ReferenceType;
            ReferenceNo = receipt.ReferenceNo;
            WarehouseReceiptOrderLines = lines.Adapt<List<WarehouseReceiptOrderLineDto>>();
        }
        public WarehouseReceiptOrderDto(WarehouseReceiptOrder receipt, List<WarehouseReceiptOrderLineDto> lines)
        {
            Id = receipt.Id;
            ReceiptNo = receipt.ReceiptNo;
            Location = receipt.Location;
            ExpectedDate = receipt.ExpectedDate;
            TenantId = receipt.TenantId;
            ScheduledArrivalNumber = receipt.ScheduledArrivalNumber;
            DocumentNo = receipt.DocumentNo;
            SupplierId = receipt.SupplierId;
            PersonInCharge = receipt.PersonInCharge;
            ConfirmedBy = receipt.ConfirmedBy;
            ConfirmedDate = receipt.ConfirmedDate;
            Status = receipt.Status;
            ReferenceType = receipt.ReferenceType;
            ReferenceNo = receipt.ReferenceNo;
            WarehouseReceiptOrderLines = lines;
        }

        public Guid Id { get; set; }

        public string? ReceiptNo { get; set; } = string.Empty;


        public string? Location { get; set; }

        public DateOnly? ExpectedDate { get; set; }


        public int TenantId { get; set; }
        public double? ScheduledArrivalNumber { get; set; }

        public string? DocumentNo { get; set; }

        public int SupplierId { get; set; }


        public string? PersonInCharge { get; set; }

        public string? ConfirmedBy { get; set; }

        public DateOnly? ConfirmedDate { get; set; }

        public List<WarehouseReceiptOrderLineDto> WarehouseReceiptOrderLines { get; set; } = new();
        public string? LocationName { get; set; }
        public string? TenantFullName { get; set; }
        public string? SupplierName { get; set; }
        public string? PersonInChargeName { get; set; }
        public EnumReceiptOrderStatus? Status { get; set; } = EnumReceiptOrderStatus.Draft;
        public EnumWarehouseTransType ReferenceType { get; set; } = EnumWarehouseTransType.Receipt;
        public string? ReferenceNo { get; set; }
    }
}
