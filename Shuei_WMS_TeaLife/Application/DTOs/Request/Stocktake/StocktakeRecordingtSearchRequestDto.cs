
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.StockTake
{
    public class StockTakeRecordingSearchRequestDto
    {
        public string? StockTakeNo { get; set; } = null;
        public DateOnly? TransactionDateFrom { get; set; } = null;
        public DateOnly? TransactionDateTo { get; set; } = null;
        public string? Location { get; set; } = null;
        public int? Tenant {  get; set; } = null;
        public string? Bin { get; set; } = null;
        public EnumInvenTransferStatus? Status { get; set; } = null;
    }
}
