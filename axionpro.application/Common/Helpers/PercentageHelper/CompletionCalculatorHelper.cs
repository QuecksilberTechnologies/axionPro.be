using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.domain.Entity;
using Newtonsoft.Json.Linq;

namespace axionpro.application.Common.Helpers.PercentageHelper
{
    public static class CompletionCalculatorHelper
    {
        // =========================================================
        // EMPLOYEE BASIC COMPLETION
        // =========================================================
        public static double EmployeePropCalculate(Employee emp, bool hasPrimaryImage)
        {
            if (emp == null)
                return 0;

            int[] checks =
            {
                IsFilled(emp.FirstName),
                IsFilled(emp.LastName),
                emp.DateOfOnBoarding != null ? 1 : 0,
                emp.DesignationId > 0 ? 1 : 0,
                emp.DepartmentId > 0 ? 1 : 0,
                IsFilled(emp.OfficialEmail),
                emp.IsActive ? 1 : 0,
                emp.IsEditAllowed == false ? 1 : 0,
                emp.IsInfoVerified == true ? 1 : 0,
                hasPrimaryImage ? 1 : 0,
                IsFilled(emp.BloodGroup),
                IsFilled(emp.MobileNumber),
                IsFilled(emp.EmergencyContactNumber),
                IsFilled(emp.EmergencyContactPerson)
            };

            return CalculatePercentage(checks);
        }
        // =========================================================
        // Experience Prop Calculate  
        // =========================================================
        public static double ExperiencePropCalculate(GetEmployeeExperienceResponseDTO exp)
        {
            if (exp == null)
                return 0;

            int[] checks =
            {
        // 🔹 Basic
        exp.Ctc.HasValue ? 1 : 0,

        // 🔹 Job Info
        IsFilled(exp.CompanyName),
        IsFilled(exp.Designation),
        IsFilled(exp.EmployeeIdOfCompany),

        exp.StartDate != null ? 1 : 0,
        exp.EndDate != null ? 1 : 0,

        exp.Experience.HasValue ? 1 : 0,

        // 🔹 Location
        exp.WorkingCountryId.HasValue && exp.WorkingCountryId > 0 ? 1 : 0,
        exp.WorkingStateId.HasValue && exp.WorkingStateId > 0 ? 1 : 0,
        exp.WorkingDistrictId.HasValue && exp.WorkingDistrictId > 0 ? 1 : 0,

        // 🔹 Flags
        exp.IsWFH ? 1 : 0,
        exp.IsForeignExperience ? 1 : 0,

        // 🔹 Exit
        IsFilled(exp.ReasonForLeaving),
        IsFilled(exp.Remark),

        // 🔹 Reporting
        IsFilled(exp.ColleagueName),
        IsFilled(exp.ColleagueDesignation),
        IsFilled(exp.ColleagueContactNumber),

        IsFilled(exp.ReportingManagerName),
        IsFilled(exp.ReportingManagerNumber),

        IsFilled(exp.VerificationEmail),

        // 🔹 Gap
        exp.IsAnyGap ? 1 : 0,
        exp.IsAnyGap && IsFilled(exp.ReasonOfGap) == 1 ? 1 : 0,

        //exp.GapYearFrom != null ? 1 : 0,
        //exp.GapYearTo != null ? 1 : 0,

        // 🔹 Verification
        exp.IsExperienceVerified == true ? 1 : 0,
        exp.IsExperienceVerifiedByMail == true ? 1 : 0,
        exp.IsExperienceVerifiedByCall == true ? 1 : 0,

        exp.IsInfoVerified == true ? 1 : 0,
        exp.IsEditAllowed == false ? 1 : 0,

        // 🔹 Documents (🔥 important)       
        (exp.Documents != null && exp.Documents.Any()) ? 1 : 0
     };

            return CalculatePercentage(checks);
        }

        // =========================================================
        // BANK DETAILS COMPLETION
        // =========================================================
        public static double BankPropCalculate_NoPrimary(GetBankResponseDTO record )
        {

            if (record == null)
                return 0;

            int[] checks =
            {
                IsFilled(record.BankName),
                IsFilled(record.AccountNumber),
                IsFilled(record.IFSCCode),
                IsFilled(record.BranchName),
                IsFilled(record.AccountType),
                record.IsEditAllowed  == false ? 1 : 0,
                record.IsInfoVerified == true ? 1 : 0,
                 // 🔥 Business Rule
                 record.IsPrimaryAccount == false ?  0 : 1

            };

            return CalculatePercentage(checks);
        }
        public static double BankPropCalculate(GetBankResponseDTO record)
        {

            if (record == null)
                return 0;

            int[] checks =
            {
                IsFilled(record.BankName),
                IsFilled(record.AccountNumber),
                IsFilled(record.IFSCCode),
                IsFilled(record.BranchName),
                IsFilled(record.AccountType),
               record.IsEditAllowed  == false ? 1 : 0,
                record.IsInfoVerified == true ? 1 : 0,

                // 🔥 Business Rule
                record.IsPrimaryAccount == true ? (record.HasChequeDocUploaded ? 1 : 0)  : 1


            };

            return CalculatePercentage(checks);
        }

