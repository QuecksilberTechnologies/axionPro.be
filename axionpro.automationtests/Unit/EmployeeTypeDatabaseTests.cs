using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Exceptions;
using axionpro.application.Mappings;
using axionpro.persistance.Data.Context;
using axionpro.persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

[TestFixture]
[Category("BulkImportDatabase")]
[NonParallelizable]
public sealed class EmployeeTypeDatabaseTests
{
    private WorkforceDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;
    private EmployeeTypeRepository _repository = null!;
    private long[] _tenants = null!;
    private string _name = null!;

    [SetUp]
    public async Task Setup()
    {
        var connection = Environment.GetEnvironmentVariable("AXIONPRO_BULK_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connection))
        {
            Assert.Ignore("Requires the isolated axionpro_bulk_test database with tenant EmployeeType migration.");
        }
        Assert.That(new NpgsqlConnectionStringBuilder(connection).Database, Is.EqualTo("axionpro_bulk_test"));
        _context = new WorkforceDbContext(new DbContextOptionsBuilder<WorkforceDbContext>().UseNpgsql(connection).Options);
        _transaction = await _context.Database.BeginTransactionAsync();
        _repository = new EmployeeTypeRepository(_context,
            new MapperConfiguration(config => config.AddProfile<MappingProfile>()).CreateMapper());
        _tenants = await _context.Tenants.OrderBy(item => item.Id).Select(item => item.Id).Take(2).ToArrayAsync();
        Assert.That(_tenants.Length, Is.EqualTo(2));
        _name = "BulkTypeTest-" + Guid.NewGuid().ToString("N");
    }

    [TearDown]
    public async Task Cleanup()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }
    }

    [Test]
    public async Task Same_name_is_allowed_in_two_tenants_but_case_trim_duplicate_is_rejected()
    {
        var first = await _repository.CreateAsync(_tenants[0], 1, new CreateEmployeeTypeDTO { TypeName = _name }, CancellationToken.None);
        var second = await _repository.CreateAsync(_tenants[1], 1, new CreateEmployeeTypeDTO { TypeName = _name }, CancellationToken.None);
        Assert.That(first.Id, Is.Not.EqualTo(second.Id));
        Assert.ThrowsAsync<ConflictException>(async () => await _repository.CreateAsync(_tenants[0], 1,
            new CreateEmployeeTypeDTO { TypeName = " " + _name.ToUpperInvariant() + " " }, CancellationToken.None));
        Assert.That(await _repository.GetEmployeeTypeByIdAsync(_tenants[1], first.Id, CancellationToken.None), Is.Null);
    }

    [Test]
    public async Task Tenant_reads_hide_legacy_and_foreign_catalogues()
    {
        var own = await _repository.CreateAsync(_tenants[0], 1, new CreateEmployeeTypeDTO { TypeName = _name }, CancellationToken.None);
        var foreign = await _repository.CreateAsync(_tenants[1], 1, new CreateEmployeeTypeDTO { TypeName = _name + "-foreign" }, CancellationToken.None);
        var rows = await _repository.GetAllAsync(_tenants[0], CancellationToken.None);
        var legacy = await _context.EmployeeTypes.Where(item => item.TenantId == null).Select(item => item.Id).ToListAsync();
        Assert.That(rows.Any(item => item.Id == own.Id), Is.True);
        Assert.That(rows.Any(item => item.Id == foreign.Id || legacy.Contains(item.Id)), Is.False);
    }

    [Test]
    public async Task Onboarding_resolves_one_tenant_permanent_type_idempotently()
    {
        var before = await _context.EmployeeTypes.CountAsync(item => item.TenantId == _tenants[0]);
        var first = await _repository.EnsureOnboardingTypeAsync(_tenants[0], 1, CancellationToken.None);
        var second = await _repository.EnsureOnboardingTypeAsync(_tenants[0], 1, CancellationToken.None);
        Assert.That(second, Is.EqualTo(first));
        Assert.That(await _context.EmployeeTypes.Where(item => item.Id == first).Select(item => item.TenantId).SingleAsync(),
            Is.EqualTo(_tenants[0]));
        var after = await _context.EmployeeTypes.CountAsync(item => item.TenantId == _tenants[0]);
        Assert.That(after - before, Is.InRange(0, 1), "Must not seed six defaults.");
    }

    [Test]
    public async Task Cross_tenant_employee_assignment_is_rejected_by_database()
    {
        var employee = await _context.Employees.AsNoTracking().FirstAsync(item => item.TenantId != null);
        var otherTenant = _tenants.First(item => item != employee.TenantId);
        var foreign = await _repository.CreateAsync(otherTenant, 1, new CreateEmployeeTypeDTO { TypeName = _name }, CancellationToken.None);
        var error = Assert.ThrowsAsync<PostgresException>(async () => await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE axionpro.\"Employee\" SET \"EmployeeTypeId\"={foreign.Id} WHERE \"Id\"={employee.Id}"));
        Assert.That(error!.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
    }

    [Test]
    public async Task Migrated_employee_references_keep_tenant_ownership_and_legacy_templates_remain()
    {
        var invalid = await (from employee in _context.Employees
            join type in _context.EmployeeTypes on employee.EmployeeTypeId equals type.Id
            where employee.TenantId != null && employee.TenantId != type.TenantId
            select employee.Id).CountAsync();
        Assert.That(invalid, Is.Zero);
        Assert.That(await _context.EmployeeTypes.AnyAsync(item => item.Id == 1 && item.TenantId == null), Is.True);
    }

    [Test]
    public async Task Existing_type_cannot_be_moved_to_another_tenant()
    {
        var own = await _repository.CreateAsync(_tenants[0], 1,
            new CreateEmployeeTypeDTO { TypeName = _name }, CancellationToken.None);
        var error = Assert.ThrowsAsync<PostgresException>(async () => await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE axionpro.\"EmployeeType\" SET \"TenantId\"={_tenants[1]} WHERE \"Id\"={own.Id}"));
        Assert.That(error!.SqlState, Is.EqualTo(PostgresErrorCodes.CheckViolation));
    }

    [TestCase(255, true)]
    [TestCase(256, false)]
    public void Preview_respects_employee_type_database_text_lengths(int length, bool valid)
    {
        var preview = BulkImportPreviewService.Build(BulkImportMaster.EmployeeType, null,
            new BulkImportTableDTO
            {
                Columns = new() { "TypeName", "Description", "Remark" },
                Rows = new() { new() { RowNumber = 2, Values = new() { _name, new string('a', length), new string('b', length) } } }
            }, Array.Empty<axionpro.application.DTOs.Department.GetDepartmentResponseDTO>(),
            Array.Empty<axionpro.application.DTOs.Designation.GetDesignationResponseDTO>(),
            Array.Empty<axionpro.application.DTOs.Role.GetRoleResponseDTO>());
        Assert.That(preview.IsValid, Is.EqualTo(valid));
    }
}
