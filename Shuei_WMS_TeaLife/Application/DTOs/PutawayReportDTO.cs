using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PutawayReportDTO
    {

        public DateOnly Period { get; set; }
        public string Tenant { get; set; }
        public string? ExpectedStock { get; set; }
        public string? RemainingNumber { get; set; }
        public double ProgressRate { get; set; }
        [NotMapped] public string? ProgressRateString { get; set; }
        public double? Productivity { get; set; } = 0;
        public int TotalJournalQty { get; set; }
        public int TotalTransQty { get; set; }
        public int TotalRemainingNumber { get; set; }
        [NotMapped] public string TotalJournalQtyOpen { get; set; }
        [NotMapped] public string TotalTransQtyOpen { get; set; }
        [NotMapped] public string TotalRemainingNumberOpen { get; set; }
        [NotMapped] public string TotalJournalQtyComplete { get; set; }
        [NotMapped] public string TotalTransQtyComplete { get; set; }
        [NotMapped] public string TotalRemainingNumberComplete { get; set; }
    }
}
