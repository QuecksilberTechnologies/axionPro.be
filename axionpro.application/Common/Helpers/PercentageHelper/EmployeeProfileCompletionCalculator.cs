using axionpro.application.DTOS.Employee.CompletionPercentage;

namespace axionpro.application.Common.Helpers.PercentageHelper;

/// <summary>
/// Provides the common aggregation rules used by Employee profile completion sections.
/// Verification is deliberately metadata and never contributes to completion percentage.
/// </summary>
public static class EmployeeProfileCompletionCalculator
{
    public static double CalculateRowPercentage(params bool[] requiredFields)
    {
        if (requiredFields.Length == 0)
            return 0;

        return Math.Round(
            requiredFields.Count(isComplete => isComplete) * 100d / requiredFields.Length,
            0);
    }

    public static CompletionSectionDTO CreateSection(
        string sectionName,
        IReadOnlyCollection<double> rowPercentages,
        IReadOnlyCollection<bool?>? verificationStates = null,
        IReadOnlyCollection<bool?>? editStates = null)
    {
        bool hasRows = rowPercentages.Count > 0;

        return new CompletionSectionDTO
        {
            SectionName = sectionName,
            CompletionPercent = hasRows ? Math.Round(rowPercentages.Average(), 0) : 0,
            IsInfoVerified = hasRows && verificationStates?.Count > 0
                ? verificationStates.All(value => value == true)
                : false,
            IsEditAllowed = hasRows && verificationStates?.All(value => value == true) == true
                ? false
                : editStates?.Any(value => value == true) == true,
            IsSectionCreate = hasRows
        };
    }

    public static CompletionSectionDTO CreateAssignmentSection(string sectionName, bool exists)
    {
        return CreateSection(sectionName, exists ? new[] { 100d } : Array.Empty<double>());
    }
}
