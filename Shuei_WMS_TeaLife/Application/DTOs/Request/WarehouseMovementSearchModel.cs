using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class WarehouseMovementSearchModel
    {
        public string? MovementNo { get; set; }
        public Guid OutboundLocationId { get; set; }
        public DateOnly? EstimateShipDateFrom { get; set; }
        public DateOnly? EstimateShipDateTo { get; set; }
        public Guid InboundLocationId { get; set; }
        public DateOnly? ExpectedReceiveDateFrom { get; set; }
        public DateOnly? ExpectedReceiveDateTo { get; set; }
        public EnumShipmentOrderStatus? Status { get; set; }
    }
}
