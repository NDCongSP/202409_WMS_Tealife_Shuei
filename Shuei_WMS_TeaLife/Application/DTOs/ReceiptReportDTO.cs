using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ReceiptReportDTO
    {
        public DateOnly Period { get; set; }
        public string Tenant { get; set; }
        public string? NumberPutaway { get; set; }
        public string? RemainingNumber { get; set; }
        public decimal ProgressRate { get; set; }
        public string? ProgressRateString { get; set; }
        public string? Productivity { get; set; }
        public int TotalOrderQty { get; set; }
        public int TotalTransQty { get; set; }
        public int TotalRemainingNumber { get; set; }
    }
}
