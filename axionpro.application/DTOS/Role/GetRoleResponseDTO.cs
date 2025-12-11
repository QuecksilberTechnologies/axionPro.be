using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class GetRoleResponseDTO 
    {
         public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty; // Default value        
        public int RoleType { get; set; }          
        public string? RoleTypeName { get; set; }          
        public bool IsActive { get; set; } = false; // Default false
        public string? Remark { get; set; } = string.Empty; // Default false
      






    }

}
