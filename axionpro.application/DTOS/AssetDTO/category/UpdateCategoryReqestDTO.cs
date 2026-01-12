using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.category
{
    public class UpdateCategoryReqestDTO
    {

       
        public string? CategoryName { get; set; }
        public long  Id { get; set; } 

        public string? Remark { get; set; }

        public bool? IsActive { get; set; }
 
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();


    }
}
