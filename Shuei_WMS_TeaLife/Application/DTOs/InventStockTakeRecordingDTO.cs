using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class InventStockTakeRecordingDTO
    {
        public Guid Id { get; set; }
        public string StockTakeNo { get; set; }
        public int RecordNo { get; set; }
        public string Location { get; set; }
        public string PersonInCharge { get; set; }
        public DateOnly TransactionDate { get; set; }
        public EnumInvenTransferStatus Status { get; set; } = EnumInvenTransferStatus.InProcess;
        public string? Remarks { get; set; }
        public EnumHHTStatus HHTStatus { get; set; } = EnumHHTStatus.New;
        public string? HHTInfo { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public string? CreateOperatorId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? UpdateOperatorId { get; set; }
        public DateTime? UpdateAt { get; set; }

        // add on
        public string? TenantFullName { get; set; }
        public string? LocationName { get; set; }
        public string? PersonName { get; set; }
        public List<InventStockTakeRecordingLineDtos> InventStockTakeRecordingLineDtos { get; set; } = new List<InventStockTakeRecordingLineDtos>();

        public InventStockTakeRecordingDTO() { }

        public InventStockTakeRecordingDTO(InventStockTakeRecording inventStockTakeRecording, string locationName, string tenantFullName, string personName, List<InventStockTakeRecordingLineDtos> inventStockTakeRecordingLineDTOs)
        {
            Id = inventStockTakeRecording.Id;
            StockTakeNo = inventStockTakeRecording.StockTakeNo;
            RecordNo = inventStockTakeRecording.RecordNo;
            Location = inventStockTakeRecording.Location;
            PersonInCharge = inventStockTakeRecording.PersonInCharge;
            TransactionDate = inventStockTakeRecording.TransactionDate;
            Status = inventStockTakeRecording.Status;
            Remarks = inventStockTakeRecording.Remarks;
            HHTStatus = inventStockTakeRecording.HHTStatus;
            HHTInfo = inventStockTakeRecording.HHTInfo;
            TenantId = inventStockTakeRecording.TenantId;
            TenantFullName = tenantFullName;
            LocationName = locationName;
            PersonName = personName;
            InventStockTakeRecordingLineDtos = inventStockTakeRecordingLineDTOs;
        }
    }

    public class InventStockTakeRecordingLineDtos
    {
        public Guid Id { get; set; }
        public Guid StockTakeRecordingId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string? ProductName { get; set; }
        public string LotNo { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty;
        public double? ExpectedQty { get; set; }
        public double? ActualQty { get; set; }
        public double? Remaining { get; set; }
    }
}
