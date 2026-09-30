using axionpro.application.Exceptions;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Provides validation rules shared by Employee profile create and update handlers.
/// </summary>
public static class EmployeeProfileValidationHelper
{
    public static void RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationErrorException($"{fieldName} is required.");
    }

    public static void ValidateDateRange(
        DateTime? startDate,
        DateTime? endDate,
        string startFieldName = "Start date",
        string endFieldName = "End date")
    {
        if (startDate.HasValue && endDate.HasValue && endDate.Value.Date < startDate.Value.Date)
        {
            throw new ValidationErrorException(
                $"{endFieldName} cannot be before {startFieldName.ToLowerInvariant()}.");
        }
    }

    public static void ValidateDateOfBirth(DateTime? dateOfBirth)
    {
        if (dateOfBirth.HasValue && dateOfBirth.Value.Date > DateTime.UtcNow.Date)
            throw new ValidationErrorException("Date of birth cannot be in the future.");
    }

    public static void ValidateGap(
        bool hasGap,
        double gapYears,
        string? reason,
        string gapLabel)
    {
        if (!hasGap)
            return;

        if (gapYears <= 0)
            throw new ValidationErrorException($"{gapLabel} years must be greater than zero.");

        RequireText(reason, $"{gapLabel} reason");
    }
}
