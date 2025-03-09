using Mapster;

namespace Application.DTOs
{
    public class ReturnOrderDto : GenericEntity
    {
        public ReturnOrderDto()
        {
        }

        public ReturnOrderDto(ReturnOrder ro, List<ReturnOrderLine> rol, string picName, string referenceNo)
        {
            Id = ro.Id;
            ReturnOrderNo = ro.ReturnOrderNo;
            ShipmentNo = ro.ShipmentNo;
            ReturnDate = ro.ReturnDate;
            Reason = ro.Reason;
            PersonInCharge = ro.PersonInCharge;
            ShipDate = ro.ShipDate;
            Status = ro.Status;
            CreateOperatorId = ro.CreateOperatorId;
            CreateAt = ro.CreateAt;
            UpdateOperatorId = ro.UpdateOperatorId;
            UpdateAt = ro.UpdateAt;
            IsDeleted = ro.IsDeleted;
            ReturnOrderLines = rol.Adapt<List<ReturnOrderLineDto>>();
            PersonInChargeName = picName;
            ReferenceNo = referenceNo;  
        }

        public ReturnOrderDto(ReturnOrder ro, List<ReturnOrderLineDto> rol)
        {
            Id = ro.Id;
            ReturnOrderNo = ro.ReturnOrderNo;
            ShipmentNo = ro.ShipmentNo;
            ReturnDate = ro.ReturnDate;
            Reason = ro.Reason;
            PersonInCharge = ro.PersonInCharge;
            ShipDate = ro.ShipDate;
            Status = ro.Status;
            CreateOperatorId = ro.CreateOperatorId;
            CreateAt = ro.CreateAt;
            UpdateOperatorId = ro.UpdateOperatorId;
            UpdateAt = ro.UpdateAt;
            IsDeleted = ro.IsDeleted;
            ReturnOrderLines = rol;
        }

        public Guid Id { get; set; }

        public string? ReturnOrderNo { get; set; } 

        public string ShipmentNo { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public string Reason { get; set; }

        public string PersonInCharge { get; set; }
        public string? PersonInChargeName { get; set; }

        public DateOnly? ShipDate { get; set; }
        public EnumReturnOrderStatus Status { get; set; } = EnumReturnOrderStatus.Open;

        public List<ReturnOrderLineDto> ReturnOrderLines { get; set; } = new();
        public string? ReferenceNo { get; set; }
    }

    public class ReturnOrderLineDto : GenericEntity
    {
        public ReturnOrderLineDto()
        {
        }

        public Guid Id { get; set; }

        public string? ReturnOrderNo { get; set; }

        public string? Location { get; set; }
        public string? LocationName { get; set; }

        public double? Qty { get; set; }
        public EnumStatus Status { get; set; } = EnumStatus.Activated;
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public int UnitId { get; set; }
        public string? UnitName { get; set; }
        public int? AvailableReturnQty { get; set; }

        public double? PackedQty { get; set; }

    }
}
