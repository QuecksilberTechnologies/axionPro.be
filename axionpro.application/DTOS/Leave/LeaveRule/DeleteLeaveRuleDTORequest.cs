using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave.LeaveRule
{
    public class DeleteLeaveRuleDTO
    {
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long TenantId { get; set; }
        public int Id { get; set; }
        public long UserId { get; set; }
    }
}
