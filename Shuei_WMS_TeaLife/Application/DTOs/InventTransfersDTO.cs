using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class InventTransfersDTO :GenericEntity
    {
        public Guid Id { get; set; }
        public string TransferNo { get; set; }
        public string Location { get; set; }
        public string LocationName { get; set; }
        public DateOnly? TransferDate { get; set; }
        public int TenantId { get; set; }
        public string Description { get; set; }
        public string PersonInCharge { get; set; }
        public string PersonInChargeName{ get; set; }
        public EnumInvenTransferStatus Status { get; set; } = EnumInvenTransferStatus.InProcess;
        public List<InventTransfersLineDTO> InventTransferLines { get; set; } // Ensure this property is public
        public string TenantName { get; set; }


    }
}
