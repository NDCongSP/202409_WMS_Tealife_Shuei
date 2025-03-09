using Magicodes.ExporterAndImporter.Core;

namespace Application.DTOs
{
    [Exporter(Name = "REPORT")]
    public class ReceiptInfo
    {
        public string QrValue { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
