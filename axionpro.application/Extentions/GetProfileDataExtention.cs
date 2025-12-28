using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Education;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Extentions
{
    public static class GetProfileDataExtention
    {
              // ------------------ EDUCATION ENTITY EXTENSION ------------------
        public static CompletionSectionDTO CalculateEducationCompletion(this IEnumerable<EmployeeEducation> items)
        {
            if (items == null || !items.Any())
                return CreateEmptyResponse("Education");

            int totalRows = items.Count();
            double totalPercent = 0;

            foreach (var edu in items)
            {
                int totalFields = 6;
                int filled = 0;

                if (!string.IsNullOrEmpty(edu.Degree)) filled++;
                if (!string.IsNullOrEmpty(edu.InstituteName)) filled++;
                if (edu.StartDate != null) filled++;
                if (edu.EndDate != null) filled++;
                if (edu.HasEducationDocUploded == true) filled++;
                if (edu.ScoreType != null && edu.ScoreType != 0) filled++;


                totalPercent += Math.Round((filled / (double)totalFields) * 100);
            }

            var first = items.First();
            double finalPercent = Math.Round(totalPercent / totalRows);

            return new CompletionSectionDTO
            {
                SectionName = "Education",
                CompletionPercent = finalPercent,
                IsInfoVerified = first.IsInfoVerified,
                IsEditAllowed = first.IsEditAllowed,
                IsSectionCreate = true
            };
        }


        // ------------------ EDUCATION DTO EXTENSION ------------------
   
        public static CompletionSectionDTO CalculateEducationCompletionDTO(
           this IEnumerable<EducationRowDTO> items)
        {
            if (items == null || !items.Any())
                return CreateEmptyResponse("Education");

            double totalPercent = items.Sum(edu =>
            {
                bool[] fields =
                {
            !string.IsNullOrWhiteSpace(edu.Degree),
            !string.IsNullOrWhiteSpace(edu.InstituteName),
            edu.StartDate != null,
            edu.EndDate != null,
            edu.HasEducationDocUploded == true,   // ✅ true only
            edu.ScoreType != null && edu.ScoreType != 0
        };

                int filled = fields.Count(f => f);
                int totalFields = fields.Length;

                return (filled / (double)totalFields) * 100;
            });

            var first = items.First();

            return new CompletionSectionDTO
            {
                SectionName = "Education",
                CompletionPercent = Math.Round(totalPercent / items.Count()),
                IsInfoVerified = first.IsInfoVerified,
                IsEditAllowed = first.IsEditAllowed,
                IsSectionCreate = true
            };
        }

        // ------------------ BANK DTO-----------------
        public static CompletionSectionDTO CalculateBankCompletionDTO(
           this IEnumerable<BankRowDTO> items)
        {
            if (items == null || !items.Any())
                return CreateEmptyResponse("Bank");

            double totalPercent = items.Sum(bank =>
            {
                bool[] fields =
                {
            !string.IsNullOrWhiteSpace(bank.BankName),
            !string.IsNullOrWhiteSpace(bank.BranchName),
            !string.IsNullOrWhiteSpace(bank.IFSCCode),
            bank.AccountNumber != null,
            bank.AccountType != null,
            bank.IsPrimaryAccount != null,
            bank.HasChequeDocUploaded == true   // ✅ true only
        };

                int filled = fields.Count(f => f);
                int totalFields = fields.Length;

                return (filled / (double)totalFields) * 100;
            });

            var first = items.First();

            return new CompletionSectionDTO
            {
                SectionName = "Bank",
                CompletionPercent = Math.Round(totalPercent / items.Count()),
                IsInfoVerified = first.IsInfoVerified,
                IsEditAllowed = first.IsEditAllowed,
                IsSectionCreate = true
            };
        }

        public static CompletionSectionDTO CalculateExperienceCompletion(this IEnumerable<dynamic> items)
        {
            if (items == null || !items.Any())
            {
                return new CompletionSectionDTO
                {
                    SectionName = "Experiecne",
                    CompletionPercent = 0,
                    IsInfoVerified = false,
                    IsEditAllowed = false,
                    IsSectionCreate = false
                };
            }

            int totalRows = 0;
            double totalPercent = 0;

            foreach (var item in items)
            {
                dynamic edu = item;   // ⭐⭐ IMPORTANT FIX

                totalRows++;

                int totalFields = 4;
                int filled = 0;

                if (!string.IsNullOrEmpty((string?)edu.Degree)) filled++;
                if (!string.IsNullOrEmpty((string?)edu.InstituteName)) filled++;
                if (edu.StartDate != null) filled++;
                if (edu.EndDate != null) filled++;

                double percent = Math.Round((filled / (double)totalFields) * 100, 0);

                totalPercent += percent;
            }

            dynamic first = items.First();

            double finalPercent = Math.Round(totalPercent / totalRows, 0);

            return new CompletionSectionDTO
            {
                SectionName = "Education",
                CompletionPercent = finalPercent,
                IsInfoVerified = first.IsInfoVerified,
                IsEditAllowed = first.IsEditAllowed,
                IsSectionCreate = true
            };
        }

        // ------------------ COMMON EMPTY FACTORY ------------------
        private static CompletionSectionDTO CreateEmptyResponse(string sectionName)
        {
            return new CompletionSectionDTO
            {
                SectionName = sectionName,
                CompletionPercent = 0,
                IsInfoVerified = false,
                IsEditAllowed = false,
                IsSectionCreate = false
            };
        }
    }

}





 



