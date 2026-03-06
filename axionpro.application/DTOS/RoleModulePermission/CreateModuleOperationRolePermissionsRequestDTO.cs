using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.RoleModulePermission
{
    public class CreateModuleOperationRolePermissionsRequestDTO
    {
             public  long TenantId { get; set; }
             public  long EmployeeId { get; set; }
             public  long RoleId { get; set; }
             public  int? ModuleId { get; set; }
             public  int?  ParentModuleId { get; set; }       
       

    }
    

}
