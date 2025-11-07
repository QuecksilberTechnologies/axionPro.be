using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave.LeaveRule
{
    public class GetLeaveRuleRequestDTO
    {

        public long EmployeeId { get; set; }
        public long TenantId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
