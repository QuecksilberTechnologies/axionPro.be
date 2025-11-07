using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Experience
{
    public class CreateExperienceRequestDTO:BaseRequest
    {

        public string EmployeeId { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }

        // 🔹 Experience Verification
        public bool? IsExperienceVerified { get; set; }        
        public DateTime? ExperienceVerificationDateTime { get; set; }
        public bool? IsExperienceVerifiedByMail { get; set; }
        public bool? IsExperienceVerifiedByCall { get; set; }

        // 🔹 Experience Details
        public bool? IsSoftDeleted { get; set; }
        public int? ExperienceTypeId { get; set; }
        public string? Location { get; set; }
        public decimal? CTC { get; set; }
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }
        public string? ReportingManagerEmail { get; set; }
        public string? WorkedWithName { get; set; }
        public string? WorkedWithContactNumber { get; set; }
        public string? WorkedWithDesignation { get; set; }
        public string? ExperienceLetterPath { get; set; }
        public string? Comment { get; set; }

        // 🔹 Common Audit Fields
        public string? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
 

        // 🔹 Info Verification
        public string? InfoVerifiedById { get; set; }
        public bool? IsInfoVerified { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }

        // 🔹 Flags
        public bool? IsEditAllowed { get; set; }
        public bool IsActive { get; set; }
    }


}
