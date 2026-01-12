using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.status
{
    public class CreateStatusRequestDTO
    {
       
   
        public string StatusName { get; set; } = null!;
        public string? ColorKey { get; set; } = null!;
        public string? Description { get; set; }
      
        public ExtraPropRequestDTO Prop  = new ExtraPropRequestDTO();

    }
}
