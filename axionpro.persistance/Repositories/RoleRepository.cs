using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Constants;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class RoleRepository : IRoleRepository
    {

        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleRepository> _logger;
        private readonly IEncryptionService _encryptionService;

        public RoleRepository(
     WorkforceDbContext context,
     ILogger<RoleRepository> logger,
     IMapper mapper,
     IDbContextFactory<WorkforceDbContext> contextFactory,
     IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
            _encryptionService = encryptionService;
        }



        public async Task<int> AutoCreateUserRoleAndAutomatedRolePermissionMappingAsync(long? tenantId, long employeeId, int role)
        {
            try
            {

                // 3. Fetch TenantEnabledOperation list for that tenant
                //   await using var context = await _contextFactory.CreateDbContextAsync();

                var enabledOperations = await _context.TenantEnabledOperations
                    .Where(x => x.TenantId == tenantId)
                    .ToListAsync();

                // 4. Convert to RoleModuleAndPermission
                var rolePermissions = enabledOperations.Select(op => new RoleModuleAndPermission
                {
                    RoleId = role,
                    ModuleId = op.ModuleId,
                    OperationId = op.OperationId,
                    HasAccess = true,
                    IsActive = op.IsEnabled,
                    Remark = "System Genrate Prmission for user",
                    IsOperational = true,
                    AddedById = tenantId,
                    AddedDateTime = DateTime.Now,
                    IsSoftDeleted = false,

                }).ToList();

                await _context.RoleModuleAndPermissions.AddRangeAsync(rolePermissions);

                // Final Save
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AutoCreateRoleUserRoleAndAutomatedRolePermissionMappingAsync");
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

        public async Task<int> AutoCreatedForTenantRoleAsync(List<Role> roles)
        {
            if (roles == null || !roles.Any())
            {
                _logger.LogWarning("AutoCreatedForTenantRoleAsync called with null or empty role list.");
                return -1;
            }

            try
            {
                //    await using var context = await _contextFactory.CreateDbContextAsync();
                var validRoles = new List<Role>();

                foreach (var dto in roles)
                {
                    // ✅ Validation: TenantId & RoleName required
                    if (dto.TenantId <= 0 || string.IsNullOrWhiteSpace(dto.RoleName))
                    {
                        _logger.LogWarning("Skipping invalid role entry. TenantId: {TenantId}, RoleName: {RoleName}", dto.TenantId, dto.RoleName);
                        continue;
                    }

                    // ✅ Duplicate check (same tenant + same role name)
                    bool exists = await _context.Roles
                        .AnyAsync(r => r.TenantId == dto.TenantId &&
                                        r.RoleName.ToLower() == dto.RoleName.ToLower() &&
                                       r.IsSoftDeleted == false);

                    if (exists)
                    {
                        _logger.LogInformation("Role '{RoleName}' already exists for TenantId {TenantId}. Skipping insert.", dto.RoleName, dto.TenantId);
                        continue;
                    }

                    // ✅ Prepare entity (no mapping needed if it's already Role type)
                    var roleEntity = new Role
                    {
                        TenantId = dto.TenantId,
                        RoleName = dto.RoleName,
                        RoleType = dto.RoleType, // int type handled properly ✅
                        IsSystemDefault = false,
                        IsActive = dto.IsActive,
                        IsSoftDeleted = false,
                        Remark = dto.Remark ?? ConstantValues.TenantAllRoleRemark,
                        AddedById = dto.TenantId,
                        AddedDateTime = DateTime.UtcNow,
                        UpdatedById = null,
                        UpdatedDateTime = null,
                        SoftDeletedById = null,
                        DeletedDateTime = null
                    };

                    validRoles.Add(roleEntity);
                }

                // ✅ Nothing valid to insert
                if (!validRoles.Any())
                {
                    _logger.LogWarning("No valid roles to insert after validation.");
                    return -1;
                }

                // ✅ Bulk insert all valid roles
                await _context.Roles.AddRangeAsync(validRoles);

                _logger.LogInformation("Before SaveChangesAsync - total roles: {Count}", validRoles.Count);
                try
                {
                    var insertedCount = await _context.SaveChangesAsync();
                    _logger.LogInformation("After SaveChangesAsync - count: {Count}", insertedCount);

                }
                catch (Exception innerEx)
                {
                    _logger.LogError(innerEx, "Error inside SaveChangesAsync()");
                    throw; // rethrow to outer catch
                }


                // ✅ Return TenantAdmin RoleId (latest one)
                var tenantAdminRole = await _context.Roles
                    .Where(r => r.RoleType == ConstantValues.RoleTypeAdmin &&
                                r.IsActive == true &&
                                r.IsSoftDeleted == false)
                    .OrderByDescending(r => r.Id)
                    .FirstOrDefaultAsync();

                if (tenantAdminRole != null)
                {
                    _logger.LogInformation("Tenant-Admin role created successfully with ID: {RoleId}", tenantAdminRole.Id);
                    return tenantAdminRole.Id;
                }

                _logger.LogWarning("Tenant-Admin role not found after insert operation.");
                return -1;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while auto-creating roles.");
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in AutoCreatedForTenantRoleAsync.");
                return -1;
            }
        }



        public async Task<bool> UpdateAsync(UpdateRoleRequestDTO dto, long employeeId)
        {
            if (dto == null)
            {
                _logger.LogWarning("❌ UpdateAsync1 called with null DTO.");
                return false;
            }

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 1️⃣ Validate input
                if (dto == null || dto.Id == "0")
                {
                    _logger.LogWarning("⚠️ DeleteDesignationAsync called with invalid DesignationId.");
                    throw new ArgumentException("Invalid DesignationId for deletion.");
                }
                int Id = SafeParser.TryParseInt(dto.Id);
                // 🔍 Fetch existing role for update
                var existingRole = await context.Roles
                    .FirstOrDefaultAsync(r =>
                        r.Id == Id && (r.IsSoftDeleted != true));

                if (existingRole == null)
                {
                    _logger.LogWarning("⚠️ Role with ID {RoleId} not found ", dto.Id);
                    return false;
                }

                // -------------------------
                // ✅ Field-wise conditional updates
                // -------------------------
                bool isModified = false;

                if (!string.IsNullOrWhiteSpace(dto.RoleName) && dto.RoleName != existingRole.RoleName)
                {
                    existingRole.RoleName = dto.RoleName;
                    isModified = true;
                }

                if (dto.RoleType.HasValue && dto.RoleType.Value != existingRole.RoleType)
                {
                    existingRole.RoleType = dto.RoleType.Value;
                    isModified = true;
                }

                if (dto.Remark != null && dto.Remark != existingRole.Remark)
                {
                    existingRole.Remark = dto.Remark;
                    isModified = true;
                }

                if (dto.IsActive.HasValue && dto.IsActive.Value != existingRole.IsActive)
                {
                    existingRole.IsActive = dto.IsActive.Value;
                    isModified = true;
                }

                // Always update audit fields
                existingRole.UpdatedById = employeeId;
                existingRole.UpdatedDateTime = DateTime.UtcNow;

                // -------------------------
                // 💾 Save changes only if required
                // -------------------------
                if (isModified)
                {
                    var changes = await context.SaveChangesAsync();

                    if (changes > 0)
                    {
                        _logger.LogInformation("✅ Role with ID {RoleId} updated successfully.", dto.Id);
                        return true;
                    }

                    _logger.LogInformation("ℹ️ No database changes detected for Role ID {RoleId}.", dto.Id);
                    return true; // treat as success, even if no rows changed
                }
                else
                {
                    _logger.LogInformation("🔹 No changes detected for Role ID {RoleId}. Returning success (no update required).", dto.Id);
                    return true; // treat as success
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error occurred while updating role with ID {RoleId} for ", dto.Id);
                return false;
            }
        }

    
        public async Task<GetSingleRoleResponseDTO?> GetByIdAsync1(GetSingleRoleRequestDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ GetByIdAsync1 called with null DTO.");
                    return null;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                _logger.LogInformation("🔍 Fetching role details for RoleId: {RoleId}, TenantId:", dto.Id);

                // ✅ Fetch Role by Id with Tenant check and SoftDelete filter
                var role = await context.Roles
                    .Where(r =>
                        r.Id == dto.Id &&
                        r.IsSoftDeleted != true)
                    .FirstOrDefaultAsync();

                if (role == null)
                {
                    _logger.LogWarning("⚠️ No Role found for RoleId: {RoleId} and TenantId: ", dto.Id);
                    return null;
                }

                // ✅ Map entity to response DTO
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



        public async Task<bool> DeleteAsync(DeleteRoleRequestDTO dto, long employeeId)
        {
            try
            {
                // 🧩 1️⃣ Validate input
                if (dto == null || dto.Id == "0")
                {
                    _logger.LogWarning("⚠️ DeleteDesignationAsync called with invalid DesignationId.");
                    throw new ArgumentException("Invalid DesignationId for deletion.");
                }
                int Id = SafeParser.TryParseInt(dto.Id);
                // ✅ Step 1: Fetch existing role which is not soft deleted
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == Id && (r.IsSoftDeleted == null || r.IsSoftDeleted == false));

                if (role == null)
                {
                    _logger.LogWarning("Delete failed: Role not found for Id: {Id}", dto.Id);
                    return false;
                }

                // ✅ Step 2: Soft delete logic
                role.IsSoftDeleted = true;
                role.IsActive = false;
                role.SoftDeletedById = employeeId;
                role.DeletedDateTime = DateTime.Now;
                role.UpdatedById = employeeId;
                role.UpdatedDateTime = DateTime.Now;

                // ✅ Step 3: Mark entity as updated
                _context.Roles.Update(role);

                // ✅ Step 4: Commit changes
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role deleted successfully. Id: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting Role. Id: {Id}", dto.Id);
                return false;
            }
        }


        public async Task<PagedResponseDTO<GetRoleResponseDTO>> CreateAsync(CreateRoleRequestDTO dto, long tenantId, long employeeId)
        {
            var response = new PagedResponseDTO<GetRoleResponseDTO>();

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ CreateAsync called with null DTO.");

                    return response;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Check if role with same name already exists for tenant
                bool isDuplicate = await context.Roles
                    .AnyAsync(r => r.RoleName.ToLower() == dto.RoleName.ToLower()
                                && r.TenantId == tenantId
                                && r.IsSoftDeleted != true);

                if (isDuplicate)
                {
                    _logger.LogWarning("⚠️ Role '{RoleName}' already exists for TenantId: {TenantId}", dto.RoleName, tenantId);

                    return response;
                }

                // ✅ Map DTO to Entity
                var roleEntity = _mapper.Map<Role>(dto);
                roleEntity.TenantId = tenantId;
                roleEntity.AddedById = employeeId;
                roleEntity.AddedDateTime = DateTime.UtcNow;
                roleEntity.IsSoftDeleted = false;

                // ✅ Save to database
                await context.Roles.AddAsync(roleEntity);
                await context.SaveChangesAsync();

                // ✅ Map to Response DTO
                var createdRole = _mapper.Map<GetRoleResponseDTO>(roleEntity);

                response.Items = new List<GetRoleResponseDTO> { createdRole };
                response.TotalCount = 1;
                response.PageNumber = 1;
                response.PageSize = 1;

                _logger.LogInformation("✅ Role '{RoleName}' created successfully for TenantId: {TenantId} by EmployeeId: {EmployeeId}",
                    dto.RoleName, tenantId, employeeId);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error creating role '{RoleName}' for TenantId: {TenantId}", dto?.RoleName, tenantId);

                response.Items = new List<GetRoleResponseDTO>();
                response.TotalCount = 0;
            }

            return response;
        }


        public async Task<PagedResponseDTO<GetRoleResponseDTO>> GetAsync(GetRoleRequestDTO request, long tenantId)
        {
            var response = new PagedResponseDTO<GetRoleResponseDTO>();

            try
            {
                if (request == null)
                {
                    _logger.LogWarning("⚠️ GetAsync called with null request DTO.");
                    return response;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                int roleId = Convert.ToInt32(request.Id);
                var query = context.Roles
                    .Where(r => r.TenantId == tenantId && (r.IsSoftDeleted != true))
                    .AsQueryable();

                // ✅ Optional Filters
                if (roleId > 0)
                    query = query.Where(r => r.Id == roleId);

                if (!string.IsNullOrWhiteSpace(request.RoleName))
                    query = query.Where(r => r.RoleName.ToLower().Contains(request.RoleName.ToLower()));

                if (request.IsActive == true)
                    query = query.Where(r => r.IsActive == request.IsActive);

                // ✅ Sorting
                query = request.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(x => x.RoleName)
                    : query.OrderByDescending(x => x.RoleName);

                // ✅ Pagination
                var totalRecords = await query.CountAsync();
                var roles = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var mappedList = _mapper.Map<List<GetRoleResponseDTO>>(roles);

                response.Items = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;

                _logger.LogInformation("✅ Retrieved {Count} roles for TenantId: {TenantId}", mappedList.Count, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching roles for TenantId: {TenantId}", tenantId);
                new List<GetRoleResponseDTO>();
            }

            return response;
        }

        public async Task<ApiResponse<List<GetRoleOptionResponseDTO?>>> GetOptionAsync(GetRoleOptionRequestDTO dto, long tenantId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // ✅ Base Query
                var query = context.Roles
                    .Where(x => x.TenantId == tenantId && x.IsSoftDeleted != true && x.IsActive == true);

                // ✅ Conditional Filter (agar RoleType = 0 nahi hai)
                if (dto.RoleType != 0)
                {
                    query = query.Where(x => x.RoleType == dto.RoleType);
                }

                // ✅ Projection
                var roles = await query
                    .Select(r => new GetRoleOptionResponseDTO
                    {
                        Id = r.Id,
                        RoleName = r.RoleName,   // ⚡ correct property name as per your entity
                        RoleType = r.RoleType,
                        IsActive = r.IsActive

                    })
                    .ToListAsync();

                // ✅ ApiResponse return
                return new ApiResponse<List<GetRoleOptionResponseDTO?>>
                {
                   
                    Message = roles.Any()
                        ? "Role options fetched successfully."
                        : "No roles found for this tenant.",
                    Data = roles
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching role options for tenantId {TenantId}", tenantId);

                return new ApiResponse<List<GetRoleOptionResponseDTO?>>
                {
                   
                    Message = "An error occurred while fetching role options.",
                    Data = new List<GetRoleOptionResponseDTO?>()
                };
            }
        }

    }
}