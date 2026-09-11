using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using axionpro.application.DTOS.Tenant;
using axionpro.domain.Entity;

namespace axionpro.application.Common.Helpers;

/// <summary>
/// Formats and recognizes employee codes using the existing tenant pattern fields.
/// Callers supply the original joining date; this helper never substitutes today's date.
/// </summary>
public static class EmployeeCodePatternFormatter
{
    #region Pattern Formatting

    /// <summary>
    /// Builds a code using the established prefix/year/month/department/sequence order.
    /// </summary>
    public static string Format(
        EmployeeCodePattern pattern,
        DateTime joiningDate,
        int? departmentId,
        int runningNumber)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        if (runningNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(runningNumber));
        }

        if (pattern.RunningNumberLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pattern.RunningNumberLength));
        }

        if ((pattern.IncludeYear || pattern.IncludeMonth) && joiningDate == default)
        {
            throw new ArgumentException("Original joining date is required.", nameof(joiningDate));
        }

        if (pattern.IncludeDepartment && departmentId is not > 0)
        {
            throw new ArgumentException("Department is required by this pattern.", nameof(departmentId));
        }

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(pattern.Prefix))
        {
            parts.Add(pattern.Prefix);
        }

        if (pattern.IncludeYear)
        {
            parts.Add(joiningDate.Year.ToString(CultureInfo.InvariantCulture));
        }

        if (pattern.IncludeMonth)
        {
            parts.Add(joiningDate.ToString("MMM", CultureInfo.InvariantCulture).ToUpperInvariant());
        }

        if (pattern.IncludeDepartment)
        {
            parts.Add(departmentId!.Value.ToString(CultureInfo.InvariantCulture));
        }

        parts.Add(runningNumber.ToString(CultureInfo.InvariantCulture)
            .PadLeft(pattern.RunningNumberLength, '0'));

        return string.Join(pattern.Separator, parts);
    }

    #endregion

    #region Existing Code Recognition

    /// <summary>
    /// Recognizes a complete matching code without splitting on a potentially empty
    /// separator or treating digits in the prefix/year as part of the running number.
    /// </summary>
    public static bool TryGetRunningNumber(
        EmployeeCodePattern pattern,
        DateTime joiningDate,
        int? departmentId,
        string? code,
        out int runningNumber)
    {
        runningNumber = 0;
        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        var sample = Format(pattern, joiningDate, departmentId, 1);
        var prefixLength = sample.Length - pattern.RunningNumberLength;
        if (code.Length < prefixLength + pattern.RunningNumberLength ||
            !code.StartsWith(sample[..prefixLength], StringComparison.Ordinal))
        {
            return false;
        }

        var digits = code[prefixLength..];
        if (digits.Any(character => character is < '0' or > '9') ||
            !int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) ||
            parsed <= 0 ||
            !string.Equals(code, Format(pattern, joiningDate, departmentId, parsed), StringComparison.Ordinal))
        {
            return false;
        }

        runningNumber = parsed;
        return true;
    }

    /// <summary>
    /// Returns the next number after the highest preserved sequence, with overflow protection.
    /// </summary>
    public static int NextRunningNumber(IEnumerable<int> preservedNumbers)
    {
        ArgumentNullException.ThrowIfNull(preservedNumbers);
        var numbers = preservedNumbers.ToArray();
        if (numbers.Any(number => number <= 0))
        {
            throw new ArgumentException("Preserved running numbers must be positive.", nameof(preservedNumbers));
        }

        return checked(numbers.DefaultIfEmpty(0).Max() + 1);
    }

    #endregion

    #region Existing Employee Change Preview

    /// <summary>
    /// Preserves recognized sequences when changing a pattern. Unrecognized codes,
    /// missing dates and collisions block the entire change instead of being guessed.
    /// </summary>
    public static SaveEmployeeCodePatternResponseDTO PreviewChanges(
        long tenantId,
        EmployeeCodePattern? current,
        EmployeeCodePattern proposed,
        IReadOnlyList<Employee> employees)
    {
        var result = new SaveEmployeeCodePatternResponseDTO();
        foreach (var employee in employees.OrderBy(item => item.Id))
        {
            var row = new EmployeeCodeChangeDTO
            {
                EmployeeId = employee.Id,
                Name = string.Join(" ", new[] { employee.FirstName, employee.MiddleName, employee.LastName }
                    .Where(part => !string.IsNullOrWhiteSpace(part))),
                CurrentCode = employee.EmployementCode,
                JoiningDate = employee.DateOfOnBoarding
            };
            result.Employees.Add(row);
            try
            {
                if (employee.TenantId != tenantId)
                {
                    throw new ArgumentException("Employee does not belong to this tenant.");
                }

                var date = employee.DateOfOnBoarding ?? default;
                var matchesNew = TryGetRunningNumber(proposed, date, employee.DepartmentId,
                    employee.EmployementCode, out var number);
                if (!matchesNew && (current is null || !TryGetRunningNumber(current, date,
                        employee.DepartmentId, employee.EmployementCode, out number)))
                {
                    throw new ArgumentException("Existing code does not match the current or selected pattern. Correct it before confirming.");
                }

                row.ProposedCode = matchesNew
                    ? employee.EmployementCode
                    : Format(proposed, date, employee.DepartmentId, number);
                if (row.ProposedCode!.Length > 50)
                {
                    throw new ArgumentException("Employee code exceeds the existing 50-character field limit.");
                }

                result.LastUsedNumber = Math.Max(result.LastUsedNumber, number);
            }
            catch (ArgumentException exception)
            {
                row.Errors.Add(exception.Message);
            }
        }

        foreach (var duplicate in result.Employees.Where(row => row.ProposedCode is not null)
                     .GroupBy(row => row.ProposedCode, StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            foreach (var row in duplicate)
            {
                row.Errors.Add("Proposed employee code conflicts with another employee.");
            }
        }

        if (result.Employees.Any(row => row.Errors.Count > 0))
        {
            result.Errors.Add("Resolve employee code/date errors before confirming the pattern change.");
        }

        // Do not recycle an already issued number when the last employee was removed.
        result.LastUsedNumber = Math.Max(result.LastUsedNumber, current?.LastUsedNumber ?? 0);
        var snapshot = JsonSerializer.Serialize(new
        {
            tenantId,
            currentPattern = current is null ? null : new
            {
                current.Id, current.Prefix, current.Separator, current.IncludeYear,
                current.IncludeMonth, current.IncludeDepartment, current.RunningNumberLength,
                current.LastUsedNumber, current.UpdatedDateTime
            },
            proposedPattern = new
            {
                proposed.Prefix, proposed.Separator, proposed.IncludeYear,
                proposed.IncludeMonth, proposed.IncludeDepartment, proposed.RunningNumberLength
            },
            employees = employees.OrderBy(employee => employee.Id).Select(employee => new
            {
                employee.Id, employee.TenantId, employee.EmployementCode,
                employee.DateOfOnBoarding, employee.DepartmentId, employee.UpdatedDateTime
            })
        });
        result.PreviewHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(snapshot)));
        return result;
    }

    #endregion
}
