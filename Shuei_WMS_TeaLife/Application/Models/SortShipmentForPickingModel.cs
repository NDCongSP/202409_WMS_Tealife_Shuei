using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{

    /// <summary>
    /// model get data from store procedure.
    /// </summary>
    public class SortShipmentForPickingModel : WarehouseShipmentLine
    {
        public Guid? ShipmentId { get; set; }
        public string? SalesNo { get; set; }
        public string? TrackingNo { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string ShippingCarrierCode { get; set; }
        public DateOnly? PlanShipDate { get; set; }
        public string LocationName { get; set; }
        public Guid? BinId { get; set; }
        public string BinCode { get; set; }
        public int SortOrderNum { get; set; }
        /// <summary>
        /// S123,S134,...
        /// </summary>
        public string? BinJontNumber { get; set; }
        public int? SortedNum { get; set; }
    }

    public class ShipmentForPickingGroupShipmentCodeModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; }
        public string ShippingCarrierCode { get; set; }
        public int TenantId { get; set; }
        public DateOnly? PlanShipDate { get; set; }
        public string LocationName { get; set; }
        public string? BinJontNumber { get; set; }
        public int? SortedNum { get; set; }
        public List<SortShipmentForPickingModel> ShipmentLines { get; set; }
    }

    public class PickingDataFinal
    {
        public int TenantId { get; set; }
        public string ShippingCarrierCode { get; set; }
        /// <summary>
        /// Contains  List<PickingDataGroupShipmentCodeMode>.
        /// </summary>
        public List<ShipmentForPickingGroupShipmentCodeModel> Shipments { get; set; }
    }
}
