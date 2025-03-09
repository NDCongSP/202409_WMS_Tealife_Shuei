using Magicodes.ExporterAndImporter.Core;

namespace Application.DTOs
{
    [Exporter(Name = "REPORT 1")]
    public class LabelInfoDto
    {
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Base64 code.
        /// </summary>
        public string QrValue { get; set; }=string.Empty;
        public string Title1 { get; set; } = string.Empty;
        public string Title2 { get; set; } = string.Empty;
        public string Title3 { get; set; } = string.Empty;
        public string Title4 { get; set; } = string.Empty;
        public string Content1 { get; set; } = string.Empty;
        public string Content2 { get; set; } = string.Empty;
        public string Content3 { get; set; } = string.Empty;
        public string Content4 { get; set; } = string.Empty;
        public string QrValue2 { get; set; } = string.Empty;

        public bool SplitLabel { get; set; } = false;
    }

    /// <summary>
    /// Using for print product label at putaway.
    /// </summary>
    public class ProductLabelPrint
    {
        public int TotalLabel { get; set; } = 0;
        /// <summary>
        /// Index from 1.
        /// </summary>
        public int RowIndex { get; set; } = 0;

        public List<LabelInfoDto> DataPrint { get; set; } = new List<LabelInfoDto>();
    }
}
