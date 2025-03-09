using Mapster;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class InventoryDto
    {
        public InventoryDto()
        {
        }

        public InventoryDto(Product product, string supplierName, string unitName, string categoryName, List<ProductJanCode> janCodes, int quantityShipment)
        {
            Id = product.Id;
            ProductName = product.ProductName;
            StockAvailableQuantity = product.StockAvailableQuanitty;
            ProductCode = product.ProductCode;
            SupplierName = supplierName;
            ProductStatus = product.ProductStatus;
            UnitName = unitName;
            UnitId = product.UnitId;
            CategoryName = categoryName;
            ProductStatusString = product.ProductStatus.ToString();
            JanCodes = janCodes.Adapt<List<ProductJanCodeDto>>();
            StockAvailableQuantityTrans = StockAvailableQuantityTrans - QuantityShipment;
            QuantityShipment = quantityShipment;
        }

        [Key] public int Id { get; set; }

        public string? ProductName { get; set; }
        /// <summary>
        /// Quantity from table Product
        /// </summary>
        public int? StockAvailableQuantity { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public EnumProductStatus? ProductStatus { get; set; }
        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string? LocationName { get; set; }
        public string? BinCode { get; set; }
        public string ProductStatusString { get; set; }
        public List<ProductJanCodeDto> JanCodes { get; set; }

        /// <summary>
        /// Quantity from wh trans
        /// </summary>
        public int StockAvailableQuantityTrans { get; set; }
        /// <summary>
        /// Quantity from wh shipment status OPEN
        /// </summary>
        public int QuantityShipment { get; set; }

    }
}
