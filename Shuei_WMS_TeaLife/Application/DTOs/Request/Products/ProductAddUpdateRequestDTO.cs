namespace Application.DTOs.Request.Products
{
    public class ProductAddUpdateRequestDTO : Product
    {
        public List<ProductJanCode> JanCodeList { get; set; } = [];

        public bool IsUpdateImage { get; set; } = false;
        //public ImageInfoDTO ImageInfo { get; set; }=new ImageInfoDTO();
        public ImageStorage ImageStorage { get; set; } = new ImageStorage();
    }
}
