using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class CreateRoleRequestDTO 
    {
        public required string UserEmployeeId { get; set; }
       
        public string RoleName { get; set; } = string.Empty; // Default value        
       
        public int RoleType { get; set; }       
        public string? Remark { get; set; } // Nullable
        public bool IsActive { get; set; } = false; // Default false
   
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();





    }
}
