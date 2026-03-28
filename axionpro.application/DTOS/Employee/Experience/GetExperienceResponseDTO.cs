using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.Experience
{
    

    public class GetEmployeeExperienceResponseDTO : BaseRequest
    {

      
        public long Id { get; set; }
        public string? EmployeeId { get; set; }

        // 🔹 Basic Info
        public decimal? Ctc { get; set; }


        // 🔹 Job Info
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeIdOfCompany { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? Experience { get; set; }
        public bool IsWFH { get; set; }

        // 🔹 Location
        public int? WorkingCountryId { get; set; }
        public int? WorkingStateId { get; set; }
        public int? WorkingDistrictId { get; set; }

        public bool IsForeignExperience { get; set; }

        // 🔹 Exit
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }

        // 🔹 Reporting
        public string? ColleagueName { get; set; }
        public string? ColleagueDesignation { get; set; }
        public string? ColleagueContactNumber { get; set; }

        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }

        public string? VerificationEmail { get; set; }
        public double CompletionPercentage { get; set; }

        // 🔹 Gap
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }

        public DateTime? GapYearFrom { get; set; }
        public DateTime? GapYearTo { get; set; }

        // 🔹 Verification
        public bool? IsExperienceVerified { get; set; }
        public bool? IsExperienceVerifiedByMail { get; set; }
        public bool? IsExperienceVerifiedByCall { get; set; }

        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }

        // 🔹 Documents
        public List<GetEmployeeExperienceDocumentDTO>? Documents { get; set; }
    }
    public class GetEmployeeExperienceDocumentDTO
    {
        public long Id { get; set; }

        public int DocumentType { get; set; }
        public string? DocumentTypeName { get; set; } // 🔥 UI ke liye

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Url { get; set; } // 🔥 Ready URL

        public string? Remark { get; set; }
    }

}
