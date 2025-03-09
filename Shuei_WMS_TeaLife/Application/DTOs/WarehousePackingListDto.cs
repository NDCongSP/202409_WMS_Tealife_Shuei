using Application.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTOs
{
    public class WarehousePackingListDto
    {
        #region Packing list
        /// <summary>
        /// ShipmentId.
        /// </summary>
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; }
        public string LocationName { get; set; }
        public string ShippingCarrierCode { get; set; }
        public Guid? ShippingBoxesId { get; set; }
        public string? ShippingBoxesName { get; set; }
        public string PrinterName { get; set; }

        public string ShippingAddress { get; set; }
        public DateTime PlanShipDate { get; set; }
        public string? SalesNo { get; set; }
        public EnumPackingListStatus StatusOfShipment { get; set; } = EnumPackingListStatus.Packed;
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? TrackingNo { get; set; }
        public string? LabelFilePath { get; set; }
        public string? LabelFileExtension { get; set; }
        public DateTime? PickedDate { get; set; }
        public DateTime? PackedDate { get; set; }
        public string? InternalRemarks { get; set; }
        #endregion
        public List<PackingListModel> ShipmentLines { get; set; }
        public EnumPackingStatus PackingStatus { get; set; } = EnumPackingStatus.Beginning;

        public List<PackingListGenerateRow> GenerateDetails { get; set; } = new List<PackingListGenerateRow>();
    }
}
