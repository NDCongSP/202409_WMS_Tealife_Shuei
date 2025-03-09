
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.Picking
{
    public class PickingListSearchRequestDto
    {
        /// <summary>
        /// Instruction number
        /// </summary>
        public string? PickingNo { get; set; } = null;
        public string? ShipmentNumber { get; set; } = null;
        public DateOnly? PlanShipDateFrom { get; set; } = null;
        public DateOnly? PlanShipDateTo { get; set; } = null;
        /// <summary>
        /// Select location.
        /// </summary>
        public string? Location { get; set; } = null;
        /// <summary>
        /// Select Bin.
        /// </summary>
        public string? Bin { get; set; } = null;
        public EnumShipmentOrderStatus? Status { get; set; } = null;
    }
}
