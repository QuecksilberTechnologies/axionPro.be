using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Common
{
    public class UploadDocResponseDTO
    {
        public bool IsUploaded { get; set; } = false;
        public string? FileName { get; set; } 
        public string? FilePath { get; set; }
    }

}
