using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.RoleModulePermission
{
    public class GetTenantModuleOperationRolePermissionsRequestDTO : BaseRequest
    {
             public  long TenantId { get; set; }
             public  long EmployeeId { get; set; }
             public required  int RoleId { get; set; }
             public  int? ModuleId { get; set; }
             public  int?  ParentModuleId { get; set; }       
             public  int? SubscriptionPlanId { get; set; }       
       

    }
   


}
