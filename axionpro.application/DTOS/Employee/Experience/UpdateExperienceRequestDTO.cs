using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace axionpro.application.DTOS.Employee.Experience
{
   
    // =====================================================================
    //  EXPERIENCE   DTO
    // =====================================================================
    public class UpdateExperienceRequestDTO
    {
        // 🔥 IMPORTANT
        [Required]
        public string UserEmployeeId { get; set; } = string.Empty;

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        // 🔹 Parent Fields
        public decimal? Ctc { get; set; }
        public long Id { get; set; } // null = NEW, value = UPDATE

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

        // 🔹 Gap
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }

        public DateTime? GapYearFrom { get; set; }
        public DateTime? GapYearTo { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new();


        // 🔹 Documents
        public List<UpdateExperienceDocumentDTO>? Documents { get; set; }
    }

    // =====================================================================
    //  PAYSLIP UPLOAD DTO
    // =====================================================================
    public class UpdateExperienceDocumentDTO
    {
        // 🔥 IMPORTANT
        public long? Id { get; set; } // null = NEW, value = UPDATE

        public int DocumentType { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public string? Remark { get; set; }
    }

}
