namespace Application.DTOs.Transfer
{
    public class PickingSlipInfos
    {
        public WarehousePickingDTO PickInfo { get; set; } = new WarehousePickingDTO();
        public string? Barcode { get; set; }
        public List<WarehousePickingShipmentDTO> ShipmentInfos { get; set; } = new List<WarehousePickingShipmentDTO>();
        public List<WarehousePickingLineDTO> PickLineInfos { get; set; } = new List<WarehousePickingLineDTO>();
        public PickingSlipInfos() { }
        public PickingSlipInfos(WarehousePickingDTO pickInfo, List<WarehousePickingShipmentDTO> shipmentInfos, List<WarehousePickingLineDTO> pickLineInfos, string barcode)
        {
            PickInfo = pickInfo;
            ShipmentInfos = shipmentInfos;
            PickLineInfos = pickLineInfos;
            Barcode = barcode;
        }
    }
}
