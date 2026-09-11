using axionpro.application.Common.Helpers;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Exceptions;
using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

public partial class TenantEmployeeCodePatternRepository
{
    #region Pattern Preview And Confirmation

    /// <inheritdoc />
    public async Task<SaveEmployeeCodePatternResponseDTO> SaveWithEmployeeCodesAsync(
        long tenantId,
        long actorId,
        EmployeeCodePattern proposed,
        bool create,
        bool confirm,
        string? previewHash,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        if (confirm)
        {
            await LockTenantCodeAllocationAsync(tenantId, cancellationToken);
        }

        var patternQuery = confirm
            ? _context.EmployeeCodePatterns.FromSqlInterpolated(
                $"SELECT * FROM axionpro.\"EmployeeCodePattern\" WHERE \"TenantId\" = {tenantId} AND \"IsActive\" = TRUE FOR UPDATE")
            : _context.EmployeeCodePatterns;
        var patterns = await patternQuery.AsNoTracking()
            .Where(pattern => pattern.TenantId == tenantId && pattern.IsActive)
            .ToListAsync(cancellationToken);
        if (patterns.Count > 1 || (create && patterns.Count != 0) || (!create && patterns.Count != 1))
        {
            throw new ConflictException(create
                ? "An active pattern already exists. Use update."
                : "Exactly one active pattern is required for update.");
        }

        var current = patterns.SingleOrDefault();
        var employeeQuery = confirm
            ? _context.Employees.FromSqlInterpolated(
                $"SELECT * FROM axionpro.\"Employee\" WHERE \"TenantId\" = {tenantId} ORDER BY \"Id\" FOR UPDATE")
            : _context.Employees;
        var employees = await employeeQuery.AsNoTracking()
            .Where(employee => employee.TenantId == tenantId)
            .OrderBy(employee => employee.Id)
            .ToListAsync(cancellationToken);
        var result = EmployeeCodePatternFormatter.PreviewChanges(tenantId, current, proposed,
            employees.Where(employee => !employee.IsSoftDeleted).ToList());
        var reservedCodes = employees.Where(employee => employee.IsSoftDeleted && employee.EmployementCode is not null)
            .Select(employee => employee.EmployementCode!).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var row in result.Employees.Where(row => row.ProposedCode is not null && reservedCodes.Contains(row.ProposedCode)))
        {
            row.Errors.Add("Proposed code is reserved by an archived employee.");
            result.Errors.Add("Archived employee codes cannot be reused.");
        }
        if (!confirm)
        {
            return result;
        }

        if (!result.CanCommit)
        {
            throw new ValidationErrorException("Employee-code preview contains errors.");
        }

        if (string.IsNullOrEmpty(previewHash) ||
            !string.Equals(previewHash, result.PreviewHash, StringComparison.Ordinal))
        {
            throw new ConflictException("Pattern or employees changed. Review a fresh preview before confirming.");
        }

        var now = DateTime.UtcNow;
        proposed.TenantId = tenantId;
        proposed.LastUsedNumber = result.LastUsedNumber;
        proposed.IsActive = true;
        if (current is null)
        {
            proposed.AddedById = actorId;
            proposed.AddedDateTime = now;
            _context.EmployeeCodePatterns.Add(proposed);
        }
        else
        {
            proposed.Id = current.Id;
            proposed.AddedById = current.AddedById;
            proposed.AddedDateTime = current.AddedDateTime;
            proposed.UpdatedById = actorId;
            proposed.UpdatedDateTime = now;
            _context.EmployeeCodePatterns.Update(proposed);
        }

        foreach (var row in result.Employees.Where(row => row.CurrentCode != row.ProposedCode))
        {
            await _context.Employees.Where(employee => employee.Id == row.EmployeeId && employee.TenantId == tenantId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(employee => employee.EmployementCode, row.ProposedCode)
                    .SetProperty(employee => employee.UpdatedById, actorId)
                    .SetProperty(employee => employee.UpdatedDateTime, now), cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        result.Applied = true;
        return result;
    }

    /// <summary>Serializes this tenant's allocation and pattern changes without blocking other tenants.</summary>
    private Task LockTenantCodeAllocationAsync(long tenantId, CancellationToken cancellationToken)
    {
        var lockKey = $"{axionpro.application.Constants.BulkImportConstants.EmployeeCodeLockPrefix}{tenantId}";
        return _context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock(hashtextextended({lockKey}, 0))", cancellationToken);
    }

    #endregion
}
