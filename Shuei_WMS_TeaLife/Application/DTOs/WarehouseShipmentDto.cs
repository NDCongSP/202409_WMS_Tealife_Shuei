using Mapster;
using System.Text.Json.Serialization;

namespace Application.DTOs
{
    public class WarehouseShipmentDto
    {
        public Guid Id { get; set; }

        public string? ShipmentNo { get; set; } = default!;

        public string? SalesNo { get; set; }

        public int TenantId { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? BinId { get; set; } = string.Empty;

        public DateOnly? PlanShipDate { get; set; }

        public string? PersonInCharge { get; set; }

        public string? ShippingCarrierCode { get; set; }

        public string? ShippingAddress { get; set; }

        public string? Telephone { get; set; }

        public string? TrackingNo { get; set; }

        public int? OrderDispatchId { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? PickingNo { get; set; }

        public string? LocationName { get; set; }

        public string? TenantName { get; set; }

        public string? PersonInChargeName { get; set; }

        public string? CreateOperatorId { get; set; }

        public DateTime? CreateAt { get; set; }

        public EnumShipmentOrderStatus? Status { get; set; } = EnumShipmentOrderStatus.Open;

        public EnumWarehouseTransType? ShipmentType { get; set; } = EnumWarehouseTransType.Shipment;

        public int DHLPickup { get; set; }

        public DateTime? DHLPickupDatetime { get; set; }

        [JsonIgnore]
        public DateOnly? ExpectedReceiveDate { get; set; }
        
        [JsonIgnore]
        public string? InboundLocation { get; set; }

        public List<WarehouseShipmentLineDto> WareHouseShipmentLineDtos { get; set; } = new();

        public WarehouseShipmentDto() { }

        public WarehouseShipmentDto(WarehouseShipment data)
        {
            Id = data.Id;
            ShipmentNo = data.ShipmentNo;
            SalesNo = data.SalesNo;
            TenantId = data.TenantId;
            Location = data.Location;
            BinId = data.BinId;
            PlanShipDate = data.PlanShipDate;
            PersonInCharge = data.PersonInCharge;
            ShippingCarrierCode = data.ShippingCarrierCode;
            ShippingAddress = data.ShippingAddress;
            Telephone = data.Telephone;
            TrackingNo = data.TrackingNo;
            Email = data.Email;
            Address = data.Address;
            PickingNo = data.PickingNo;
            Status = data.Status;
            WareHouseShipmentLineDtos = new();
        }

        public WarehouseShipmentDto(WarehouseShipment info, List<WarehouseShipmentLine> item)
        {
            Id = info.Id;
            ShipmentNo = info.ShipmentNo;
            SalesNo = info.SalesNo;
            TenantId = info.TenantId;
            Location = info.Location;
            BinId = info.BinId;
            PlanShipDate = info.PlanShipDate;
            PersonInCharge = info.PersonInCharge;
            ShippingCarrierCode = info.ShippingCarrierCode;
            ShippingAddress = info.ShippingAddress;
            Telephone = info.Telephone;
            TrackingNo = info.TrackingNo;
            Email = info.Address;
            Address = info.Address;
            PickingNo = info.PickingNo;
            Status = info.Status;
            WareHouseShipmentLineDtos = item.Adapt<List<WarehouseShipmentLineDto>>();
        }
    }
    public class WarehouseShipmentLineDto
    {
        public Guid Id { get; set; }

        public string? ShipmentNo { get; set; }

        public string? ProductCode { get; set; }

        public int? UnitId { get; set; }

        public double? ShipmentQty { get; set; }

        public string? LotNo { get; set; }

        public string? Location { get; set; }

        public string Bin { get; set; }

        public double? PackedQty { get; set; }

        public DateOnly? PackedDate { get; set; }

        public DateOnly? ExpirationDate { get; set; }

        public EnumShipmentOrderStatus? Status { get; set; } = EnumShipmentOrderStatus.Draft;

        public string? ProductName { get; set; }

        public string? Unit { get; set; }

        public int AvailableQuantity { get; set; }

        public int StockAvailable { get; set; }

        public WarehouseShipmentLineDto() { }

        public WarehouseShipmentLineDto(WarehouseShipmentLine data)
        {
            Id = data.Id;
            ShipmentNo = data.ShipmentNo;
            ProductCode = data.ProductCode;
            UnitId = data.UnitId;
            ShipmentQty = data.ShipmentQty;
            Location = data.Location;
            Bin = data.Bin;
            PackedQty = data.PackedQty;
            PackedDate = data.PackedDate;
            Status = data.Status;
        }
    }
    public class SubmitCompletedShipmentDto
    {
        /// <summary>
        /// list containing shipmentIds to do picking.
        /// </summary>
        public List<Guid> Id { get; set; }
        /// <summary>
        /// 0-all.
        /// </summary>
        public int TenantId { get; set; } = 0;
        public string? Manager { get; set; }
        public string? Remarks { get; set; }
    }

    public class DeliveryNoteModel
    {
        public string? Logo { get; set; }
        public string? ShipmentNo { get; set; }
        public int TenantId { get; set; }
        public bool IsSplitOrder { get; set; }
        public Order? OrderData { get; set; }
        public string? Barcode { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CompanyName { get; set; }
        public string? ShopName { get; set; }
        public string? DeliveryNoteMessage { get; set; }
        public string? SiteAddress { get; set; }
        public string? SiteAddressName { get; set; } 
        public string? CompanyEmail { get; set; }
        public string? OrderNo { get; set; } = "Order #21613";
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? QRCode { get; set; }
        public List<ShipmentLineSummaryModel> Items { get; set; } = new();
    }

    public class ShipmentLineSummaryModel
    {
        public string? ShipmentNo { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public int PackedQuantity { get; set; }
    }
}
