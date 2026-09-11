using AutoMapper;
using axionpro.application.Constants;
using GetEmployeeTypeResponseDTO = axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace axionpro.persistance.Repositories;

/// <summary>Owns tenant EmployeeType reads and writes; legacy shared catalogue rows are never tenant options.</summary>
public sealed class EmployeeTypeRepository(WorkforceDbContext context, IMapper mapper) : IEmployeeTypeRepository
{
    #region Tenant reads
    public Task<EmployeeType?> GetEmployeeTypeByIdAsync(long tenantId, int employeeTypeId,
        CancellationToken cancellationToken)
    {
        return context.EmployeeTypes.AsNoTracking().SingleOrDefaultAsync(item =>
            item.Id == employeeTypeId && item.TenantId == tenantId && item.IsSoftDeleted != true,
            cancellationToken);
    }

    /// <inheritdoc />
    public Task<EmployeeType?> GetForUpdateAsync(long tenantId, int employeeTypeId,
        CancellationToken cancellationToken)
    {
        return context.EmployeeTypes.SingleOrDefaultAsync(item =>
            item.Id == employeeTypeId &&
            item.TenantId == tenantId &&
            item.IsSoftDeleted != true,
            cancellationToken);
    }

    public async Task<List<GetEmployeeTypeResponseDTO>> GetAllAsync(long tenantId,
        CancellationToken cancellationToken)
    {
        if (tenantId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var rows = await context.EmployeeTypes.AsNoTracking()
            .Where(item => item.TenantId == tenantId && item.IsSoftDeleted != true)
            .OrderBy(item => item.TypeName).ThenBy(item => item.Id).ToListAsync(cancellationToken);
        return mapper.Map<List<GetEmployeeTypeResponseDTO>>(rows);
    }

    /// <inheritdoc />
    public Task<bool> NameExistsAsync(long tenantId, string typeName,
        int? excludeEmployeeTypeId, CancellationToken cancellationToken)
    {
        var normalizedName = typeName.Trim().ToLower();
        return context.EmployeeTypes.AsNoTracking().AnyAsync(item =>
            item.TenantId == tenantId &&
            item.IsSoftDeleted != true &&
            item.TypeName != null &&
            item.TypeName.Trim().ToLower() == normalizedName &&
            (!excludeEmployeeTypeId.HasValue || item.Id != excludeEmployeeTypeId.Value),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> HasDeletionDependenciesAsync(long tenantId, int employeeTypeId,
        CancellationToken cancellationToken)
    {
        if (await context.Employees.AsNoTracking().AnyAsync(employee =>
                employee.TenantId == tenantId &&
                employee.EmployeeTypeId == employeeTypeId &&
                !employee.IsSoftDeleted,
                cancellationToken))
        {
            return true;
        }

        if (await context.EmployeesChangedTypeHistories.AsNoTracking().AnyAsync(history =>
                (history.OldEmployeeTypeId == employeeTypeId || history.NewEmployeeTypeId == employeeTypeId) &&
                history.Employee.TenantId == tenantId &&
                !history.Employee.IsSoftDeleted,
                cancellationToken))
        {
            return true;
        }

        if (await context.UnStructuredPolicyTypeMappingWithEmployeeTypes.AsNoTracking().AnyAsync(mapping =>
                mapping.TenantId == tenantId &&
                mapping.EmployeeTypeId == employeeTypeId &&
                !mapping.IsSoftDeleted,
                cancellationToken))
        {
            return true;
        }

        if (await context.PolicyLeaveTypeMappings.AsNoTracking().AnyAsync(mapping =>
                mapping.TenantId == tenantId &&
                mapping.EmployeeTypeId == employeeTypeId &&
                mapping.IsSoftDeleted != true,
                cancellationToken))
        {
            return true;
        }

        if (await context.AccoumndationAllowancePolicyByDesignations.AsNoTracking().AnyAsync(mapping =>
                mapping.EmployeeTypeId == employeeTypeId &&
                mapping.Designation.TenantId == tenantId &&
                mapping.IsSoftDelete != true,
                cancellationToken))
        {
            return true;
        }

        if (await context.MealAllowancePolicyByDesignations.AsNoTracking().AnyAsync(mapping =>
                mapping.EmployeeTypeId == employeeTypeId &&
                mapping.Designation.TenantId == tenantId &&
                mapping.IsSoftDelete != true,
                cancellationToken))
        {
            return true;
        }

        if (await context.TravelAllowancePolicyByDesignations.AsNoTracking().AnyAsync(mapping =>
                mapping.EmployeeTypeId == employeeTypeId &&
                mapping.Designation.TenantId == tenantId &&
                mapping.IsSoftDelete != true,
                cancellationToken))
        {
            return true;
        }

        // EmployeeTypeBasicMenu has no soft-delete state. Preserving an existing mapping
        // is required for a future EmployeeType restore, so every row blocks deletion.
        return await context.EmployeeTypeBasicMenus.AsNoTracking().AnyAsync(mapping =>
            mapping.EmployeeTypeId == employeeTypeId,
            cancellationToken);
    }
    #endregion

    #region Creation
    public async Task<GetEmployeeTypeResponseDTO> CreateAsync(long tenantId, long actorId,
        CreateEmployeeTypeDTO request, CancellationToken cancellationToken)
    {
        if (tenantId <= 0 || actorId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var name = request.TypeName?.Trim();
        if (string.IsNullOrWhiteSpace(name) || name.Length > 255 ||
            request.Description?.Length > 255 || request.Remark?.Length > 255)
        {
            throw new ValidationErrorException("TypeName is required; type name, description and remark must not exceed 255 characters.");
        }
        if (await context.EmployeeTypes.AnyAsync(item => item.TenantId == tenantId &&
            item.IsSoftDeleted != true && item.TypeName!.Trim().ToLower() == name.ToLower(), cancellationToken))
        {
            throw new ConflictException("EmployeeType already exists in this tenant.");
        }
        var entity = mapper.Map<EmployeeType>(request);
        entity.TypeName = name;
        entity.TenantId = tenantId;
        entity.AddedById = actorId;
        entity.AddedDateTime = DateTime.UtcNow;
        entity.IsSoftDeleted = false;
        context.EmployeeTypes.Add(entity);
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException postgres &&
            postgres.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            context.Entry(entity).State = EntityState.Detached;
            throw new ConflictException("EmployeeType already exists in this tenant.");
        }
        return mapper.Map<GetEmployeeTypeResponseDTO>(entity);
    }

    /// <inheritdoc />
    public async Task<GetEmployeeTypeResponseDTO> UpdateAsync(EmployeeType entity,
        CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<GetEmployeeTypeResponseDTO>(entity);
    }

    /// <inheritdoc />
    public async Task<bool> SoftDeleteAsync(EmployeeType entity,
        CancellationToken cancellationToken)
    {
        entity.IsSoftDeleted = true;
        entity.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>Preserves existing super-admin onboarding with one tenant-owned Permanent type, not six defaults.</summary>
    public async Task<int> EnsureOnboardingTypeAsync(long tenantId, long actorId, CancellationToken cancellationToken)
    {
        // Caller is the existing tenant-creation transaction. The legacy constant is a template ID only.
        var template = await context.EmployeeTypes.AsNoTracking().SingleAsync(item =>
            item.Id == ConstantValues.ParmanentEmployeeType && item.TenantId == null, cancellationToken);
        var existing = await context.EmployeeTypes.SingleOrDefaultAsync(item => item.TenantId == tenantId &&
            item.TypeName == template.TypeName && item.IsSoftDeleted != true, cancellationToken);
        if (existing is not null)
        {
            return existing.Id;
        }
        var created = await CreateAsync(tenantId, actorId, new CreateEmployeeTypeDTO
        {
            TypeName = template.TypeName!,
            Description = template.Description,
            Remark = template.Remark,
            IsActive = true
        }, cancellationToken);
        return created.Id;
    }
    #endregion
}
