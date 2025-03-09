using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ImagesName:List<ImageName>
    {
    }

    public class ImageName
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
    }
}
