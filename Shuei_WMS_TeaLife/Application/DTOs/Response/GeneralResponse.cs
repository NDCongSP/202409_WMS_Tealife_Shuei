using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class GeneralResponse
    {
        public bool Flag { get; set; }=false;
        public string Message { get; set; } = null;
    }
}
