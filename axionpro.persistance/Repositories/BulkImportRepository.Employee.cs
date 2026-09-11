using System.ComponentModel.DataAnnotations;
using System.Globalization;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace axionpro.persistance.Repositories;

public sealed partial class BulkImportRepository
{
    #region Employee Preview

    public async Task<BulkImportPreviewResponseDTO> PreviewEmployeesAsync(
        BulkImportTableDTO table, string? mappingJson, CommonDecodedResult actor,
        CancellationToken cancellationToken)
    {
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var mapping = BulkImportPreviewService.ResolveColumnMapping(table, mappingJson, BulkImportConstants.EmployeeColumns);
        var preview = new BulkImportPreviewResponseDTO
        {
            Master = BulkImportMaster.Employee,
            SourceColumns = table.Columns.ToList(),
            ColumnMapping = mapping
        };
        var required = typeof(CreateBaseEmployeeRequestDTO).GetProperties()
            .Where(property => Attribute.IsDefined(property, typeof(RequiredAttribute)))
            .Select(property => property.Name)
            .Concat(new[] { nameof(Employee.DateOfOnBoarding), nameof(Employee.HasPermanent), nameof(Employee.IsActive) });
        foreach (var field in required.Where(field => !mapping.ContainsKey(field)))
        {
            preview.Errors.Add($"Map the required column {field}.");
        }
        foreach (var source in table.Columns.Where(column => !mapping.Values.Contains(column)))
        {
            preview.Errors.Add($"Map or remove the unrecognized source column {source}; Employee data is never silently discarded.");
        }
        foreach (var source in table.Rows)
        {
            preview.Rows.Add(new BulkImportPreviewRowDTO
            {
                RowNumber = source.RowNumber,
                Values = mapping.ToDictionary(pair => pair.Key,
                    pair => source.Values[table.Columns.IndexOf(pair.Value)].Trim()),
                Status = BulkImportRowStatus.Ready
            });
        }
        var snapshot = await EmployeeSnapshotAsync(actor.TenantId, preview.Rows, cancellationToken);
        if (snapshot.Pattern is null)
        {
            preview.Errors.Add("Configure exactly one active employee-code pattern and review existing employee changes first.");
        }
        else
        {
            preview.EmployeePatternHash = PatternHash(actor.TenantId, snapshot.Pattern);
        }
        foreach (var row in preview.Rows)
        {
            ValidateEmployeeRow(row, snapshot, table.IsExcel, table.Uses1904DateSystem);
        }
        foreach (var field in new[] { nameof(Employee.OfficialEmail), BulkImportConstants.EmployeeCode })
        {
            foreach (var duplicate in preview.Rows.Where(row => !string.IsNullOrWhiteSpace(row.Values.GetValueOrDefault(field)))
                         .GroupBy(row => row.Values[field], StringComparer.OrdinalIgnoreCase).Where(group => group.Count() > 1))
            {
                foreach (var row in duplicate)
                {
                    row.Errors.Add($"Duplicate {field} in the uploaded file.");
                    row.Status = BulkImportRowStatus.Invalid;
                }
            }
        }
        if (snapshot.Pattern is not null)
        {
            AssignPreviewCodes(preview, snapshot);
        }
        if (snapshot.AvailableSeats < preview.ReadyCount)
        {
            preview.Errors.Add($"Subscription has {Math.Max(0, snapshot.AvailableSeats)} available seats; {preview.ReadyCount} new employees requested. Admin counts toward MaxUsers.");
        }
        return preview;
    }

