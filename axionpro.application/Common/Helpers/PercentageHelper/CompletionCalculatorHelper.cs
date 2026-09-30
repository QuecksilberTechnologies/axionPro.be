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

            return EmployeeProfileCompletionCalculator.CalculateOverviewRow(emp, hasPrimaryImage);
        }
        // =========================================================
        // Experience Prop Calculate  
        // =========================================================
        public static double ExperiencePropCalculate(GetEmployeeExperienceResponseDTO exp)
        {
            if (exp == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateExperienceRow(exp);
        }

        // =========================================================
        // BANK DETAILS COMPLETION
        // =========================================================
        public static double BankPropCalculate_NoPrimary(GetBankResponseDTO record )
        {

            if (record == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateBankRow(record);
        }
        public static double BankPropCalculate(GetBankResponseDTO record)
        {

            if (record == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateBankRow(record);
        }

        // =========================================================
        // EDUCATION COMPLETION ✅ FIXED
        // =========================================================
        public static double EduPropCalculate(GetEducationResponseDTO edu)
        {
            if (edu == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateEducationRow(edu);
        }
        // =========================================================
        // DEPENDENT DETAILS COMPLETION
        // =========================================================
        public static double DependentPropCalculate(GetDependentResponseDTO dep)
        {
            if (dep == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateDependentRow(dep);
        }

        // =========================================================
        // CONTACT DETAILS COMPLETION
        // =========================================================
        public static double ContactPropCalculate_NoPrimary(GetContactResponseDTO contact)
        {
            if (contact == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateContactRow(contact);
        }
        public static double ContactPropCalculate(GetContactResponseDTO contact)
        {
            if (contact == null)
                return 0;

            return EmployeeProfileCompletionCalculator.CalculateContactRow(contact);
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
