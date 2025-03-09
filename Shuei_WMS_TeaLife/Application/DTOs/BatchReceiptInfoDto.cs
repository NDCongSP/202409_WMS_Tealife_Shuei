using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;

namespace Application.DTOs
{
    /// <summary>
    ///     批量导出Dto
    /// </summary>
    [PdfExporter(Orientation = Orientation.Portrait, PaperKind = PaperKind.A5)]
    [Exporter(Name = "REPORT")]
    public class BatchReceiptInfoDto
    {
        public string Payee { get; set; }
        public List<ReceiptInfoTest> ReceiptInfoInputs { get; set; }
    }
}
