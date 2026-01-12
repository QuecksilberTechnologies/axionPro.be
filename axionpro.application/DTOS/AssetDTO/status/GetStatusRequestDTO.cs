using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.status
{

    /// <summary>
    /// post-request to fetch all asset status
    /// </summary>
    public class GetStatusRequestDTO 
    {
         
           
        public long Id { get; set; }        
        public bool  IsActive { get; set; }
       
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();








    }
}
