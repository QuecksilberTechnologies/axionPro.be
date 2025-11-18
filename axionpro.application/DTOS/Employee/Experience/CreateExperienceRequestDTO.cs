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
        // ------------------ IDENTIFIERS ------------------
        public string? UserEmployeeId { get; set; }
        public long _UserEmployeeId { get; set; }
        public long _EmployeeExperienceId { get; set; }
        public string? EmployeeId { get; set; }
        public long _Id { get; set; }
        public long _EmployeeId { get; set; }

        // ------------------ FINANCIAL ------------------
        public decimal _CTC { get; set; }
        public string? CTC { get; set; }

        // ------------------ STATUS FLAGS ------------------
        public bool IsEditAllowed { get; set; }
        public bool IsFresher { get; set; }
        public bool IsGap { get; set; }
        public bool IsActive { get; set; }
        public bool HasEPFAccount { get; set; }

        // ------------------ AUDIT ------------------
        public string? AddedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public string? SoftDeletedById { get; set; }
        public DateTime? DeletedDateTime { get; set; }

        // ------------------ OTHERS ------------------
        public string? Comment { get; set; }

        public List<EmployeeExperienceDetailDTO> ExperienceDetails { get; set; } = new();
    }

    // =====================================================================
    //  EXPERIENCE DETAILS DTO
    // =====================================================================
    public class EmployeeExperienceDetailDTO
    {
        // ------------------ IDENTIFIERS ------------------
        public string? Id { get; set; }
        public long? _EmployeeExperienceDetail { get; set; }
        public string? EmployeeId { get; set; }
        public long _Id { get; set; }
        public long _EmployeeId { get; set; }
        public bool _IsAnyGap { get; set; }

        // ------------------ BASIC COMPANY INFO ------------------
        public string CompanyName { get; set; } = string.Empty;
        public string? Experience { get; set; }
        public string? Designation { get; set; }
        public bool IsInfoLatestYear { get; set; }

        // ------------------ WORK LOCATION ------------------
        public bool IsWFH { get; set; }
        public string? WorkingCountryId { get; set; }
        public string? WorkingStateId { get; set; }
        public string? WorkingDistrictId { get; set; }

        // ------------------ GAP DETAILS ------------------
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }
        public DateTime? GapYearFrom { get; set; }
        public DateTime? GapYearTo { get; set; }
        public bool HasUploadedGapCertificate { get; set; }
        public string? GapCertificateDocName { get; set; }
        public string? GapCertificateDocPath { get; set; }
        public IFormFile? GapCertificateDocument { get; set; }

        // ------------------ TAXATION ------------------
        public bool HasUploadedTaxationDoc { get; set; }
        public IFormFile? TaxationDocument { get; set; }
        public bool HasTaxationDoc { get; set; }

        // ------------------ EXPERIENCE LETTER ------------------
        public bool HasUploadedExperienceLetter { get; set; }
        public string? ExperienceLetterDocName { get; set; }
        public string? ExperienceLetterDocPath { get; set; }
        public IFormFile? ExperienceLetterDocument { get; set; }

        // ------------------ JOINING LETTER ------------------
        public bool HasUploadedJoiningLetter { get; set; }
        public string? JoiningLetterDocName { get; set; }
        public string? JoiningLetterDocPath { get; set; }
        public IFormFile? JoiningLetterDocument { get; set; }

        // ------------------ BANK STATEMENT ------------------
        public bool HasBankStatementUploaded { get; set; }
        public string? BankStatementDocName { get; set; }
        public string? BankStatementDocPath { get; set; }
        public IFormFile? BankStatementDocument { get; set; }

        // ------------------ FOREIGN EXPERIENCE ------------------
        public bool IsForeignExperience { get; set; }
        public string? VisaType { get; set; }
        public bool  HasVisaUploaded { get; set; }
        public bool HasWorkPermitUploaded { get; set; }
        public bool HasImmigrationStampUploaded { get; set; }
        public bool HasForeignContractUploaded { get; set; }
   
        public string? WorkPermitNumber { get; set; }
        public IFormFile? VisaDocument { get; set; }
        public IFormFile? WorkPermitDocument { get; set; }
        public IFormFile? ImmigrationStampDocument { get; set; }
        public IFormFile? ForeignContractDocument { get; set; }

        // ------------------ VERIFICATION DETAILS ------------------
        public bool IsExperienceVerified { get; set; }
        public bool IsExperienceVerifiedByMail { get; set; }
        public bool IsExperienceVerifiedByCall { get; set; }
        public long? InfoVerifiedById { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }

        // ------------------ CONTACT DETAILS ------------------
        public string? EmployeeIdOfCompany { get; set; }
        public string? ColleagueName { get; set; }
        public string? ColleagueDesignation { get; set; }
        public string? ColleagueContactNumber { get; set; }
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }
        public string? VerificationEmail { get; set; }

        // ------------------ OTHER INFO ------------------
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsInfoVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public bool IsEditAllowed { get; set; }

        // ------------------ AUDIT ------------------
        public string? SoftDeletedById { get; set; }
        public string? AddedById { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        // ------------------ PAYSLIPS ------------------
        public List<EmployeeExperiencePayslipUploadDTO> Payslips { get; set; } = new();
    }

    // =====================================================================
    //  PAYSLIP UPLOAD DTO
    // =====================================================================
    public class EmployeeExperiencePayslipUploadDTO
    {
        // ------------------ IDENTIFIERS ------------------
        public string? Id { get; set; }
        public string? EmployeeId { get; set; }
        public long _Id { get; set; }
        public long _EmployeeId { get; set; }
        public string? ExperienceDetailId { get; set; }
        public long _ExperienceDetailId { get; set; }

        // ------------------ YEAR & MONTH ------------------
        public int _Year { get; set; }
        public int _Month { get; set; }
        public string? Year { get; set; }
        public string? Month { get; set; }

        // ------------------ DOCUMENT ------------------
        public bool HasUploadedPayslip { get; set; }
        public string? PayslipDocName { get; set; }
        public string? PayslipDocPath { get; set; }
        public IFormFile? PayslipDocument { get; set; }

        // ------------------ STATUS/AUDIT ------------------
        public bool IsActive { get; set; }
        public bool IsSoftDeleted { get; set; }
        public string? SoftDeletedById { get; set; }
        public string? AddedById { get; set; }
        public string? UpdatedById { get; set; }
        public DateTime AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }


}
