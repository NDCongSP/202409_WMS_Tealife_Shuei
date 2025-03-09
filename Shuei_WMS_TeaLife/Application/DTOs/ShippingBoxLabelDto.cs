using Magicodes.ExporterAndImporter.Core;

namespace Application.DTOs
{
    [Exporter(Name = "Shipping Boxes QR Label")]
    public class ShippingBoxLabelDto
    {
        public string QrValue { get; set; } = string.Empty;
        public string BoxName { get; set; } = string.Empty;
        public string BoxType { get; set; } = string.Empty;
    }
}
