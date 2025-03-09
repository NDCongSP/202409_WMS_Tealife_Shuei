


using RestEase.Implementation;

namespace Application.DTOs
{
    public class WarehousePutAwayDto : GenericEntity
    {
        public Guid Id { get; set; }

        public string PutAwayNo { get; set; } = string.Empty;
        public string ReceiptNo { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int TenantId { get; set; }

        public DateOnly? TransDate { get; set; }

        public DateOnly? DocumentDate { get; set; }

        public string? DocumentNo { get; set; }

        public string Location { get; set; }

        public DateOnly? PostedDate { get; set; }

        public string? PostedBy { get; set; }

        public List<WarehousePutAwayLineDto> WarehousePutAwayLines { get; set; }
        
        public EnumPutAwayStatus Status { get; set; } = EnumPutAwayStatus.PutAway;
        public EnumHHTStatus HHTStatus { get; set; } = EnumHHTStatus.New;
        public string? HHTInfo { get; set; } = string.Empty;
        public EnumWarehouseTransType ReferenceType { get; set; } = EnumWarehouseTransType.Receipt;
        public string? ReferenceNo { get; set; }

        public WarehousePutAwayDto()
        {
        }

        public WarehousePutAwayDto(WarehouseReceiptOrderDto receipt)
        {
            Id = receipt.Id;
            ReceiptNo = receipt.ReceiptNo;
            TenantId = receipt.TenantId;
            DocumentNo = receipt.DocumentNo;
            Location = receipt.Location;
            WarehousePutAwayLines = receipt.WarehouseReceiptOrderLines.Select(r => new WarehousePutAwayLineDto
            {
                Id = r.Id,
                ProductCode = r.ProductCode,
                UnitId = (int)r.UnitId,
                JournalQty = r.OrderQty,
                TransQty = r.TransQty,
                Bin = r.Bin,
                LotNo = r.LotNo
            }).ToList();
        }
    }
}
