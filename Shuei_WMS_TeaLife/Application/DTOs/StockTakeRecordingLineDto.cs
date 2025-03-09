namespace Application.DTOs
{
    public class StockTakeRecordingLineDto : GenericEntity
    {
        public Guid Id { get; set; }
        public Guid StockTakeRecordingId { get; set; }
        public string StockTakeNo { get; set; }
        public int RecordNo { get; set; }
        public string ProductCode { get; set; }
        public string Bin { get; set; }
        public string LotNo { get; set; }
        public double? ExpectedQty { get; set; }
        public double? ActualQty { get; set; }
        public int? UnitId { get; set; }
        public EnumInvenTransferStatus Status { get; set; } = EnumInvenTransferStatus.InProcess;
        public int? TenantId { get; set; }

        public StockTakeRecordingLineDto()
        {
        }

        public StockTakeRecordingLineDto(InventStockTakeRecordingLine line, int? tenantId)
        {
            Id = line.Id;
            StockTakeRecordingId = line.StockTakeRecordingId;
            StockTakeNo = line.StockTakeNo;
            RecordNo = line.RecordNo;
            ProductCode = line.ProductCode;
            Bin = line.Bin;
            LotNo = line.LotNo;
            ExpectedQty = line.ExpectedQty;
            ActualQty = line.ActualQty;
            UnitId = line.UnitId;
            Status = line.Status;
            TenantId = tenantId;
        }
    }
}
