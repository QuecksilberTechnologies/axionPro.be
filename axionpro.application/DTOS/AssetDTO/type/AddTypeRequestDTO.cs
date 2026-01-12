using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.type
{
    public class AddTypeRequestDTO
    {
        
       
        public required long AssetCategoryId { get; set; }
        public required string TypeName { get; set; }
        public string? Description { get; set; }
        public required bool IsActive { get; set; }
        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();
    }
}
