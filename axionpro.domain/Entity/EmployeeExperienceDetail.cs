using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class EmployeeExperienceDetail
{
 
        // ---------------------------------------------------------
        // 🔹 Basic Identifiers
        // ---------------------------------------------------------
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public long? EmployeeExperienceId { get; set; }

        // ---------------------------------------------------------
        // 🔹 Employment Information
        // ---------------------------------------------------------
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? EmployeeIdOfCompany { get; set; }
        public bool IsInfoLatestYear { get; set; }
        public int Experience { get; set; }
        public bool IsWFH { get; set; }

        // ---------------------------------------------------------
        // 🔹 Employment Duration
        // ---------------------------------------------------------
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }

        // ---------------------------------------------------------
        // 🔹 Colleague & Reporting Manager Info
        // ---------------------------------------------------------
        public string? ColleagueName { get; set; }
        public string? ColleagueDesignation { get; set; }
        public string? ColleagueContactNumber { get; set; }
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }
        public string? VerificationEmail { get; set; }

        // ---------------------------------------------------------
        // 🔹 Gap Details
        // ---------------------------------------------------------
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }
        public DateTime? GapYearFrom { get; set; }
        public DateTime? GapYearTo { get; set; }
        public bool HasUploadedGapCertificate { get; set; }
        public string? GapCertificateDocName { get; set; }
        public string? GapCertificateDocPath { get; set; }

        // ---------------------------------------------------------
        // 🔹 Location (Local or Foreign)
        // ---------------------------------------------------------
        public int WorkingCountryId { get; set; }
        public int WorkingStateId { get; set; }
        public int WorkingDistrictId { get; set; }
        public bool IsForeignExperience { get; set; }

        // ---------------------------------------------------------
        // 🔹 Taxation / Finance Documents
        // ---------------------------------------------------------
        public bool HasTaxationDoc { get; set; }
        public bool HasUploadedTaxationDoc { get; set; }
        public string? TaxationDocFileName { get; set; }
        public string? TaxationDocFilePath { get; set; }
        public bool? HasBankStatementUploaded { get; set; }
        public string? BankStatementDocName { get; set; }
        public string? BankStatementDocPath { get; set; }

        // ---------------------------------------------------------
        // 🔹 Experience Proof Documents
        // ---------------------------------------------------------
        public bool HasUploadedExperienceLetter { get; set; }
        public string? ExperienceLetterDocName { get; set; }
        public string? ExperienceLetterDocPath { get; set; }

        public bool HasUploadedJoiningLetter { get; set; }
        public string? JoiningLetterDocName { get; set; }
        public string? JoiningLetterDocPath { get; set; }

        // ---------------------------------------------------------
        // 🔹 Foreign Experience Documents
        // ---------------------------------------------------------
        public string? VisaType { get; set; }
        public string? WorkPermitNumber { get; set; }

        public string? VisaDocName { get; set; }
        public string? VisaDocPath { get; set; }

        public string? WorkPermitDocName { get; set; }
        public string? WorkPermitDocPath { get; set; }

        public string? ImmigrationStampDocName { get; set; }
        public string? ImmigrationStampDocPath { get; set; }

        public string? ForeignContractDocName { get; set; }
        public string? ForeignContractDocPath { get; set; }

        public bool? HasVisaUploaded { get; set; }
        public bool? HasWorkPermitUploaded { get; set; }
        public bool? HasImmigrationStampUploaded { get; set; }
        public bool? HasForeignContractUploaded { get; set; }

        // ---------------------------------------------------------
        // 🔹 Verification Info
        // ---------------------------------------------------------
        public bool? IsExperienceVerified { get; set; }
        public bool? IsExperienceVerifiedByMail { get; set; }
        public bool? IsExperienceVerifiedByCall { get; set; }
        public long? InfoVerifiedById { get; set; }
        public DateTime? InfoVerifiedDateTime { get; set; }
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }

        // ---------------------------------------------------------
        // 🔹 System Fields (Audit)
        // ---------------------------------------------------------
        public bool IsActive { get; set; }
        public bool? IsSoftDeleted { get; set; }
        public long? SoftDeletedById { get; set; }

        public long AddedById { get; set; }
        public long? UpdatedById { get; set; }

        public DateTime AddedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }

        // ---------------------------------------------------------
        // 🔹 Navigation Properties
        // ---------------------------------------------------------
        public virtual Employee Employee { get; set; } = null!;
        public virtual EmployeeExperience? EmployeeExperience { get; set; }
        public virtual ICollection<EmployeeExperiencePayslipUpload> EmployeeExperiencePayslipUploads { get; set; }
            = new List<EmployeeExperiencePayslipUpload>();
    }

 


