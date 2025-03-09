using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class InventStockTakeSearchRequestDTO
    {
        public string StockTakeNo { get; set; } = null;
        public int RecordNo { get; set; } = 0;
    }
}
