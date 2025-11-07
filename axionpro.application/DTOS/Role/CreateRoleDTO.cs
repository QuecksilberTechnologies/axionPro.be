using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class CreateRoleDTO
    {

        public long? TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty; // Default value        
        public string? Remark { get; set; } // Nullable
        public bool IsActive { get; set; } = false; // Default false
 





    }
}
