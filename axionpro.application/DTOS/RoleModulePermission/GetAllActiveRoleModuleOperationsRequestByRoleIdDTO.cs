using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOs.RoleModulePermission
{
    public class GetAllActiveRoleModuleOperationsRequestByRoleIdDTO
    {
        
        
        public int RoleId { get; set; }  // RoleId of the role
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO(); // Common properties for request (e.g., TenantId, EmployeeId, etc.) 
    }

}
