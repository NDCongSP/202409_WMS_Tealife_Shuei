
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.shipment
{
    public class PackingListSearchRequestDto
    {
        /// <summary>
        /// Instruction number
        /// </summary>
        public string? InstructionNumber { get; set; } = null;
        public string? ScheduledShipDateFrom { get; set; } = null;
        public string? ScheduledShipDateTo { get; set; } = null;
        /// <summary>
        /// Select location.
        /// </summary>
        public string? DeliveryLocation { get; set; } = null;
        /// <summary>
        /// Select Bin.
        /// </summary>
        public string? OutgoingBin { get; set; } = null;
        public string? TrackingNo { get; set; } = null;
        public EnumPackingListStatus? Status { get; set; } = null;
    }
}
