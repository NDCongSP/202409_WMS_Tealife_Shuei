
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Transfer
{
    public class StockTakeRecordingSlipInfos
    {
        public InventStockTakeRecordingDTO StockTakeRecording { get; set; } = new InventStockTakeRecordingDTO();
        public List<InventStockTakeRecordingLineDtos> StockTakeRecordingLines { get; set; } = new List<InventStockTakeRecordingLineDtos>();

        public DateOnly PrintDate = DateOnly.FromDateTime(DateTime.Now);
        public StockTakeRecordingSlipInfos(InventStockTakeRecordingDTO stockTakeRecording, List<InventStockTakeRecordingLineDtos> stockTakeRecordingLines)
        {
            StockTakeRecording = stockTakeRecording;
            StockTakeRecordingLines = stockTakeRecordingLines;
        }
    }
}
