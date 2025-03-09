using Application.DTOs;
using Application.Models;

namespace WebUIFinal.Core
{
    public class MasterTransferToDetails
    {
        public WarehousePackingListDto TransferToPackingDetail { get; set; }

        public WarehousePickingDTO TransferToPickingDetail { get; set; }

        public string ShipmentNo { get; set; } = string.Empty;

        public string RoleId { get; set; } = string.Empty;

        public SupplierTenantDTO SupplierInfo { get; set; }
    }
}
