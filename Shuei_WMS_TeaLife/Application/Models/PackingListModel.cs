
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class PackingListModel
    {
        /// <summary>
        /// ShipingLineId.
        /// </summary>
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; }

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double ShipmentQty { get; set; } = 0;
        public string LocationName { get; set; }
        public string? Location { get; set; }
        public string Bin { get; set; }
        public string? LotNo { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? CreateOperatorId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UpdateOperatorId { get; set; }
        public DateTime? UpdateAt { get; set; }

        /// <summary>
        /// shipment line status.
        /// </summary>
        public EnumPackingListStatus Status { get; set; } = EnumPackingListStatus.Packing;//shipment line status
        /// <summary>
        /// shipment status.
        /// </summary>
        public EnumPackingListStatus StatusOfShipment { get; set; } = EnumPackingListStatus.Packing;//Shipment status
        public double? PackedQty { get; set; } = 0;
        public DateTime? PackedDate { get; set; }
        public Guid IdShipment { get; set; }
        /// <summary>
        /// OrderNo.
        /// </summary>        
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public DateTime PlanShipDate { get; set; }
        public string? CreateOperatorIdOfShipment { get; set; }
        public DateTime? CreateAtOfShipment { get; set; }
        public string? UpdateOperatorIdOfShipment { get; set; }
        public DateTime? UpdateAtOfShipment { get; set; }
        public string? PersonInCharge { get; set; }
        public string? PersonInChargeName { get; set; }
        public string? ShippingCarrierCode { get; set; }
        public Guid? ShippingBoxesId { get; set; }
        public string? ShippingBoxesName { get; set; }
        public string? PrinterName { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Telephone { get; set; }
        public string? SalesNo { get; set; }
        public string? TrackingNo { get; set; }
        public string? LabelFilePath { get; set; }
        public string? LabelFileExtension { get; set; }
        public string? Email { get; set; }
        public string? BinId { get; set; }
        public string? Address { get; set; }
        public string? PickingNo { get; set; }
        public DateTime? PickedDate { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string? ProductQRCode { get; set; }
        public string? InternalRemarks { get; set; }

        //NOT MAP with store procedure.
        private double? _remaining = 0;
        [NotMapped]
        public double? Remaining
        {
            get { return ShipmentQty - PackedQty; }
            set
            {
                _remaining = value;
            }
        }
    }
}
