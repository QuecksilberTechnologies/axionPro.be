using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Education;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers.PercentageHelper
{
    public static class CompletionSectionCalculator
    {
        // Calculates SECTION completion (average of rows)
        public static CompletionSectionDTO CalculateBankSection(
            IEnumerable<GetBankResponseDTO> records)
        {
            if (records == null || !records.Any())
                return new CompletionSectionDTO
                {
                    SectionName = "Bank",
                    CompletionPercent = 0,
                    IsSectionCreate = false
                };

            // Row-level % reused (NO DUPLICATION)
            double average = records
                .Average(r => CompletionCalculatorHelper.BankPropCalculate(r));

            var first = records.First();

            return new CompletionSectionDTO
            {
                SectionName = "Bank",
                CompletionPercent = Math.Round(average, 2),
                IsInfoVerified = first.IsInfoVerified,
                IsEditAllowed = first.IsEditAllowed,
                IsSectionCreate = true
            };
        }
    }
}
