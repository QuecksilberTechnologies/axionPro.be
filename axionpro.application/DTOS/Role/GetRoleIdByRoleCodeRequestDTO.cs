using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Role
{
    public class GetRoleIdByRoleCodeRequestDTO
    {
        public string RoleCode { get; set; } = string.Empty;
        public string RoleType { get; set; } = string.Empty;        
        public long? TenantId { get; set; } 
        public bool IsSystem { get; set; }
     
    }
}
