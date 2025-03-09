
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.shipment
{
    public class PackingListUpdatePackedQtyRequestDto
    {
        /// <summary>
        /// Shipment line id.
        /// </summary>
        public Guid Id { get; set; }
        public string? ShipmentNo { get; set; }
        public double? PackedQty { get; set; }

        public DateOnly? PackedDate { get; set; }
        public EnumShipmentOrderStatus? StatusShipment { get; set; }
        public EnumStatusIssue? StatusIssueWhTran { get; set; }
        public Guid? ShippingBoxesId { get; set; }
        public string? ShippingBoxesName { get; set; }
    }
}
