using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFtoPrinter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrintClientController : ControllerBase
    {

        private readonly ILogger<PrintClientController> _logger;

        public PrintClientController(ILogger<PrintClientController> logger)
        {
            _logger = logger;
        }

        [HttpPost("print")]
        public IActionResult Print([FromBody] PrintData model)
        {
            try
            {
                byte[] binaryData = Convert.FromBase64String(model.printData);
                var filename = "Data/Tealife_" + Guid.NewGuid().ToString() + ".pdf";
                System.IO.File.WriteAllBytes(filename, binaryData);
                var filePath = binaryData;
                var networkPrinterName = string.Format(@"\\{0}\{1}", model.printerAddress, model.printerName);
                var printTimeout = new TimeSpan(0, 30, 0);
                var printer = new PDFtoPrinterPrinter();
                printer.Print(new PrintingOptions(model.printerName, filename), printTimeout);
                string args = string.Format("{0} \"{1}\"", filename, model.printerName);
                Process.Start(@"PDFtoPrinter.exe", args);
                //FileInfo fileInfo = new FileInfo(filename);
                //System.IO.File.Copy(fileInfo.FullName, networkPrinterName, true);
                return Ok(true);
            }
            catch (Exception ex)
            { 
                return BadRequest(ex);
            }
        }
    }
}

