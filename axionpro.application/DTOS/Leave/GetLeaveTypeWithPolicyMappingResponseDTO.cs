using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class GetLeaveTypeWithPolicyMappingResponseDTO
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public string PolicyTypeName { get; set; } = string.Empty;   // PolicyType se
        public int LeaveTypeId { get; set; }    
        public string LeaveTypeName { get; set; } = string.Empty;   // LeaveType se
        public int EmployeeTypeId { get; set; }
        public bool? IsEmployeeMapped { get; set; }
        public string? EmployeeTypeName { get; set; }               // EmployeeType se
        public int? ApplicableGenderId { get; set; }
        public bool? IsMarriedApplicable { get; set; }
        
        public int TotalLeavesPerYear { get; set; }
        public bool MonthlyAccrual { get; set; }
        public bool CarryForward { get; set; }
        public bool Encashable { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; }
        public string? Remark { get; set; }
    }
}