    private void AssignPreviewCodes(BulkImportPreviewResponseDTO preview, EmployeeSnapshot snapshot)
    {
        var pattern = snapshot.Pattern!;
        var last = pattern.LastUsedNumber;
        foreach (var employee in snapshot.Employees)
        {
            try
            {
                if (EmployeeCodePatternFormatter.TryGetRunningNumber(pattern, employee.DateOfOnBoarding ?? default,
                    employee.DepartmentId, employee.EmployementCode, out var number))
                {
                    last = Math.Max(last, number);
                }
            }
            catch (ArgumentException)
            {
                // Old records are never repaired by import. The persisted counter still protects issued numbers.
            }
        }
        foreach (var row in preview.Rows.Where(row => row.Status == BulkImportRowStatus.Ready))
        {
            var employee = EmployeeImportRowMapper.Map(row).Employee;
            if (EmployeeCodePatternFormatter.TryGetRunningNumber(pattern, employee.DateOfOnBoarding!.Value,
                employee.DepartmentId, row.Values.GetValueOrDefault(BulkImportConstants.EmployeeCode), out var number))
            {
                row.ProposedEmployeeCode = row.Values[BulkImportConstants.EmployeeCode];
                last = Math.Max(last, number);
            }
        }
        foreach (var row in preview.Rows.Where(row => row.Status == BulkImportRowStatus.Ready))
        {
            var employee = EmployeeImportRowMapper.Map(row).Employee;
            try
            {
                row.ProposedEmployeeCode ??= EmployeeCodePatternFormatter.Format(pattern,
                    employee.DateOfOnBoarding!.Value, employee.DepartmentId, checked(++last));
                if (row.ProposedEmployeeCode.Length > 50 || snapshot.Employees.Any(item =>
                    string.Equals(item.EmployementCode, row.ProposedEmployeeCode, StringComparison.OrdinalIgnoreCase)))
                {
                    row.Errors.Add("Proposed employee code conflicts with an existing record or exceeds its field length.");
                    row.Status = BulkImportRowStatus.Invalid;
                }
            }
            catch (OverflowException)
            {
                row.Errors.Add("Employee code sequence is exhausted.");
                row.Status = BulkImportRowStatus.Invalid;
            }
        }
        preview.ReservedEmployeeSequence = last;
    }

    #endregion

    #region Tenant References And Capacity

    private sealed record EmployeeSnapshot(
        long TenantId, EmployeeCodePattern? Pattern, List<Employee> Employees, List<LoginCredential> Logins,
        List<Department> Departments, List<Designation> Designations, List<EmployeeType> Types,
        List<Role> Roles, List<Country> Countries, List<Gender> Genders, List<State> States,
        List<District> Districts, int AvailableSeats);

