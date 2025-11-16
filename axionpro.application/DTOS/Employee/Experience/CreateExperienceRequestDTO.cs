using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Experience
{

 public class CreateExperienceRequestDTO: BaseRequest
    {
        public string? EmployeeId { get; set; }

        // Company Info
        public string? CompanyName { get; set; }
        public bool IsLatestCompany { get; set; }
        public string? ExperienceInMonths { get; set; }    // Better than int years

        // Gap Details
        public bool IsAnyGap { get; set; }
        public string? ReasonOfGap { get; set; }
        public DateTime? GapFrom { get; set; }
        public DateTime? GapTo { get; set; }
        public bool HasUploadedGapCertificate { get; set; }
        public IFormFile? GapCertificateDocument { get; set; }

        // Work Mode
        public bool IsWFH { get; set; }

        // Company Location Info
        public string? CompanyCountry { get; set; }
        public string? CompanyState { get; set; }
        public string? CompanyDistrict { get; set; }

        // Form 50
        public bool HasForm50 { get; set; }
        public bool HasUploadedForm50 { get; set; }
        public IFormFile? Form50Document { get; set; }

        // Experience Letter
        public bool HasUploadedExperienceLetter { get; set; }
        public IFormFile? ExperienceLetterDocument { get; set; }

        // Joining Letter
        public bool HasUploadedJoiningLetter { get; set; }
        public IFormFile? JoiningLetterDocument { get; set; }

        // Company Internal Employee ID
        public string? CompanyEmployeeId { get; set; }

        // Reference Person Info
        public string? ColleagueName { get; set; }
        public string? ColleagueDesignation { get; set; }
        public string? ColleagueContactNumber { get; set; }

        // Reporting Manager Info
        public string? ReportingManagerName { get; set; }
        public string? ReportingManagerNumber { get; set; }
        public string? VerificationEmail { get; set; }

        // Other Info
        public string? ReasonForLeaving { get; set; }
        public string? Remark { get; set; }
        public string? Designation { get; set; }

        // Dates
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Added By
        public long AddedById { get; set; }
    }

}
