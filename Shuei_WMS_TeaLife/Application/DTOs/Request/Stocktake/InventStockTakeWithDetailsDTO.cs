﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request.StockTake
{
    public class InventStockTakeWithDetailsDTO
    {
        public InventStockTakeDto InventStockTakeDto { get; set; }
        public List<InventStockTakeLineDto> InventStockTakeLineDto { get; set; }
    }
}