    private async Task<EmployeeSnapshot> EmployeeSnapshotAsync(
        long tenantId, IReadOnlyList<BulkImportPreviewRowDTO> rows, CancellationToken token)
    {
        var patterns = await context.EmployeeCodePatterns.AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.IsActive).ToListAsync(token);
        var employees = await context.Employees.AsNoTracking().Where(item => item.TenantId == tenantId).ToListAsync(token);
        var emails = rows.Select(row => row.Values.GetValueOrDefault(nameof(Employee.OfficialEmail), string.Empty).ToUpperInvariant()).Distinct().ToList();
        var logins = await context.LoginCredentials.AsNoTracking().Where(item => emails.Contains(item.LoginId.ToUpper()))
            .Select(item => new LoginCredential { EmployeeId = item.EmployeeId, TenantId = item.TenantId, LoginId = item.LoginId, IsSoftDeleted = item.IsSoftDeleted }).ToListAsync(token);
        var departments = await context.Departments.AsNoTracking().Where(item => item.TenantId == tenantId && item.IsActive == true && item.IsSoftDeleted != true).ToListAsync(token);
        var designations = await context.Designations.AsNoTracking().Where(item => item.TenantId == tenantId && item.IsActive == true && item.IsSoftDeleted != true).ToListAsync(token);
        var types = await context.EmployeeTypes.AsNoTracking().Where(item => item.TenantId == tenantId && item.IsActive == true && item.IsSoftDeleted != true).ToListAsync(token);
        var roles = await context.Roles.AsNoTracking().Where(item => item.TenantId == tenantId && item.IsActive && item.IsSoftDeleted != true).ToListAsync(token);
        var countries = await context.Countries.AsNoTracking().Where(item => item.IsActive == true).ToListAsync(token);
        var genders = await context.Genders.AsNoTracking().ToListAsync(token);
        var states = await context.States.AsNoTracking().Where(item => item.IsActive == true).ToListAsync(token);
        var districts = await context.Districts.AsNoTracking().Where(item => item.IsActive).ToListAsync(token);
        var available = await EmployeeCapacityGuard.AvailableSeatsAsync(context, tenantId, token);
        return new EmployeeSnapshot(tenantId, patterns.Count == 1 ? patterns[0] : null, employees, logins,
            departments, designations, types, roles, countries, genders, states, districts, available);
    }

    private void ValidateEmployeeRow(BulkImportPreviewRowDTO row, EmployeeSnapshot snapshot, bool excel = false, bool date1904 = false)
    {
        var (dto, contactDto) = EmployeeImportRowMapper.Map(row, excel, date1904);
        if (row.Values.GetValueOrDefault(BulkImportConstants.EmployeeCode, string.Empty).Length > 50)
        {
            row.Errors.Add("EmployeeCode exceeds the 50-character field limit.");
        }
        if (!snapshot.Departments.Any(item => item.Id == dto.DepartmentId))
            row.Errors.Add("DepartmentId must be active in this tenant.");
        if (!snapshot.Designations.Any(item => item.Id == dto.DesignationId && item.DepartmentId == dto.DepartmentId))
            row.Errors.Add("DesignationId must be active in this tenant and belong to DepartmentId.");
        if (!snapshot.Types.Any(item => item.Id == dto.EmployeeTypeId))
            row.Errors.Add("EmployeeTypeId must be active in this tenant.");
        if (dto.RoleId == 0)
        {
            var defaultRole = snapshot.Roles.Where(item => item.RoleType == ConstantValues.RoleTypeEmployee).OrderBy(item => item.Id).FirstOrDefault();
            dto.RoleId = defaultRole?.Id ?? 0;
            if (dto.RoleId > 0)
                row.Values[nameof(dto.RoleId)] = dto.RoleId.ToString(CultureInfo.InvariantCulture);
        }
        if (!snapshot.Roles.Any(item => item.Id == dto.RoleId))
            row.Errors.Add("RoleId must be active in this tenant, or a default Employee role must exist.");
        if (!snapshot.Countries.Any(item => item.Id == dto.CountryId))
            row.Errors.Add("CountryId must be active.");
        if (!snapshot.Genders.Any(item => item.Id == dto.GenderId))
            row.Errors.Add("GenderId must exist.");
        var employee = mapper.Map<Employee>(dto);
        employee.MobileNumber = row.Values.GetValueOrDefault(nameof(Employee.MobileNumber));
        ValidateLengths(employee, row);
        if (contactDto is not null)
        {
            if (contactDto.CountryId.HasValue && !snapshot.Countries.Any(item => item.Id == contactDto.CountryId))
                row.Errors.Add("ContactCountryId must be active.");
            if (contactDto.StateId.HasValue && !snapshot.States.Any(item => item.Id == contactDto.StateId && item.CountryId == contactDto.CountryId))
                row.Errors.Add("StateId must be active and belong to ContactCountryId.");
            if (contactDto.DistrictId.HasValue && !snapshot.Districts.Any(item => item.Id == contactDto.DistrictId && item.StateId == contactDto.StateId))
                row.Errors.Add("DistrictId must be active and belong to StateId.");
            ValidateLengths(mapper.Map<EmployeeContact>(contactDto), row);
        }
        var existing = snapshot.Employees.Where(item => string.Equals(item.OfficialEmail, dto.OfficialEmail, StringComparison.OrdinalIgnoreCase)).ToList();
        if (existing.Count == 1 && !existing[0].IsSoftDeleted)
        {
            var suppliedCode = row.Values.GetValueOrDefault(BulkImportConstants.EmployeeCode);
            if (!string.IsNullOrEmpty(suppliedCode) && !string.Equals(suppliedCode, existing[0].EmployementCode, StringComparison.Ordinal))
                row.Errors.Add("Email already belongs to an employee with a different code. Existing records are not overwritten.");
            row.ProposedEmployeeCode = existing[0].EmployementCode;
            row.Status = BulkImportRowStatus.Existing;
        }
        else if (existing.Count > 0 || snapshot.Logins.Any(item => string.Equals(item.LoginId, dto.OfficialEmail, StringComparison.OrdinalIgnoreCase)))
        {
            row.Errors.Add("OfficialEmail is unavailable. Import will not replace an existing account.");
        }
        if (row.Errors.Count > 0)
            row.Status = BulkImportRowStatus.Invalid;
    }

    private void ValidateLengths<TEntity>(TEntity entity, BulkImportPreviewRowDTO row) where TEntity : class
    {
        foreach (var property in context.Model.FindEntityType(typeof(TEntity))!.GetProperties())
        {
            if (property.GetMaxLength() is int max && property.PropertyInfo?.GetValue(entity) is string value && value.Length > max)
                row.Errors.Add($"{property.Name} exceeds its {max}-character field limit.");
        }
    }

    private static string PatternHash(long tenantId, EmployeeCodePattern pattern)
    {
        return EmployeeCodePatternFormatter.PreviewChanges(tenantId, pattern, pattern, Array.Empty<Employee>()).PreviewHash;
    }

    #endregion

    #region Confirmation And Worker

    private Task LockEmployeeCodesAsync(long tenantId, CancellationToken token)
    {
        return context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({$"{BulkImportConstants.EmployeeCodeLockPrefix}{tenantId}"}, 0))", token);
    }

    private async Task ConfirmEmployeesAsync(BulkImportJob job, BulkImportPreviewResponseDTO preview, CancellationToken token)
    {
        await LockEmployeeCodesAsync(job.TenantId, token);
        var fresh = await EmployeeSnapshotAsync(job.TenantId, preview.Rows, token);
        if (fresh.Pattern is null || PatternHash(job.TenantId, fresh.Pattern) != preview.EmployeePatternHash)
            throw new ConflictException("Employee code pattern or sequence changed. Review a fresh preview.");
        if (fresh.AvailableSeats < preview.ReadyCount)
            throw new ConflictException("Subscription capacity changed. Review a fresh preview.");
        foreach (var saved in preview.Rows)
        {
            var row = new BulkImportPreviewRowDTO { Values = new(saved.Values), Status = BulkImportRowStatus.Ready };
            ValidateEmployeeRow(row, fresh);
            if (row.Status != saved.Status || row.Errors.Count > 0 ||
                (saved.Status == BulkImportRowStatus.Ready && fresh.Employees.Any(item => item.EmployementCode == saved.ProposedEmployeeCode)))
                throw new ConflictException("Employee data or references changed. Review a fresh preview.");
        }
        // Reserve the approved range before queueing so a concurrent single-create cannot consume it.
        await context.EmployeeCodePatterns.Where(item => item.Id == fresh.Pattern.Id).ExecuteUpdateAsync(setters => setters
            .SetProperty(item => item.LastUsedNumber, preview.ReservedEmployeeSequence ?? fresh.Pattern.LastUsedNumber)
            .SetProperty(item => item.UpdatedDateTime, DateTime.UtcNow), token);
    }

    private async Task ProcessEmployeeBatchAsync(BulkImportJob job, BulkImportPreviewResponseDTO preview, CancellationToken token)
    {
        await LockEmployeeCodesAsync(job.TenantId, token);
        foreach (var table in new[] { "Department", "Designation", "EmployeeType", "Role" })
        {
            // Table names are the fixed existing master tables, never request data.
            await context.Database.ExecuteSqlRawAsync(
                $"SELECT \"Id\" FROM axionpro.\"{table}\" WHERE \"TenantId\" = {{0}} FOR SHARE",
                new object[] { job.TenantId }, token);
        }
        var end = Math.Min(preview.Rows.Count, job.NextRow + Math.Clamp(options.Value.BatchSize, 1, 200));
        // A deterministic order prevents cross-tenant batches with reversed email lists from deadlocking.
        foreach (var email in preview.Rows.Skip(job.NextRow).Take(end - job.NextRow)
                     .Select(row => row.Values.GetValueOrDefault(nameof(Employee.OfficialEmail), string.Empty).Trim().ToUpperInvariant())
                     .Distinct().OrderBy(value => value, StringComparer.Ordinal))
        {
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"SELECT pg_advisory_xact_lock(hashtextextended({$"{BulkImportConstants.EmployeeLoginLockPrefix}{email}"}, 0))", token);
        }
        for (var index = job.NextRow; index < end; index++)
        {
            var row = preview.Rows[index];
            if (row.Status is BulkImportRowStatus.Created || row.Processed && row.Status == BulkImportRowStatus.Existing)
            {
                job.NextRow = index + 1;
                continue;
            }
            var approvedCode = row.ProposedEmployeeCode;
            var fresh = await EmployeeSnapshotAsync(job.TenantId, new[] { row }, token);
            row.Errors.Clear();
            row.Status = BulkImportRowStatus.Ready;
            ValidateEmployeeRow(row, fresh);
            if (row.Status == BulkImportRowStatus.Ready)
            {
                var dto = EmployeeImportRowMapper.Map(row).Employee;
                if (fresh.Pattern is null || !EmployeeCodePatternFormatter.TryGetRunningNumber(fresh.Pattern,
                    dto.DateOfOnBoarding!.Value, dto.DepartmentId, approvedCode, out _))
                    row.Errors.Add("Pattern changed after approval. Create a fresh preview for these rows.");
                if (fresh.AvailableSeats <= 0)
                    row.Errors.Add("Subscription MaxUsers capacity is exhausted, including the initial Admin seat.");
                if (fresh.Employees.Any(item => string.Equals(item.EmployementCode, approvedCode, StringComparison.OrdinalIgnoreCase)))
                    row.Errors.Add("Approved employee code was taken after preview. Create a fresh preview.");
                if (row.Errors.Count == 0)
                {
                    await context.Database.CurrentTransaction!.CreateSavepointAsync("employee_row", token);
                    try
                    {
                        await InsertEmployeeAsync(job, row, token);
                        row.Status = BulkImportRowStatus.Created;
                    }
                    catch (Exception error) when (error is ValidationErrorException || error is DbUpdateException
                        { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ForeignKeyViolation or PostgresErrorCodes.CheckViolation } })
                    {
                        await context.Database.CurrentTransaction!.RollbackToSavepointAsync("employee_row", token);
                        context.ChangeTracker.Clear();
                        row.Errors.Add("Data changed during account creation. Review the row before retrying.");
                    }
                    await context.Database.CurrentTransaction!.ReleaseSavepointAsync("employee_row", token);
                }
            }
            if (row.Errors.Count > 0 || row.Status == BulkImportRowStatus.Invalid)
                row.Status = BulkImportRowStatus.Failed;
            row.Processed = true;
            job.NextRow = index + 1;
        }
        job.Status = job.NextRow >= preview.Rows.Count
            ? (int)(preview.Rows.Any(row => row.Status == BulkImportRowStatus.Failed) ? BulkImportJobStatus.CompletedWithErrors : BulkImportJobStatus.Completed)
            : (int)BulkImportJobStatus.Running;
        await SaveProgress(job, preview, token);
    }

    private async Task InsertEmployeeAsync(BulkImportJob job, BulkImportPreviewRowDTO row, CancellationToken token)
    {
        var (dto, contactDto) = EmployeeImportRowMapper.Map(row);
        var employee = mapper.Map<Employee>(dto);
        employee.TenantId = job.TenantId;
        employee.EmployementCode = row.ProposedEmployeeCode;
        employee.MobileNumber = row.Values.GetValueOrDefault(nameof(Employee.MobileNumber));
        employee.AddedById = job.ActorId;
        employee.AddedDateTime = DateTime.UtcNow;
        employee.IsEditAllowed = true;
        employee.IsInfoVerified = false;
        employee.IsSoftDeleted = false;
        if (contactDto is not null)
        {
            var contact = mapper.Map<EmployeeContact>(contactDto);
            contact.Employee = employee;
            contact.AddedById = job.ActorId;
            contact.AddedDateTime = DateTime.UtcNow;
            contact.IsActive = true;
            contact.IsSoftDeleted = false;
            contact.IsEditAllowed = true;
            contact.IsInfoVerified = false;
            employee.EmployeeContact.Add(contact);
        }
        var login = new LoginCredential
        {
            Employee = employee, TenantId = job.TenantId, LoginId = dto.OfficialEmail,
            HasFirstLogin = true, IsPasswordChangeRequired = true, IsActive = dto.IsActive,
            AddedById = job.ActorId, AddedDateTime = DateTime.UtcNow, IsSoftDeleted = false
        };
        var role = new UserRole
        {
            Employee = employee, RoleId = dto.RoleId, IsActive = true, IsPrimaryRole = true,
            RoleStartDate = employee.DateOfOnBoarding, AddedById = job.ActorId, AddedDateTime = DateTime.UtcNow
        };
        await (employeeRepository ?? throw new InvalidOperationException("Employee repository is not configured."))
            .CreateEmployeeAsync(employee, login, role);
        row.ImportedEmployeeId = employee.Id;
        row.InvitationStatus = BulkImportInvitationStatus.Pending;
        token.ThrowIfCancellationRequested();
        // Welcome email is deliberately absent. Invitation dispatch is a separate explicit action.
    }

    #endregion
}
