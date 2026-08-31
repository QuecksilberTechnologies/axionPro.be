// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Persists tenant roles and role query projections.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Constants;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Role;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data; using axionpro.domain.Entity;

namespace axionpro.persistance.Repositories;

/// <summary>
/// Provides persistence operations for Role records.
/// </summary>
public class RoleRepository : IRoleRepository
{

    private readonly WorkforceDbContext _context;

    private readonly IMapper _mapper;
    private readonly ILogger<RoleRepository> _logger;
    private readonly IEncryptionService _encryptionService;

    public RoleRepository(
 WorkforceDbContext context,
 ILogger<RoleRepository> logger,
 IMapper mapper,

 IEncryptionService encryptionService)
    {
        _context = context;
        _logger = logger;
        _mapper = mapper;

        _encryptionService = encryptionService;
    }


    public async Task<Role?> GetTenantAdminRoleAsync(long tenantId)
    {
        try
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.TenantId == tenantId &&
                    r.RoleType == ConstantValues.RoleTypeAdmin &&
                    r.IsActive == true &&
                    r.IsSoftDeleted == false &&
                    r.IsSystemDefault == false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching tenant admin role for TenantId: {TenantId}", tenantId);
            return null;
        }
    }
    public async Task<int> AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(long? tenantId, long employeeId, int role)
    {
        try
        {
            var enabledOperations = await _context.TenantEnabledOperations
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();

            var rolePermissions = enabledOperations.Select(op => new RoleModuleAndPermission
            {
                RoleId = role,
                ModuleId = op.ModuleId,
                OperationId = op.OperationId,
                HasAccess = true,
                IsActive = op.IsEnabled,
                Remark = "System Generate Permission for user",
                IsOperational = true,
                AddedById = tenantId,
                AddedDateTime = DateTime.UtcNow,
                IsSoftDeleted = false
            }).ToList();

            await _context.RoleModuleAndPermissions.AddRangeAsync(rolePermissions);

            _logger.LogInformation(
                "RoleModuleAndPermission entries added to DbContext successfully. Count: {Count}, TenantId: {TenantId}, RoleId: {RoleId}",
                rolePermissions.Count,
                tenantId,
                role);

            return rolePermissions.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync");
            throw;
        }
    }




    /// <summary>
    /// Retrieves all active roles for a given tenant.
    /// </summary>
    /// <param name="role">The request DTO containing tenant and filter information.</param>
    /// <returns>
    /// A list of <see cref="GetRoleResponseDTO"/> representing all active roles for the tenant.
    /// </returns>
    ///





    /// <summary>
    /// Creates a new role for the specified tenant.
    /// </summary>
    /// <param name="dto">The request DTO containing role creation details.</param>
    /// <returns>
    /// A list containing the created role details, mapped to <see cref="GetRoleResponseDTO"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the request DTO is null.</exception>
    /// <exception cref="Exception">Logs and rethrows any exception during creation.</exception>

    /// <summary>
    /// Updates an existing role with values from the DTO.
    /// Only updates fields that are provided; keeps existing values for null/empty fields.
    /// </summary>
    /// <param name="dto">DTO containing the updated role information.</param>
    /// <returns>Returns true if update succeeds, false otherwise.</returns>




    public async Task<Role> AutoCreatedSingleTenantRoleAsync(Role role)
    {
        try
        {
            // await using var context = await _contextFactory.CreateDbContextAsync();

            if (role == null)
            {
                _logger.LogWarning("AutoCreatedForTenantRoleAsync: Received null role object.");
                throw new ArgumentNullException(nameof(role), "Role object cannot be null.");
            }

            // Logging input
            _logger.LogInformation("Creating new Role for TenantId: {TenantId}, RoleName: {RoleName}", role.TenantId, role.RoleName);

            role.AddedDateTime = DateTime.Now;

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role created successfully with Id: {RoleId}", role.Id);

            // Optionally reload from DB if you want latest tracking
            var latestRole = await _context.Roles
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync(r => r.Id == role.Id); // ensure you get the one just created

            return latestRole ?? role;
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database update failed while creating Role.");
            throw; // Let it bubble up or wrap in custom exception
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in AutoCreatedForTenantRoleAsync.");
            throw;
        }
    }
    public async Task<bool> AutoCreatedForTenantRoleAsync(List<Role> roles)
    {
        if (roles == null || !roles.Any())
        {
            _logger.LogWarning("AutoCreatedForTenantRoleAsync called with null or empty role list.");
            return false;
        }

        try
        {
            var validRoles = new List<Role>();

            foreach (var dto in roles)
            {
                if (dto.TenantId <= 0 || string.IsNullOrWhiteSpace(dto.RoleName))
                {
                    _logger.LogWarning(
                        "Skipping invalid role entry. TenantId: {TenantId}, RoleName: {RoleName}",
                        dto.TenantId,
                        dto.RoleName);
                    continue;
                }

                bool exists = await _context.Roles.AnyAsync(r =>
                    r.TenantId == dto.TenantId &&
                    r.RoleName.ToLower() == dto.RoleName.ToLower() &&
                    r.IsSoftDeleted == false);

                if (exists)
                {
                    _logger.LogInformation(
                        "Role '{RoleName}' already exists for TenantId {TenantId}. Skipping insert.",
                        dto.RoleName,
                        dto.TenantId);
                    continue;
                }

                validRoles.Add(new Role
                {
                    TenantId = dto.TenantId,
                    RoleName = dto.RoleName,
                    RoleType = dto.RoleType,
                    IsSystemDefault = false,
                    IsActive = true,
                    IsSoftDeleted = false,
                    Remark = dto.Remark ?? ConstantValues.TenantAllRoleRemark,
                    AddedById = dto.TenantId ?? 0,
                    AddedDateTime = DateTime.UtcNow
                });
            }

            if (!validRoles.Any())
            {
                _logger.LogWarning("No valid roles to insert after validation.");
                return false;
            }

            await _context.Roles.AddRangeAsync(validRoles);

            _logger.LogInformation("Roles added to DbContext successfully. Count: {Count}", validRoles.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred in AutoCreatedForTenantRoleAsync.");
            return false;
        }
    }


    #region Update

    /// <summary>
    /// Gets a mutable role after enforcing tenant ownership.
    /// </summary>
    public Task<Role?> GetByIdForTenantAsync(
        int id,
        long tenantId,
        CancellationToken cancellationToken = default)
    {
        return _context.Roles.FirstOrDefaultAsync(
            role => role.Id == id &&
                    role.TenantId == tenantId &&
                    role.IsSoftDeleted != true,
            cancellationToken);
    }

    /// <summary>
    /// Persists a prepared tenant role entity.
    /// </summary>
    public async Task<bool> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
        {
            _logger.LogWarning("UpdateAsync called with a null Role entity.");
            return false;
        }

        try
        {

            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(
                    role => role.Id == entity.Id &&
                            role.TenantId == entity.TenantId &&
                            role.IsSoftDeleted != true,
                    cancellationToken);

            if (existingRole == null)
            {
                _logger.LogWarning("Role with ID {RoleId} was not found for the tenant.", entity.Id);
                return false;
            }

            bool isModified = false;

            //  RoleName
            if (!string.IsNullOrWhiteSpace(entity.RoleName) &&
                entity.RoleName.Trim() != existingRole.RoleName)
            {
                existingRole.RoleName = entity.RoleName.Trim();
                isModified = true;
            }

            //  RoleType
            if (entity.RoleType != existingRole.RoleType)
            {
                existingRole.RoleType = entity.RoleType;
                isModified = true;
            }

            //  Remark
            if (entity.Remark != existingRole.Remark)
            {
                existingRole.Remark = entity.Remark;
                isModified = true;
            }

            //  IsActive
            if (entity.IsActive != existingRole.IsActive)
            {
                existingRole.IsActive = entity.IsActive;
                isModified = true;
            }


            //  NEW — Update audit fields only when something changes
            if (isModified)
            {
                existingRole.UpdatedById = entity.UpdatedById;
                existingRole.UpdatedDateTime = entity.UpdatedDateTime;

                var affected = await _context.SaveChangesAsync();

                if (affected > 0)
                {
                    _logger.LogInformation(
                        "✅ Role updated successfully. ID: {RoleId}, UpdatedBy: {UpdatedById}",
                        entity.Id, entity.UpdatedById
                    );
                    return true;
                }

                _logger.LogInformation("ℹ️ Update called but no DB changes detected. ID: {RoleId}", entity.Id);
            }
            else
            {
                _logger.LogInformation("🔹 No changes found for Role ID {RoleId}.", entity.Id);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
                _logger.LogError(ex, "Error while updating Role ID {RoleId}", entity.Id);
            return false;
        }
    }

    #endregion


    public async Task<GetSingleRoleResponseDTO?> GetByIdAsync1(GetSingleRoleRequestDTO dto)
    {
        try
        {
            if (dto == null)
            {
                _logger.LogWarning("⚠️ GetByIdAsync1 called with null DTO.");
                return null;
            }


            _logger.LogInformation("🔍 Fetching role details for RoleId: {RoleId}, TenantId:", dto.Id);

            //  Fetch Role by Id with Tenant check and SoftDelete filter
            var role = await _context.Roles
                .Where(r =>
                    r.Id == dto.Id &&
                    r.IsSoftDeleted != true)
                .FirstOrDefaultAsync();

            if (role == null)
            {
                _logger.LogWarning("⚠️ No Role found for RoleId: {RoleId} and TenantId: ", dto.Id);
                return null;
            }

            //  Map entity to response DTO
            var mappedRole = _mapper.Map<GetSingleRoleResponseDTO>(role);

            _logger.LogInformation("✅ Successfully retrieved role details for RoleId: {RoleId}", dto.Id);

            return mappedRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred while fetching role details for RoleId: {RoleId}", dto?.Id);
            return null;
        }
    }




    #region Delete

    /// <summary>
    /// Determines whether the Role is still referenced by a current employee-role assignment
    /// or a current module-operation permission. The IsActive flag is deliberately not part
    /// of this guard: an inactive dependency can be reactivated and must retain its Role.
    /// </summary>
    public Task<bool> HasNonDeletedDependenciesAsync(
        int roleId,
        CancellationToken cancellationToken = default)
    {
        return _context.Roles
            .AsNoTracking()
            .Where(role => role.Id == roleId)
            .AnyAsync(
                role => role.UserRole.Any(userRole => userRole.IsSoftDeleted != true) ||
                        role.RoleModuleAndPermission.Any(permission => !permission.IsSoftDeleted),
                cancellationToken);
    }

    /// <summary>
    /// Soft deletes a role after enforcing tenant ownership.
    /// </summary>
    public async Task<bool> DeleteAsync(
        int id,
        long tenantId,
        long employeeId,
        CancellationToken cancellationToken = default)
    {
        try
        {


            //  Step 3️⃣ - Fetch Role
            var role = await _context.Roles
                .FirstOrDefaultAsync(
                    role => role.Id == id &&
                            role.TenantId == tenantId &&
                            role.IsSoftDeleted != true,
                    cancellationToken);

            if (role == null)
            {
                _logger.LogWarning("Delete failed: Role not found for Id: {Id}", id);
                return false;
            }

            //  Step 4️⃣ - Apply Soft Delete
            role.IsSoftDeleted = true;
            role.IsActive = false;
            role.SoftDeletedById = employeeId;
            role.DeletedDateTime = DateTime.UtcNow;
            role.UpdatedById = employeeId;
            role.UpdatedDateTime = DateTime.UtcNow;

            //  Step 5️⃣ - Update Entity
            _context.Roles.Update(role);

            //  Step 6️⃣ - Commit Changes
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Role deleted successfully. Id: {Id}", id);
                return true;
            }

            _logger.LogWarning("No changes saved for Role Id: {Id}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while deleting Role. Id: {Id}", id);
            return false;
        }
    }

    #endregion

    #region Create

    /// <summary>
    /// Persists a prepared tenant role entity.
    /// </summary>
    public async Task<Role?> CreateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (entity == null)
            {
                _logger.LogWarning("CreateAsync called with a null Role entity.");
                return null;
            }

            //  Duplicate check
            bool isDuplicate = await _context.Roles
                .AnyAsync(
                    role => role.RoleName.ToLower() == entity.RoleName.ToLower() &&
                            role.TenantId == entity.TenantId &&
                            role.IsSoftDeleted != true,
                    cancellationToken);

            if (isDuplicate)
            {
                _logger.LogWarning("Role '{RoleName}' already exists for TenantId: {TenantId}", entity.RoleName, entity.TenantId);
                return null;
            }

            await _context.Roles.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Role '{RoleName}' created for TenantId: {TenantId}.", entity.RoleName, entity.TenantId);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role '{RoleName}' for TenantId: {TenantId}", entity?.RoleName, entity?.TenantId);
            throw;
        }
    }

    #endregion

    public async Task<List<GetRoleResponseDTO>> GetRoleAsync(  long tenantId,   int roleTypeId,   bool isActive)
    {
        try
        {

            var query = _context.Roles
                .AsNoTracking()
                .Where(r =>
                    r.TenantId == tenantId &&
                    r.IsActive == isActive &&
                    (r.IsSoftDeleted !=true));

            // Optional filter
            if (roleTypeId > 0)
                query = query.Where(r => r.RoleType == roleTypeId);

            var result = await query
                .Select(r => new GetRoleResponseDTO
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    RoleType = r.RoleType,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "❌ Error fetching roles for TenantId: {TenantId}",
                tenantId);

            return new List<GetRoleResponseDTO>();
        }
    }

    /// <summary>
    /// Projects paged roles for the supplied trusted tenant identifier.
    /// </summary>
    public async Task<PagedResponseDTO<GetRoleResponseDTO>> GetAsync(long tenantId, GetRoleRequestDTO dto)
    {
        var response = new PagedResponseDTO<GetRoleResponseDTO>();

        try
        {
            if (dto == null)
            {
                _logger.LogWarning("⚠️ GetAsync called with null request DTO.");
                return response;
            }


            int roleType = dto.RoleType;

            var query = _context.Roles
                .Where(r => r.TenantId == tenantId && (r.IsSoftDeleted != true))
                .AsQueryable();

            //  Optional Filters
            if (dto.Id > 0)
                query = query.Where(r => r.Id == dto.Id);



            if (!string.IsNullOrWhiteSpace(dto.RoleName))
                query = query.Where(r => r.RoleName.ToLower().Contains(dto.RoleName.ToLower()));

            if (dto.IsActive == true)
                query = query.Where(r => r.IsActive == dto.IsActive);

            if (roleType > 0)
                query = query.Where(r => r.RoleType == roleType);

            //  Sorting

            query = dto.SortBy?.ToLower() switch
            {
                "rolename" => dto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.RoleName)
                    : query.OrderByDescending(x => x.RoleName),

                "roletype" => dto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.RoleType)
                    : query.OrderByDescending(x => x.RoleType),

                _ => query.OrderByDescending(x => x.Id) // default sort by Id
            };

            //  Pagination
            var totalRecords = await query.CountAsync();
            var roles = await query
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            var mappedList = _mapper.Map<List<GetRoleResponseDTO>>(roles);

            response.Data = mappedList;
            response.TotalCount = totalRecords;
            response.PageNumber = dto.PageNumber;
            response.PageSize = dto.PageSize;
            response.TotalPages = (int)Math.Ceiling((double)totalRecords / dto.PageSize);

            _logger.LogInformation("Retrieved {Count} roles for TenantId: {TenantId}", mappedList.Count, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching roles for TenantId: {TenantId}", tenantId);
            new List<GetRoleResponseDTO>();
        }

        return response;
    }

    #region Option Queries
    /// <summary>
    /// Projects role options for the trusted tenant without constructing an API response.
    /// </summary>
    /// <param name="tenantId">The authenticated tenant identifier.</param>
    /// <param name="dto">The client-editable role option query criteria.</param>
    /// <returns>The matching role option projections.</returns>
    public async Task<List<GetRoleOptionResponseDTO>> GetOptionAsync(long tenantId, GetRoleOptionRequestDTO dto)
    {
        try
        {

            var query = _context.Roles
                .Where(x => x.TenantId == tenantId && x.IsSoftDeleted != true && x.IsActive);

            if (dto.RoleType > 0)
                query = query.Where(x => x.RoleType == dto.RoleType);

            return await query
                .OrderBy(x => x.RoleName)
                .Select(r => new GetRoleOptionResponseDTO
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    RoleType = r.RoleType,
                    IsActive = r.IsActive

                })
                .AsNoTracking()
                .ToListAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching role options for tenant {TenantId}.", tenantId);
            throw;
        }
    }

    #endregion
}
