using EnumPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UEditor
{
    public class CatchImageResponseDTO
    {
        public UploadStateCode state { get; set; }

        public List<ImageUrlDTO> list { get; set; } 
    }
}
