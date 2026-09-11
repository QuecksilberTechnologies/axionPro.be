using axionpro.application.Exceptions;
using axionpro.application.Common.Helpers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("EmployeeBulkCodeDatabase")]
[NonParallelizable]
public sealed class EmployeeCodePatternDatabaseTests
{
    private string _connection = null!;
    private EmployeeCodePattern _original = null!;
    private List<Employee> _employees = null!;

    #region Isolated Fixture

    [SetUp]
    public async Task Setup()
    {
        _connection = Environment.GetEnvironmentVariable("AXIONPRO_BULK_TEST_CONNECTION")!;
        if (string.IsNullOrWhiteSpace(_connection))
        {
            Assert.Ignore("Requires the isolated axionpro_bulk_test fixture; never use the target database.");
        }

        Assert.That(new NpgsqlConnectionStringBuilder(_connection).Database, Is.EqualTo("axionpro_bulk_test"));
        await using var context = Context();
        _original = await context.EmployeeCodePatterns.AsNoTracking().FirstAsync(pattern => pattern.IsActive);
        _employees = await context.Employees.AsNoTracking()
            .Where(employee => employee.TenantId == _original.TenantId && !employee.IsSoftDeleted).ToListAsync();
        Assert.That(_employees, Is.Not.Empty);
        // Normalize only this disposable fixture. Historical target records may
        // lack joining dates; production preview must continue to reject them.
        var joiningDate = new DateTime(2022, 3, 15, 0, 0, 0, DateTimeKind.Utc);
        for (var index = 0; index < _employees.Count; index++)
        {
            var employee = _employees[index];
            var code = EmployeeCodePatternFormatter.Format(
                _original, joiningDate, employee.DepartmentId, index + 1);
            await context.Employees.Where(item => item.Id == employee.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.DateOfOnBoarding, joiningDate)
                    .SetProperty(item => item.EmployementCode, code));
        }
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (_original is null)
        {
            return;
        }

