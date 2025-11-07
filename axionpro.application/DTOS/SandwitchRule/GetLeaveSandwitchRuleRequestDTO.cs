using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SandwitchRule
{
    public class GetLeaveSandwitchRuleRequestDTO : BaseRequest
    {
        public long EmployeeId { get; set; }
         public long TenantId { get; set; }

        public int RoleId { get; set; }
        public bool IsActive { get; set; }


     

    }
}
