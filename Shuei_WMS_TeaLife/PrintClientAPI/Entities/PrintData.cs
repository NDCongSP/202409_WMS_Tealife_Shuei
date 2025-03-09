using System;
using System.Collections.Generic;

namespace WebApi.Entities
{
    public partial class PrintData
    {
        public PrintData()
        {
        }
        public string printerAddress { get; set; }
        public string printerName { get; set; }
        public string printData { get; set; }
    }
}
