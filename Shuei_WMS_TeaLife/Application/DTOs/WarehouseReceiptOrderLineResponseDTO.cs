using Application.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class WarehouseReceiptOrderLineResponseDTO : WarehouseReceiptOrderLine
    {
        private ImagesName _imageNames;
        public ImagesName ErrorImagesName
        {
            get { return !string.IsNullOrEmpty(ErrorImages) ? JsonConvert.DeserializeObject<ImagesName>(ErrorImages) : null; }
            set { _imageNames = value; }
        }

        /// <summary>
        /// error images if have.
        /// </summary>
        public List<ImageInfoDTO> LoadErrorImagesResult { get; set; } = new List<ImageInfoDTO>();
    }
}
