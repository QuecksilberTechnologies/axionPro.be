using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
    public class TenantEnabledModuleRequestDTO
    {
        public long? TenantId { get; set; }
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
     
    }
}
