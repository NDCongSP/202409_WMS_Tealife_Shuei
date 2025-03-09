using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class ErrorResponse
    {
        public string Title { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();
        public string TraceId { get; set; }
    }
}
