using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Module.ManualModule
{
    public class GetModuleChildInversRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int TenantId { get; set; }
        public bool IsActive { get; set; }
      
        

       
    }
}
