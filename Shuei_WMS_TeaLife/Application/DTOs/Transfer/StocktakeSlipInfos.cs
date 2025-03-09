
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transfer
{
    public class StockTakeSlipInfos
    {
        public InventStockTakeDto StockTakeDto { get; set; } = new InventStockTakeDto();
        public List<InventStockTakeLineDto> StockTakeLineDto { get; set; } = new List<InventStockTakeLineDto>();

        public DateOnly PrintDate = DateOnly.FromDateTime(DateTime.Now);
        public StockTakeSlipInfos(InventStockTakeDto stockTakeDto, List<InventStockTakeLineDto> stockTakeLineDto)
        {
            StockTakeDto = stockTakeDto;
            StockTakeLineDto = stockTakeLineDto;
        }
    }
}
