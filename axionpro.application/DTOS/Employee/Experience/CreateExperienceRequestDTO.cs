using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
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
       
        public string? Comment { get; set; }
        
        public string? OldCompanyEmployeeId { get; set; }
        public string? ExperienceLetterDocName { get; set; }
        public string? ExperienceLetterPath { get; set; }

        public string? BankStatementDocName { get; set; }
        public string? BankStatementPath { get; set; }

        public string? JoiningLetterDocName { get; set; }
        public string? JoiningLetterPath { get; set; }

        public string? PaySlipOneMonthDocName { get; set; }
        public string? PaySlipOneMonthPath { get; set; }

        public string? PaySlipSecondMonthDocName { get; set; }
        public string? PaySlipSecondMonthPath { get; set; }

        public string? PaySlipThirdMonthDocName { get; set; }
        public string? PaySlipThirdMonthPath { get; set; }

        public string? Form50DocPath { get; set; }
        public string? Form50DocName { get; set; }

        public bool? HasEPFAccount { get; set; }
        public string? UANNumber { get; set; }
        public bool? HasForm50 { get; set; }      

        public IFormFile? ExperienceCertificatePDF { get; set; }
        public IFormFile? JoiningLetterPDF { get; set; }
        public IFormFile? Form50CertificatePDF { get; set; }
        public IFormFile? RequiredMonthsBankStatementPDF { get; set; }
        public IFormFile? RequiredPaySlipOneMonthPDF { get; set; }
        public IFormFile? RequiredPaySlipSecondMonthPDF { get; set; }
        public IFormFile? RequiredPaySlipThirdMonthPDF { get; set; }

    }


}
