using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class UpdateReceiptImageRequestDTO
    {
        public Guid ReceiptLineId { get; set; }
        /// <summary>
        /// Error images of product.
        /// </summary>
        public List<ImageInfoDTO> ErrorImages { get; set; }
        public EnumProductErrorStatus StatusError { get; set; } = EnumProductErrorStatus.Normal;
    }
}
