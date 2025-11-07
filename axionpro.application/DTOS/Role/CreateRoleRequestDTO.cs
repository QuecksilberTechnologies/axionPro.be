using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class CreateRoleRequestDTO:BaseRequest
    {

       
        public string RoleName { get; set; } = string.Empty; // Default value        
       // public int DesignationId { get; set; }       
        public int RoleType { get; set; }       
        public string? Remark { get; set; } // Nullable
        public bool IsActive { get; set; } = false; // Default false
 





    }
}
