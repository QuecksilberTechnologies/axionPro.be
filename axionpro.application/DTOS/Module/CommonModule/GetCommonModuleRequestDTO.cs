using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Module.CommonModule
{
    public class GetCommonModuleRequestDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public int TenantId { get; set; }
        public bool IsActive { get; set; }
        public bool IsModuleDisplayInUi { get; set; }
        

       
    }
}
