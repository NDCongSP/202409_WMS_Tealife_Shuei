using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models;

namespace Application.DTOs
{
    public class InventoryInfoBinLotResponseDTO
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public List<InventoryInfoBinLotDetails> Details { get; set; }
    }
}
