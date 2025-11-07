using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Module
{
    public class ModuleOperationConfigurationRequestDTO : BaseRequest
    {
     public   long? TenantId { get; set; }

     public   int RoleId { get; set; }    

     public   long EmployeeId { get; set; }    
    }
}
