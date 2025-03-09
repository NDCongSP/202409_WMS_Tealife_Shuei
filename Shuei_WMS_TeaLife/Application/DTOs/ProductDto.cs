using Mapster;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ProductDto
    {
        public ProductDto()
        {
        }

        public ProductDto(Product product, string supplierName, string unitName, string categoryName, List<ProductJanCode> janCodes, int quantityShipment)
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
        public DateOnly? ExpirationDate { get; set; }
        public string? ProductCode { get; set; }
        public string? SupplierName { get; set; }
        public EnumProductStatus? ProductStatus { get; set; }
        public EnumProductType? ProductType { get; set; }

        public string? UnitName { get; set; }
        public int UnitId { get; set; }
        public string? CategoryName { get; set; }
        public string ProductStatusString { get; set; }
        public string? Location { get; set; }
        public string? Bin { get; set; }

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