        await using var context = Context();
        context.EmployeeCodePatterns.Update(_original);
        context.Employees.UpdateRange(_employees);
        await context.SaveChangesAsync();
    }

    private WorkforceDbContext Context()
    {
        return new WorkforceDbContext(new DbContextOptionsBuilder<WorkforceDbContext>().UseNpgsql(_connection).Options);
    }

    private static TenantEmployeeCodePatternRepository Repository(WorkforceDbContext context)
    {
        return new TenantEmployeeCodePatternRepository(context, NullLogger<TenantEmployeeCodePatternRepository>.Instance);
    }

    private EmployeeCodePattern Proposed()
    {
        return new EmployeeCodePattern
        {
            Prefix = "TEST",
            Separator = _original.Separator,
            RunningNumberLength = _original.RunningNumberLength,
            IncludeYear = _original.IncludeYear,
            IncludeMonth = _original.IncludeMonth,
            IncludeDepartment = _original.IncludeDepartment
        };
    }

    #endregion

    #region Preview And Commit

    [Test]
    public async Task Missing_active_pattern_can_be_created_after_preview_without_changing_matching_codes()
    {
        await using var context = Context();
        await context.EmployeeCodePatterns.Where(item => item.Id == _original.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.IsActive, false));
        EmployeeCodePattern NewPattern() => new()
        {
            Prefix = _original.Prefix, Separator = _original.Separator,
            RunningNumberLength = _original.RunningNumberLength, IncludeYear = _original.IncludeYear,
            IncludeMonth = _original.IncludeMonth, IncludeDepartment = _original.IncludeDepartment
        };
        try
        {
            var repository = Repository(context);
            var preview = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
                NewPattern(), true, false, null, CancellationToken.None);
            Assert.That(preview.CanCommit, Is.True);
            Assert.That(await context.EmployeeCodePatterns.CountAsync(item => item.TenantId == _original.TenantId && item.IsActive), Is.Zero);
            var saved = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
                NewPattern(), true, true, preview.PreviewHash, CancellationToken.None);
            Assert.That(saved.Applied, Is.True);
            Assert.That(saved.Employees.All(row => row.CurrentCode == row.ProposedCode), Is.True);
        }
        finally
        {
            await context.EmployeeCodePatterns.Where(item => item.TenantId == _original.TenantId && item.Id != _original.Id).ExecuteDeleteAsync();
        }
    }

    [Test]
    public async Task Archived_employee_code_blocks_reuse_without_changing_archived_record()
    {
        await using var context = Context();
        var archived = new Employee
        {
            TenantId = _original.TenantId, CountryId = _employees[0].CountryId,
            FirstName = "Archived fixture", AddedById = _employees[0].Id, AddedDateTime = DateTime.UtcNow,
            IsSoftDeleted = true,
            EmployementCode = EmployeeCodePatternFormatter.Format(Proposed(),
                new DateTime(2022, 3, 15), _employees[0].DepartmentId, 1)
        };
        context.Employees.Add(archived);
        await context.SaveChangesAsync();
        try
        {
            var preview = await Repository(context).SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
                Proposed(), false, false, null, CancellationToken.None);
            Assert.That(preview.CanCommit, Is.False);
            Assert.That(preview.Employees.Any(row => row.EmployeeId == archived.Id), Is.False);
            Assert.That(preview.Errors, Has.Some.Contains("Archived"));
        }
        finally
        {
            await context.Employees.Where(item => item.Id == archived.Id).ExecuteDeleteAsync();
        }
    }

    [Test]
    public async Task Preview_is_read_only_and_confirmation_updates_codes_and_pattern_together()
    {
        await using var context = Context();
        var repository = Repository(context);
        var preview = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
            Proposed(), false, false, null, CancellationToken.None);
        Assert.That(preview.CanCommit, Is.True, string.Join(";", preview.Employees.SelectMany(row => row.Errors)));
        Assert.That(await context.EmployeeCodePatterns.Where(pattern => pattern.Id == _original.Id)
            .Select(pattern => pattern.Prefix).SingleAsync(), Is.EqualTo(_original.Prefix));

        var confirmed = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
            Proposed(), false, true, preview.PreviewHash, CancellationToken.None);
        Assert.That(confirmed.Applied, Is.True);
        await using var verified = Context();
        foreach (var row in confirmed.Employees)
        {
            var employee = await verified.Employees.SingleAsync(employee => employee.Id == row.EmployeeId);
            Assert.That(employee.EmployementCode, Is.EqualTo(row.ProposedCode));
            Assert.That(employee.OfficialEmail, Is.EqualTo(_employees.Single(old => old.Id == employee.Id).OfficialEmail));
        }

        Assert.That(await verified.EmployeeCodePatterns.Where(pattern => pattern.Id == _original.Id)
            .Select(pattern => pattern.Prefix).SingleAsync(), Is.EqualTo("TEST"));
    }

    [Test]
    public async Task Stale_preview_is_rejected_without_changing_pattern()
    {
        await using var context = Context();
        var repository = Repository(context);
        var preview = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
            Proposed(), false, false, null, CancellationToken.None);
        await context.EmployeeCodePatterns.Where(pattern => pattern.Id == _original.Id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(pattern => pattern.LastUsedNumber, pattern => pattern.LastUsedNumber + 1));

        Assert.ThrowsAsync<ConflictException>(async () => await repository.SaveWithEmployeeCodesAsync(
            _original.TenantId, _employees[0].Id, Proposed(), false, true, preview.PreviewHash, CancellationToken.None));
        Assert.That(await context.EmployeeCodePatterns.Where(pattern => pattern.Id == _original.Id)
            .Select(pattern => pattern.Prefix).SingleAsync(), Is.EqualTo(_original.Prefix));
    }

    [Test]
    public async Task Create_cannot_replace_an_existing_active_pattern()
    {
        await using var context = Context();
        Assert.ThrowsAsync<ConflictException>(async () => await Repository(context).SaveWithEmployeeCodesAsync(
            _original.TenantId, _employees[0].Id, Proposed(), true, false, null, CancellationToken.None));
    }

    [Test]
    public async Task Concurrent_allocators_observe_committed_sequence_and_original_joining_year()
    {
        await using var first = Context();
        await using var second = Context();
        await using var transaction1 = await first.Database.BeginTransactionAsync();
        var date = new DateTime(2022, 3, 15, 0, 0, 0, DateTimeKind.Utc);
        var code1 = await Repository(first).GenerateEmployeeCodeAsync(_original.TenantId, _employees[0].DepartmentId, date);
        await using var transaction2 = await second.Database.BeginTransactionAsync();
        var pending = Repository(second).GenerateEmployeeCodeAsync(_original.TenantId, _employees[0].DepartmentId, date);
        await first.SaveChangesAsync();
        await transaction1.CommitAsync();
        var code2 = await pending.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.That(code2, Is.Not.EqualTo(code1));
        Assert.That(second.ChangeTracker.Entries<EmployeeCodePattern>().Single().Entity.LastUsedNumber,
            Is.EqualTo(_original.LastUsedNumber + 2));
        if (_original.IncludeYear)
        {
            Assert.That(code2, Does.Contain("2022"));
        }
        await transaction2.RollbackAsync();
    }

    #endregion

    #region Allocation Transaction Boundaries

    [Test]
    public async Task Multiple_allocations_without_save_are_distinct_and_rollback_restores_counter()
    {
        await using var context = Context();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var repository = Repository(context);
        var joiningDate = new DateTime(2022, 3, 15, 0, 0, 0, DateTimeKind.Utc);
        var first = await repository.GenerateEmployeeCodeAsync(_original.TenantId, _employees[0].DepartmentId, joiningDate);
        var second = await repository.GenerateEmployeeCodeAsync(_original.TenantId, _employees[0].DepartmentId, joiningDate);
        Assert.That(second, Is.Not.EqualTo(first));
        await transaction.RollbackAsync();

        await using var verify = Context();
        Assert.That(await verify.EmployeeCodePatterns.Where(item => item.Id == _original.Id)
            .Select(item => item.LastUsedNumber).SingleAsync(), Is.EqualTo(_original.LastUsedNumber));
    }

    [Test]
    public void Allocation_requires_a_transaction()
    {
        using var context = Context();
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await Repository(context).GenerateEmployeeCodeAsync(_original.TenantId));
    }

    [Test]
    public async Task Confirmation_does_not_change_other_tenant_codes_or_patterns()
    {
        await using var context = Context();
        var otherCodes = await context.Employees.AsNoTracking()
            .Where(item => item.TenantId != _original.TenantId)
            .OrderBy(item => item.Id).Select(item => new { item.Id, item.EmployementCode }).ToListAsync();
        Assert.That(otherCodes, Is.Not.Empty, "Fixture must contain a second tenant.");
        var otherPatterns = await context.EmployeeCodePatterns.AsNoTracking()
            .Where(item => item.TenantId != _original.TenantId)
            .OrderBy(item => item.Id).Select(item => new { item.Id, item.Prefix, item.LastUsedNumber }).ToListAsync();
        var repository = Repository(context);
        var preview = await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
            Proposed(), false, false, null, CancellationToken.None);
        await repository.SaveWithEmployeeCodesAsync(_original.TenantId, _employees[0].Id,
            Proposed(), false, true, preview.PreviewHash, CancellationToken.None);
        Assert.That(await context.Employees.AsNoTracking().Where(item => item.TenantId != _original.TenantId)
            .OrderBy(item => item.Id).Select(item => new { item.Id, item.EmployementCode }).ToListAsync(), Is.EqualTo(otherCodes));
        Assert.That(await context.EmployeeCodePatterns.AsNoTracking().Where(item => item.TenantId != _original.TenantId)
            .OrderBy(item => item.Id).Select(item => new { item.Id, item.Prefix, item.LastUsedNumber }).ToListAsync(), Is.EqualTo(otherPatterns));
    }

    #endregion
}
