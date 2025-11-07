using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Leave
{
    public class GetPolicyLeaveTypeMappingRequestDTO: BaseRequest
    {
        public int RoleId { get; set; }                  // Required
        public long EmployeeId { get; set; }                     // Required
        public long TenantId { get; set; }                     // Required
        public int PolicyTypeId { get; set; }           // Required
        public int LeaveTypeId { get; set; }            // Required
        public int EmployeeTypeId { get; set; }       
        public int ApplicableGenderId { get; set; }   
        public bool? IsMarriedApplicable { get; set; } // Optional (Single/Married/All)
        public int TotalLeavesPerYear { get; set; }     // Required
        public bool MonthlyAccrual { get; set; }        // Default false
        public bool CarryForward { get; set; }          // Default false
        public int? MaxCarryForward { get; set; }       
        public int? CarryForwardExpiryMonths { get; set; }  
        public bool Encashable { get; set; }            // Default false
        public int? MaxEncashable { get; set; }
        public bool IsSoftDeleted { get; set; } = false;      // Default false
        public bool IsProofRequired { get; set; }       // Default false
        public string? ProofDocumentType { get; set; }  

        public DateTime EffectiveFrom { get; set; }     // Required
        public DateTime? EffectiveTo { get; set; }      // Optional

        public bool IsActive { get; set; } = true;      // Default true
        public string? Remark { get; set; }             // Optional

                   // Required
 
    }
}
