using axionpro.application.DTOS.Employee.Education;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers.PercentageHelper
{
    public static class PercentageHelper
    {
        // ⭐ Each Record Education Completion Calculation
        public static double CalculateEducationCompletion(
            GetEducationResponseDTO edu)
        {
            if (edu == null) return 0;

            bool[] fields =
            {
            !string.IsNullOrWhiteSpace(edu.Degree),
            !string.IsNullOrWhiteSpace(edu.InstituteName),
            edu.StartDate != null,
            edu.EndDate != null,
            !string.IsNullOrWhiteSpace(edu.ScoreValue),
            !string.IsNullOrWhiteSpace(edu.ScoreType),
            !string.IsNullOrWhiteSpace(edu.GradeDivision),
            edu.HasEducationDocUploded == true   // ✅ true only
        };

            int filled = fields.Count(f => f);
            int totalFields = fields.Length;

            return Math.Round((filled / (double)totalFields) * 100, 0);
        }
    }

}
