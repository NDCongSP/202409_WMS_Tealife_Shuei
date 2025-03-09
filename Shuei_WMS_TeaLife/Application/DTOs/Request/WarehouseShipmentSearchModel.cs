

namespace Application.DTOs.Request
{
    public class WarehouseShipmentSearchModel
    {
        public EnumWarehouseTransType Type { get; set; } = 0;
        public string? ShipmentNo { get; set; }
        public string? SalesNo { get; set; }
        public int? TenantId { get; set; }
        public Guid OutboundLocationId { get; set; }
        public DateOnly? EstimateShipDateFrom { get; set; }
        public DateOnly? EstimateShipDateTo { get; set; }
        public string? BinId { get; set; }

        #region MOVEMENT
        public Guid InboundLocationId { get; set; }
        public DateOnly? ExpectedReceiveDateFrom { get; set; }
        public DateOnly? ExpectedReceiveDateTo { get; set; }
        #endregion

        public EnumShipmentOrderStatus? Status { get; set; }
    }
}
