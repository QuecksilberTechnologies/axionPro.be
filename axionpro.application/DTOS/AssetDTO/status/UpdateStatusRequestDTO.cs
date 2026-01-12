using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.status
{
    public class UpdateStatusRequestDTO
    {
        
            public int Id { get; set; }            
            public string? StatusName { get; set; }
             public string? ColorKey { get; set; } 
               public string? Description { get; set; }
             public bool? IsActive { get; set; }

          public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();



    }
}
