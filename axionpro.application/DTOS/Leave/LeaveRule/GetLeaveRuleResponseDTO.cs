using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave.LeaveRule
{
    public class GetLeaveRuleResponseDTO
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public long PolicyLeaveTypeId { get; set; }
        public bool ApplySandwichRule { get; set; }
        public bool IsHalfDayAllowed { get; set; }
        public int? LeaveTypeId { get; set; }
        public string? LeaveName { get; set; }
        public int? HalfDayNoticeHours { get; set; }
        public int? NoticePeriodDays { get; set; }
        public int? MaxContinuousLeaves { get; set; }
        public int? MinGapBetweenLeaves { get; set; }
        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public bool? IsLinkedSandwichRule { get; set; }
        public string? Remark { get; set; }
        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
