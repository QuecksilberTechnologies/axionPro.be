using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.TenantIndustry
{
 
        public class TenantIndustryResponseDTO
        {
            public int Id { get; set; }
            public string IndustryName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string? Remark { get; set; }
            public bool IsActive { get; set; } 
          
        }
     

}
