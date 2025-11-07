using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.RoleModulePermission
{
    public class GetActiveRoleModuleOperationsRequestDTO 
    {
        
        public long  TenantId { get; set; }
        public string RoleIds { get; set; } = string.Empty; // comma-separated: "1,2,3"
    }

}
