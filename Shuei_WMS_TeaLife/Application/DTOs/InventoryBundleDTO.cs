namespace Application.DTOs
{
    public class InventBundleDTO : InventBundle
    {
        public List<InventBundlesLineDTO> InventBundleLines { get; set; }
        public string? ProductName { get; set; }
        public string? LocationName { get; set; }
        public string? BinCode { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; }

    }
}
