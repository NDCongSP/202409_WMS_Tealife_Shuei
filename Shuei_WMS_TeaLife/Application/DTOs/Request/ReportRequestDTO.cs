using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Request
{
    public class ReportRequestDTO
    {
        /// <summary>
        /// Table get data.
        /// </summary>
        public string TableName { get; set; }
        public string Id { get; set; }
    }
}
