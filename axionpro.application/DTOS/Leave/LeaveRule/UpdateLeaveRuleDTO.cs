using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave.LeaveRule
{
    public class UpdateLeaveRuleRequestDTO
      
    {
        public long EmployeeId { get; set; }
        public long TenantId { get; set; }

        public long PolicyLeaveTypeId { get; set; }

        public bool ApplySandwichRule { get; set; }

        public bool IsHalfDayAllowed { get; set; }

        public int? HalfDayNoticeHours { get; set; }

        public int? NoticePeriodDays { get; set; }

        public int? MaxContinuousLeaves { get; set; }

        public int? MinGapBetweenLeaves { get; set; }

        public bool IsActive { get; set; } = true; // by default active  
 
        public string? Remark { get; set; }

        public long UpdatedById { get; set; }   // user id from request (logged-in user)
    }


}
