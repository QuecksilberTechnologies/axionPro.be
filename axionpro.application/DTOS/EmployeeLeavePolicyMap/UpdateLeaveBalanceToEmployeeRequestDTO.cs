using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.EmployeeLeavePolicyMap
{
    public class UpdateLeaveBalanceToEmployeeRequestDTO
    {
        public long TenantId { get; set; }        
        public long EmployeeId { get; set; }
        public int RoleId { get; set; }
        public long Id { get; set; }
        public int LeaveYear { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Availed { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CarryForwarded { get; set; }
        public decimal Encashed { get; set; }
        public decimal LeavesOnHold { get; set; }
        public bool IsAllBalanceOnHold { get; set; } = false;
    }
}
