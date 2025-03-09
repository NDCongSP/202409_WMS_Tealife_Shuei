using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PackingListGenerateRow
    {
        public Guid ShipmentLineId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ExpireDate { get; set; }
        public string LotNo { get; set; }
        /// <summary>
        /// index/TotalOrderQty.
        /// 1/5, 2/5,3/5,4/5,5/5.
        /// </summary>
        public string ShipmentQty { get; set; } 
        public int Qty { get; set; } = 1;

        /// <summary>
        /// khi scanner scan bacodeb, then set ='1'.
        /// </summary>
        public string Scanned { get; set; }
        /// <summary>
        /// dung de hien thi mau ne cho dong dau tien, mau xanh.
        /// </summary>
        public int Index { get; set; }
    }
}
