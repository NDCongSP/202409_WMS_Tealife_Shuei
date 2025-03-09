using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductBundleDto
    {
       
        public string SaleProductBundleCode { get; set; }

        public string ProductBundleCode { get; set; }
        public string ProductBundleSetName { get; set; }
        public string ProductCode { get; set; }

        
        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        public int SequenceNo { get; set; }
        public double? DemandQty { get; set; }
        public double? AvailableQty { get; set; }
        public double? OrderStatusQty { get; set; }

        public double? StockUpStatusQty { get; set; }

        public double? OpenShipmentQty { get; set; }
        public ProductBundleDto() { }
    }
}
