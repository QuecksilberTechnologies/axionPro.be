using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Experience
{
    public class EmployeeExperienceDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public decimal CTC { get; set; }
        public string? Comment { get; set; }
        public long AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public long? SoftDeletedById { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        public bool IsEditAllowed { get; set; }
        public bool IsActive { get; set; }
        public bool HasEPFAccount { get; set; }

        public List<EmployeeExperienceDetailDTO> ExperienceDetails { get; set; } = new();
    }

    public class EmployeeExperienceDetailDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public bool IsInfoLatestCompany { get; set; }
        public string? Experience { get; set; }

        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }
        public int? GapYearFrom { get; set; }
        public int? GapYearTo { get; set; }

        public bool HasUploadedGapCertificate { get; set; }
        public string? GapCertificateDocName { get; set; }
        public string? GapCertificateDocPath { get; set; }

        public bool HasUploadedBankStatement { get; set; }

        public bool IsWFH { get; set; }
        public string? EmployeeCompanyCountryName { get; set; }
        public string? EmployeeCompanyState { get; set; }
        public string? EmployeeCompanyDistrict { get; set; }

        public bool HasForm50 { get; set; }
        public bool HasUploadedForm50 { get; set; }
        public string? Form50DocName { get; set; }
        public string? Form50DocFilePath { get; set; }

        public bool HasUploadedExperienceLetter { get; set; }
        public string? ExperienceLetterDocName { get; set; }
        public string? ExperienceLetterDocPath { get; set; }

        public bool HasUploadedJoiningLetter { get; set; }
        public string? JoiningLetterDocName { get; set; }
        public string? JoiningLetterDocPath { get; set; }

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
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsExperienceVerified { get; set; }
        public bool IsExperienceVerifiedByMail { get; set; }
        public bool IsExperienceVerifiedByCall { get; set; }
        public long? InfoVerifiedById { get; set; }
        public bool HasBankStatementUploaded { get; set; }
        public string? BankStatementDocName { get; set; }
        public string? BankStatementDocPath { get; set; }
        public bool IsEditAllowed { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }
        public bool IsInfoVerified { get; set; }

        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public long? SoftDeletedById { get; set; }

        public long AddedById { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        public List<EmployeeExperiencePayslipUploadDTO> Payslips { get; set; } = new();
    }
    public class EmployeeExperiencePayslipUploadDTO
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long ExperienceDetailId { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public bool HasUploadedPayslip { get; set; }
        public string? PayslipDocName { get; set; }
        public string? PayslipDocPath { get; set; }

        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public long? SoftDeletedById { get; set; }

        public long AddedById { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }


}
