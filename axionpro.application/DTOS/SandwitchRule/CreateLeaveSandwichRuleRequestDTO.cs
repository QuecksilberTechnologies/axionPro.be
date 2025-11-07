using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.SandwitchRule
{
    public class CreateLeaveSandwichRuleRequestDTO
    {
     
        public long EmployeeId { get; set; }

        public int RoleId { get; set; }
        public long TenantId { get; set; }

        public string? RuleName { get; set; }

        public bool IsIncludeHoliday { get; set; }

        public bool IsIncludeWeekend { get; set; }

        public bool IsActive { get; set; }

        public string? Remark { get; set; }

    


    }
}
