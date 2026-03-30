using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace axionpro.application.DTOS.Employee.Experience
{
    // =====================================================================
    //  CREATE EXPERIENCE REQUEST DTO
    // =====================================================================
    
    public class CreateExperienceRequestDTO
    {
        public required string EmployeeId { get; set; }
        public decimal? Ctc { get; set; }
        // 🔹 Job Info
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeIdOfCompany { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? Experience { get; set; }
        public bool IsWFH { get; set; } = false;

        // 🔹 Location
        public int? WorkingCountryId { get; set; }
        public int? WorkingStateId { get; set; }
        public int? WorkingDistrictId { get; set; }

        public bool IsForeignExperience { get; set; } = false;

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
        public bool IsAnyGap { get; set; } = false;
        public string? ReasonOfGap { get; set; }

        public DateTime? GapYearFrom { get; set; }
        public DateTime? GapYearTo { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new();

        // 🔹 Documents (🔥 Important)
        public List<CreateExperienceDocumentDTO>? Documents { get; set; }
    }

    // =====================================================================
    //  PAYSLIP UPLOAD DTO
    // =====================================================================
    public class CreateExperienceDocumentDTO
    {
        public int DocumentType { get; set; }   // ENUM use hoga

        public string? FileName { get; set; }
        public IFormFile? File { get; set; }

        public string? Remark { get; set; }
    }

}