        // =========================================================
        // EDUCATION COMPLETION ✅ FIXED
        // =========================================================
        public static double EduPropCalculate(GetEducationResponseDTO edu)
        {
            if (edu == null)
                return 0;

            int[] checks =
            {
                IsFilled(edu.Degree),
                IsFilled(edu.InstituteName),
                edu.StartDate != null ? 1 : 0,
                edu.EndDate != null ? 1 : 0,
                IsFilled(edu.ScoreValue),
                IsFilled(edu.ScoreType),
                IsFilled(edu.GradeDivision),
                edu.HasEducationDocUploded == true ? 1 : 0
            };

            return CalculatePercentage(checks);
        }
        // =========================================================
        // DEPENDENT DETAILS COMPLETION
        // =========================================================
        public static double DependentPropCalculate(GetDependentResponseDTO dep)
        {
            if (dep == null)
                return 0;

            int[] checks =
            {
        // 🔹 Core identity
        !string.IsNullOrWhiteSpace(dep.DependentName) ? 1 : 0,

        // ✅ RELATION (INT ENUM VALUE)
        dep.Relation.HasValue && dep.Relation.Value > 0 ? 1 : 0,

        // 🔹 Personal info
        dep.DateOfBirth.HasValue ? 1 : 0,

        // 🔹 Flags
        dep.IsCoveredInPolicy.HasValue ? 1 : 0,
        dep.IsMarried.HasValue ? 1 : 0,

        // 🔹 Business rule
        // Proof mandatory ONLY if covered in policy
        dep.IsCoveredInPolicy == true
            ? (dep.HasProofUploaded == true ? 1 : 0)
            : 1
    };

            return CalculatePercentage(checks);
        }


        // =========================================================
        // CONTACT DETAILS COMPLETION
        // =========================================================
        public static double ContactPropCalculate_NoPrimary(GetContactResponseDTO contact)
        {
            if (contact == null)
                return 0;

            int[] checks =
            {
                // 🔹 Basic Contact Info
                 // 🔹 Core Identity
                contact.ContactType > 0 ? 1 : 0,
                contact.Relation > 0 ? 1 : 0,

                // 🔹 Contact Info
                IsFilled(contact.ContactName),
                IsFilled(contact.ContactNumber),

                // 🔹 Address Basics
                IsFilled(contact.HouseNo),
                IsFilled(contact.ContactName),
                IsFilled(contact.ContactNumber),
                  // 🔥 Business Rule
                contact.IsPrimary == false?  0  : 1,
                contact.ContactType > 0 ? 1 : 0,

                // 🔹 Address Info
                contact.CountryId > 0 ? 1 : 0,
                contact.StateId > 0 ? 1 : 0,
                contact.DistrictId > 0 ? 1 : 0,
                 contact.IsEditAllowed  == false ? 1 : 0,
                contact.IsInfoVerified == true ? 1 : 0,
                // 🔹 Optional but recommended
                IsFilled(contact.Address),
 
            };

            return CalculatePercentage(checks);
        }
        public static double ContactPropCalculate(GetContactResponseDTO contact)
        {
            if (contact == null)
                return 0;

            int[] checks =
            {
                // 🔹 Basic Contact Info
                 // 🔹 Core Identity
                contact.ContactType > 0 ? 1 : 0,
                contact.Relation > 0 ? 1 : 0,

                // 🔹 Contact Info
                IsFilled(contact.ContactName),
                IsFilled(contact.ContactNumber),

                // 🔹 Address Basics
                IsFilled(contact.HouseNo),
                IsFilled(contact.ContactName),
                IsFilled(contact.ContactNumber),
                contact.ContactType > 0 ? 1 : 0,

                // 🔹 Address Info
                contact.CountryId > 0 ? 1 : 0,
                contact.StateId > 0 ? 1 : 0,
                contact.DistrictId > 0 ? 1 : 0,

                // 🔹 Optional but recommended
                IsFilled(contact.Address),
                contact.IsEditAllowed  == false ? 1 : 0,
                contact.IsInfoVerified == true ? 1 : 0,

                // 🔥 Business Rule:
                // Primary contact must have alternate OR email
                //contact.IsPrimary == true  ? (IsFilled(contact.AlternateNumber) == 1 || IsFilled(contact.Email) == 1 ? 1 : 0)
                //    : 1
                contact.IsPrimary == true  ?1: 0
            };

            return CalculatePercentage(checks);
        }

        // =========================================================
        // COMMON HELPERS
        // =========================================================
        private static double CalculatePercentage(int[] checks)
        {
            int completed = checks.Sum();
            int total = checks.Length;

            return total == 0
                ? 0
                : Math.Round((completed / (double)total) * 100, 0);
        }

        private static int IsFilled(string? value)
            => string.IsNullOrWhiteSpace(value) ? 0 : 1;
    }
}
