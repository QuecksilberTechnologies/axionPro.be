using axionpro.application.Common.Helpers;
using axionpro.domain.Entity;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeBulkCode")]
public sealed class EmployeeCodePatternFormatterTests
{
    #region Joining Date And Existing Pattern

    [Test]
    public void Original_joining_date_supplies_year_and_month()
    {
        var pattern = Pattern();
        pattern.IncludeYear = true;
        pattern.IncludeMonth = true;

        var code = EmployeeCodePatternFormatter.Format(pattern, new DateTime(2022, 3, 15), null, 145);

        Assert.That(code, Is.EqualTo("QT/2022/MAR/0145"));
    }

    [TestCase("/", "QT/2022/MAR/0145")]
    [TestCase("", "QT2022MAR0145")]
    public void Matching_code_preserves_running_number_with_or_without_separator(string separator, string code)
    {
        var pattern = Pattern();
        pattern.Separator = separator;
        pattern.IncludeYear = true;
        pattern.IncludeMonth = true;

        var matches = EmployeeCodePatternFormatter.TryGetRunningNumber(
            pattern, new DateTime(2022, 3, 15), null, code, out var number);

        Assert.Multiple(() =>
        {
            Assert.That(matches, Is.True);
            Assert.That(number, Is.EqualTo(145));
        });
    }

    [TestCase("QT/0000")]
    [TestCase("QT/145")]
    [TestCase("QT/00145")]
    [TestCase("OTHER/0145")]
    [TestCase("QT/+145")]
    [TestCase("QT/0145 ")]
    public void Nonmatching_codes_are_not_silently_normalized(string code)
    {
        Assert.That(EmployeeCodePatternFormatter.TryGetRunningNumber(
            Pattern(), default, null, code, out _), Is.False);
    }

    [Test]
    public void Numeric_prefix_is_not_part_of_the_sequence()
    {
        var pattern = Pattern();
        pattern.Prefix = "QT99";
        pattern.Separator = "";

        Assert.That(EmployeeCodePatternFormatter.TryGetRunningNumber(
            pattern, default, null, "QT990145", out var number), Is.True);
        Assert.That(number, Is.EqualTo(145));
    }

    [Test]
    public void Highest_preserved_code_determines_next_number()
    {
        Assert.That(EmployeeCodePatternFormatter.NextRunningNumber(new[] { 200, 145 }), Is.EqualTo(201));
    }

    [Test]
    public void Sequence_overflow_is_rejected()
    {
        Assert.Throws<OverflowException>(() =>
            EmployeeCodePatternFormatter.NextRunningNumber(new[] { int.MaxValue }));
    }

    [Test]
    public void Date_dependent_pattern_rejects_missing_joining_date()
    {
        var pattern = Pattern();
        pattern.IncludeYear = true;
        Assert.Throws<ArgumentException>(() => EmployeeCodePatternFormatter.Format(pattern, default, null, 1));
    }

    [Test]
    public void Department_dependent_pattern_rejects_missing_department()
    {
        var pattern = Pattern();
        pattern.IncludeDepartment = true;
        Assert.Throws<ArgumentException>(() => EmployeeCodePatternFormatter.Format(pattern, default, null, 1));
    }

    private static EmployeeCodePattern Pattern()
    {
        return new EmployeeCodePattern
        {
            Prefix = "QT",
            Separator = "/",
            RunningNumberLength = 4
        };
    }

    [Test]
    public void Pattern_preview_recodes_admin_and_other_employee_without_mutating_them()
    {
        var current = Pattern();
        var proposed = Pattern();
        proposed.Prefix = "EMP";
        proposed.IncludeYear = true;
        var employees = new[] { Employee(1, "QT/0145"), Employee(2, "QT/0200") };

        var preview = EmployeeCodePatternFormatter.PreviewChanges(8, current, proposed, employees);

        Assert.Multiple(() =>
        {
            Assert.That(preview.CanCommit, Is.True);
            Assert.That(preview.Employees.Select(row => row.ProposedCode), Is.EqualTo(new[] { "EMP/2022/0145", "EMP/2022/0200" }));
            Assert.That(preview.LastUsedNumber, Is.EqualTo(200));
            Assert.That(employees[0].EmployementCode, Is.EqualTo("QT/0145"));
            Assert.That(preview.Applied, Is.False);
        });
    }

    [Test]
    public void Changed_employee_code_invalidates_preview_fingerprint()
    {
        var employee = Employee(1, "QT/0145");
        var first = EmployeeCodePatternFormatter.PreviewChanges(8, Pattern(), Pattern(), new[] { employee });
        employee.EmployementCode = "QT/0146";
        var second = EmployeeCodePatternFormatter.PreviewChanges(8, Pattern(), Pattern(), new[] { employee });
        Assert.That(first.PreviewHash, Is.Not.EqualTo(second.PreviewHash));
    }

    [Test]
    public void Duplicate_proposed_codes_block_the_entire_preview()
    {
        var preview = EmployeeCodePatternFormatter.PreviewChanges(8, Pattern(), Pattern(),
            new[] { Employee(1, "QT/0145"), Employee(2, "QT/0145") });
        Assert.That(preview.CanCommit, Is.False);
        Assert.That(preview.Employees.All(row => row.Errors.Count > 0), Is.True);
    }

    [Test]
    public void Foreign_employee_blocks_preview()
    {
        var employee = Employee(1, "QT/0145");
        employee.TenantId = 9;
        var preview = EmployeeCodePatternFormatter.PreviewChanges(8, Pattern(), Pattern(), new[] { employee });
        Assert.That(preview.CanCommit, Is.False);
    }

    [Test]
    public void Missing_original_date_is_not_replaced_with_today()
    {
        var employee = Employee(1, "QT/0145");
        employee.DateOfOnBoarding = null;
        var proposed = Pattern();
        proposed.IncludeYear = true;
        var preview = EmployeeCodePatternFormatter.PreviewChanges(8, Pattern(), proposed, new[] { employee });
        Assert.That(preview.CanCommit, Is.False);
    }

    private static Employee Employee(long id, string code)
    {
        return new Employee
        {
            Id = id,
            TenantId = 8,
            FirstName = "Sample",
            EmployementCode = code,
            DateOfOnBoarding = new DateTime(2022, 3, 15),
            DepartmentId = 6
        };
    }

    [TestCase(2026, true)]
    [TestCase(2025, false)]
    public void Legacy_creation_year_requires_exact_persisted_date(int creationYear, bool allowed)
    {
        var current = Pattern();
        current.IncludeYear = true;
        var employee = Employee(1, "QT/2026/0001");
        employee.DateOfOnBoarding = new DateTime(2018, 7, 24);
        employee.AddedDateTime = new DateTime(creationYear, 9, 9);

        var preview = EmployeeCodePatternFormatter.PreviewChanges(8, current, current, new[] { employee });

        Assert.That(preview.CanCommit, Is.EqualTo(allowed));
        if (allowed)
        {
            Assert.That(preview.Employees[0].ProposedCode, Is.EqualTo("QT/2018/0001"));
            employee.AddedDateTime = employee.AddedDateTime.AddDays(1);
            var changed = EmployeeCodePatternFormatter.PreviewChanges(8, current, current, new[] { employee });
            Assert.That(changed.PreviewHash, Is.Not.EqualTo(preview.PreviewHash));
        }
    }

    #endregion
}
