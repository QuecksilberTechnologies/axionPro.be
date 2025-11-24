using axionpro.application.DTOS.Employee.CompletionPercentage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Extentions
{
    public static class EducationCompletionExtensions
    {
        public static CompletionSectionDTO CalculateEducationCompletion(this IEnumerable<dynamic> items)
        {
            if (items == null || !items.Any())
            {
                return new CompletionSectionDTO
                {
                    SectionName = "Education",
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
    }





}




