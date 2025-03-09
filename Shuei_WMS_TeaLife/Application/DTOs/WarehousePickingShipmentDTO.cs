

namespace Application.DTOs
{
    public class WarehousePickingShipmentDTO
    {
        public Guid Id { get; set; }
        public string OrderNo { get; set; }
        public string ShipmentNo { get; set; }
        public double TotalQuantity { get; set; }
        public string TenantFullName { get; set; }
        public string OrderDeliveryCompany { get; set; }
        public DateOnly? OrderDate { get; set; }
        public DateOnly? PlanShipDate { get; set; }
    }
}