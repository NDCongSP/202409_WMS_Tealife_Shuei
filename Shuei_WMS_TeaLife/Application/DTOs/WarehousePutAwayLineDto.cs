


namespace Application.DTOs
{
    public class WarehousePutAwayLineDto : GenericEntity
    {
        public WarehousePutAwayLineDto()
        {
        }

        public WarehousePutAwayLineDto(WarehousePutAwayLineDto p, double? journalQty, double? transQty, string bin)
        {
            Id = p.Id;
            PutAwayNo = p.PutAwayNo;
            ProductCode = p.ProductCode;
            UnitId = p.UnitId;
            JournalQty = journalQty;
            TransQty = transQty;
            Bin = bin;
            LotNo = p.LotNo;
            ExpirationDate = p.ExpirationDate;
            TenantId = p.TenantId;
            Status = p.Status;
        }

        public WarehousePutAwayLineDto(WarehouseReceiptOrderLineDto r)
        {
            Id = r.Id;
            ReceiptNo = r.ReceiptNo;
            ProductCode = r.ProductCode;
            UnitId = r.UnitId;
            JournalQty = r.OrderQty;
            TransQty = r.TransQty;
            Bin = r.Bin;
            LotNo = r.LotNo;
        }

        public Guid Id { get; set; }
        public string ReceiptNo { get; } = string.Empty;
        public string PutAwayNo { get; set; } = string.Empty;

        public string ProductCode { get; set; }
        public string ProductShortCode { get; set; }

        public int? UnitId { get; set; }

        public string? UnitName { get; set; }
        public string? ProductName { get; set; }
        public List<string>? ProductJanCodes { get; set; }

        public double? JournalQty { get; set; }

        public double? TransQty { get; set; }

        public string? Bin { get; set; }

        public string? LotNo { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public int TenantId { get; set; }

        public EnumStatus Status { get; set; } = EnumStatus.Activated;

        public Guid? ReceiptLineId { get; set; }
        public string? ProductUrl { get; set; }
    }
}
