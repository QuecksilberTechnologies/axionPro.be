using axionpro.domain.Entity;
using axionpro.infrastructure.Repositories;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>Exercises country and tenant rule resolution against disposable PostgreSQL.</summary>
[TestFixture]
[Category("EmployeeCountryRulesDatabase")]
[NonParallelizable]
public sealed class EmployeeCountryRuleDatabaseTests
{
    #region Fixture

    private WorkforceDbContext CreateContext()
    {
        var connection = Environment.GetEnvironmentVariable("AXIONPRO_BULK_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection))
            Assert.Ignore("Requires disposable axionpro_bulk_test database.");

        Assert.That(new NpgsqlConnectionStringBuilder(connection).Database, Is.EqualTo("axionpro_bulk_test"));
        return new WorkforceDbContext(new DbContextOptionsBuilder<WorkforceDbContext>()
            .UseNpgsql(connection).Options);
    }

    #endregion

    #region Country eligibility

    [Test]
    public async Task Tenant_role_loaded_for_update_is_detached_and_persists_changed_name()
    {
        await using var context = CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var employee = await context.Employees.FirstAsync(x => !x.IsSoftDeleted && x.TenantId != null);
        var role = new Role
        {
            TenantId = employee.TenantId!.Value,
            RoleName = "Role update test " + Guid.NewGuid().ToString("N"),
            RoleType = 2,
            IsActive = true,
            IsSoftDeleted = false,
            AddedById = employee.Id,
            AddedDateTime = DateTime.UtcNow
        };
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var repository = new RoleRepository(context, NullLogger<RoleRepository>.Instance, null!, null!);
        var loaded = await repository.GetByIdForTenantAsync(role.Id, role.TenantId!.Value);
        Assert.That(loaded, Is.Not.Null);
        Assert.That(context.Entry(loaded!).State, Is.EqualTo(EntityState.Detached));

        loaded.RoleName += " changed";
        loaded.UpdatedById = employee.Id;
        loaded.UpdatedDateTime = DateTime.UtcNow;
        Assert.That(await repository.UpdateAsync(loaded), Is.True);

        var savedName = await context.Roles.AsNoTracking()
            .Where(x => x.Id == role.Id)
            .Select(x => x.RoleName)
            .SingleAsync();
        Assert.That(savedName, Does.EndWith(" changed"));
        await transaction.RollbackAsync();
    }

    [Test]
    public async Task Verified_bank_cannot_be_reopened_and_foreign_tenant_bulk_update_is_rejected()
    {
        await using var context = CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var employee = await context.Employees.FirstAsync(x => !x.IsSoftDeleted && x.TenantId != null);
        var bank = new EmployeeBankDetail
        {
            EmployeeId = employee.Id,
            BankName = "Profile verification test",
            BranchName = "Test",
            AccountNumber = "TEST" + Guid.NewGuid().ToString("N")[..12],
            AccountType = "saving",
            IsEditAllowed = true,
            IsInfoVerified = false,
            IsActive = true,
            AddedById = employee.Id,
            AddedDateTime = DateTime.UtcNow
        };
        context.Add(bank);
        await context.SaveChangesAsync();
        var repository = new BaseEmployeeRepository(context, null!,
            NullLogger<BaseEmployeeRepository>.Instance, null!, null!);

        await repository.UpdateVerificationStatusByTabAsync(2, employee.Id, employee.Id, true, CancellationToken.None);
        await repository.UpdateEditableStatusByEntityAsync(2, employee.Id, employee.Id, true, CancellationToken.None);
        var saved = await context.EmployeeBankDetails.AsNoTracking().SingleAsync(x => x.Id == bank.Id);
        Assert.That(saved.IsInfoVerified, Is.True);
        Assert.That(saved.IsEditAllowed, Is.False);

        Assert.That(await repository.UpdateSectionVerifyStatusAsync(2, employee.Id, long.MaxValue,
            false, true, employee.Id, CancellationToken.None), Is.False);
        saved = await context.EmployeeBankDetails.AsNoTracking().SingleAsync(x => x.Id == bank.Id);
        Assert.That(saved.IsInfoVerified, Is.True);
        await transaction.RollbackAsync();
    }

    [Test]
    public async Task Operational_defaults_are_locked_until_enabled_and_can_be_locked_again()
    {
        await using var context = CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var employee = await context.Employees.FirstAsync(x => !x.IsSoftDeleted && x.TenantId != null);
        var tenantId = employee.TenantId!.Value;
        var repository = new BaseEmployeeRepository(context, null!,
            NullLogger<BaseEmployeeRepository>.Instance, null!, null!);
        const string module = "EMP_DEVICES";
        await context.Set<TenantEmployeeSectionDefault>()
            .Where(x => x.TenantId == tenantId && x.ModuleCode == module).ExecuteDeleteAsync();

        Assert.That(await repository.IsOperationalSectionEditAllowedAsync(tenantId, module, CancellationToken.None), Is.False);
        await repository.SetOperationalSectionDefaultAsync(tenantId, module, true, employee.Id, CancellationToken.None);
        Assert.That(await repository.IsOperationalSectionEditAllowedAsync(tenantId, module, CancellationToken.None), Is.True);
        Assert.That(await repository.IsOperationalSectionEditAllowedAsync(long.MaxValue, module, CancellationToken.None), Is.False);
        await repository.SetOperationalSectionDefaultAsync(tenantId, module, false, employee.Id, CancellationToken.None);
        Assert.That(await repository.IsOperationalSectionEditAllowedAsync(tenantId, module, CancellationToken.None), Is.False);
        Assert.That(await context.Set<TenantEmployeeSectionDefault>().CountAsync(x => x.TenantId == tenantId && x.ModuleCode == module), Is.EqualTo(1));
        await transaction.RollbackAsync();
    }

    [Test]
    public async Task Identity_catalogue_rejects_foreign_tenant_and_inactive_rule()
    {
        await using var context = CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var employee = await context.Employees.FirstAsync(x => !x.IsSoftDeleted && x.TenantId != null);
        var rule = await context.CountryIdentityRules.Include(x => x.IdentityCategoryDocument)
            .ThenInclude(x => x.IdentityCategory).FirstAsync(x => x.IsActive &&
                x.IdentityCategoryDocument.IsActive && x.IdentityCategoryDocument.IdentityCategory.IsActive);
        employee.CountryId = rule.CountryId;
        await context.SaveChangesAsync();
        var repository = new EmployeeIdentityRepository(context, null!,
            NullLogger<EmployeeIdentityRepository>.Instance, null!, null!);

        Assert.That(await repository.IsDocumentAllowedAsync(employee.Id, employee.TenantId!.Value,
            rule.IdentityCategoryDocumentId, CancellationToken.None), Is.True);
        Assert.That(await repository.IsDocumentAllowedAsync(employee.Id, long.MaxValue,
            rule.IdentityCategoryDocumentId, CancellationToken.None), Is.False);

        await context.CountryIdentityRules.Where(x => x.CountryId == rule.CountryId &&
                x.IdentityCategoryDocumentId == rule.IdentityCategoryDocumentId)
            .ExecuteUpdateAsync(update => update.SetProperty(x => x.IsActive, false));

        Assert.That(await repository.IsDocumentAllowedAsync(employee.Id, employee.TenantId.Value,
            rule.IdentityCategoryDocumentId, CancellationToken.None), Is.False);
        await transaction.RollbackAsync();
    }

    #endregion

    #region Compliance precedence

    [Test]
    public async Task Tenant_rule_beats_global_rule_and_expired_rule_is_ignored()
    {
        await using var context = CreateContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var tenantId = await context.Employees.Where(x => !x.IsSoftDeleted && x.TenantId != null)
            .Select(x => x.TenantId!.Value).FirstAsync();
        var countryId = await context.Countries.Select(x => x.Id).FirstAsync();
        var type = new ComplianceTypeMaster
        {
            Name = "Employee rule test " + Guid.NewGuid().ToString("N"),
            CountryId = countryId,
            IsActive = true
        };
        context.Add(type);
        await context.SaveChangesAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var global = Rule(type.Id, countryId, null, today.AddDays(-2), null, 999);
        var tenant = Rule(type.Id, countryId, tenantId, today.AddDays(-2), null, 1);
        var expired = Rule(type.Id, countryId, tenantId, today.AddDays(-10), today.AddDays(-1), 1000);
        context.AddRange(global, tenant, expired);
        await context.SaveChangesAsync();
        var repository = new CompilanceRuleRepository(context, NullLogger<CompilanceRuleRepository>.Instance);

        var resolved = await repository.GetRuleAsync(type.Id, countryId, null, tenantId, today);
        Assert.That(resolved!.Id, Is.EqualTo(tenant.Id));
        var fallback = await repository.GetRuleAsync(type.Id, countryId, null, null, today);
        Assert.That(fallback!.Id, Is.EqualTo(global.Id));
        await transaction.RollbackAsync();
    }

    private static ComplianceRule Rule(int type, int country, long? tenant, DateOnly start,
        DateOnly? end, int priority)
    {
        return new ComplianceRule
        {
            ComplianceTypeId = type,
            CountryId = country,
            TenantId = tenant,
            EffectiveFrom = start,
            EffectiveTo = end,
            Priority = priority,
            RuleJson = "{}",
            IsActive = true,
            IsSoftDeleted = false
        };
    }

    #endregion
}
