using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Experience
{
 


    public class GetExperienceResponseDTO
    {
        public string ExperienceId { get; set; }           // encrypted
        public string EmployeeId { get; set; }             // encrypted
        public string Ctc { get; set; }
        public string Comment { get; set; }
        public bool IsFresher { get; set; }
        public bool IsGap { get; set; }
        public bool IsActive { get; set; }
        public DateTime? AddedDateTime { get; set; }

        public List<GetExperienceDetailDTO> Details { get; set; } = new();
    }
    public class GetExperienceDetailDTO
    {
        public string? DetailId { get; set; }
        public string? ExperienceId { get; set; }
        public string? EmployeeId { get; set; }

        // GAP
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }
        public string? GapYearFrom { get; set; }
        public string? GapYearTo { get; set; }
        public string? GapCertificateDocName { get; set; }
        public string? GapCertificateDocPath { get; set; }

        // EXPERIENCE
        public string? CompanyName { get; set; }
        public int? Experience { get; set; }
        public bool? IsWFH { get; set; }
        public string? WorkingCountryId { get; set; }
        public string? WorkingStateId { get; set; }
        public string? WorkingDistrictId { get; set; }

        public string? EmployeeIdOfCompany { get; set; }

        public string? ColleagueName { get; set; }
        public string? ColleagueDesignation { get; set; }
        public string? ColleagueContactNumber { get; set; }
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }
        public string? VerificationEmail { get; set; }
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }
        public string? Designation { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // DOCS
        public string? TaxationDocName { get; set; }
        public string? TaxationDocFilePath { get; set; }
        public string? ExperienceLetterDocName { get; set; }
        public string? ExperienceLetterDocPath { get; set; }
        public string? JoiningLetterDocName { get; set; }
        public string? JoiningLetterDocPath { get; set; }
        public string? BankStatementDocName { get; set; }
        public string? BankStatementDocPath { get; set; }

        public DateTime? AddedDateTime { get; set; }

        // PAYSLIPS
        public List<GetExperiencePayslipDTO> Payslips { get; set; } = new();
    }

    public class GetExperiencePayslipDTO
    {
        public string? PayslipId { get; set; }
        public string? EmployeeId { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public string? PayslipDocName { get; set; }
        public string? PayslipDocPath { get; set; }

        public bool IsActive { get; set; }
        public bool HasUploadedPayslip { get; set; }

        public DateTime? AddedDateTime { get; set; }
    }



}
