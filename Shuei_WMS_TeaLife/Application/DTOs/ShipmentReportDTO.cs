using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ShipmentReportDTO
    {
        public string TenantName { get; set; }
        public string ShipmentType { get; set; }
        public string? PackedQty { get; set; }
        public int? RemainingQty { get; set; }
        public double ProgressRate { get; set; }
        public string? ProgressRateString { get; set; }
        public string? Productivity { get; set; }
        public int RemainingIdentificationNumber { get; set; }
        public int totalPackedQty { get; set; }
        public int totalShipmentQty { get; set; }
    }
}
