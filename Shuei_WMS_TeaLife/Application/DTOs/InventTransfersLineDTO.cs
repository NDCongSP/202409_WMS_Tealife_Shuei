namespace Application.DTOs
{
    public class InventTransfersLineDTO : GenericEntity
    {
        public InventTransfersLineDTO()
        {
        }

        public InventTransfersLineDTO(InventTransferLine line, Product product, Unit unit)
        {
            Id = line.Id;
            InventTransferId = line.InventTransferId;
            TransferNo = line.TransferNo;
            Qty = line.Qty;
            UnitId = line.UnitId;
            Status = line.Status;
            FromBin = line.FromBin;
            ToBin = line.ToBin;
            FromLotNo = line.FromLotNo;
            ToLotNo = line.ToLotNo;
            ProductCode = line.ProductCode;
            ProductName = product.ProductName;
            StockAvailable = product.StockAvailableQuanitty;
            
            UnitName = unit.UnitName;
        }

        public Guid Id { get; set; }
        public Guid InventTransferId { get; set; }
        public string TransferNo { get; set; }
        public double? Qty { get; set; } = 0;
        public double? StockAvailable { get; set; }
        public double? AvailableQuantity { get; set; }
        public int UnitId { get; set; }
        public EnumInvenTransferStatus Status { get; set; } = EnumInvenTransferStatus.InProcess;
        public string FromBin { get; set; }
        public string ToBin { get; set; }
        public string FromLotNo { get; set; }
        public string ToLotNo { get; set; }
        public string ProductCode { get; set; } 
        public string ProductName { get; set; }

        public string? UnitName { get; set; }
    }
}
