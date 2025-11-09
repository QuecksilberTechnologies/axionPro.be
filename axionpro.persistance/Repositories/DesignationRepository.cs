using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Designation.Custom;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Role;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
  
    public   class DesignationRepository : IDesignationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<DesignationRepository> _logger;

        public DesignationRepository(
            WorkforceDbContext context,
            ILogger<DesignationRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            this._logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }
        

        public async Task<int> AutoCreateDesignationAsync(List<Designation> designations,int departmentId)
        {
            try
            {
                if (designations == null || !designations.Any())
                {
                    _logger.LogWarning("Designation seed list is null or empty. Seeding aborted.");
                    return 0;
                }

                long tenantId = designations.First().TenantId;

                _logger.LogInformation("Attempting to create {Count} Designation(s) for TenantId: {TenantId}", designations.Count, tenantId);

                await _context.Designations.AddRangeAsync(designations);
                var result = await _context.SaveChangesAsync();

                if (result == designations.Count)
                {
                    _logger.LogInformation("Successfully created {Count} designations for TenantId: {TenantId}", result, tenantId);

                    // Admin Designation ID return karo
                    var adminDesignation = designations.FirstOrDefault(d => d.Department.TenantId == tenantId && d.Department.IsExecutiveOffice == true);
                    if (adminDesignation != null)
                    {
                        return adminDesignation.Id; // Id will be populated after SaveChangesAsync
                    }

                    _logger.LogWarning("Admin designation not found in the inserted list.");
                    return 0;
                }
                else
                {
                    _logger.LogWarning("Mismatch in inserted designation count. Expected: {Expected}, Inserted: {Inserted}", designations.Count, result);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating designations.");
                return 0;
            }
        }

        
        public async Task<bool> CheckDuplicateValueAsync(long tenantId, string value)
        {
            try
            {
               

                // Check if duplicate value exists in Designation table (case-insensitive)
                bool exists = await _context.Designations
              .AnyAsync(d => d.TenantId == tenantId
                          && d.IsSoftDeleted != true
                          && d.DesignationName.ToLower() == value.Trim().ToLower());


                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking duplicate Designation value '{Value}' .", value);
                throw;
            }
        }

        public async Task<PagedResponseDTO<GetDesignationResponseDTO>> CreateAsync(CreateDesignationRequestDTO dto, long tenantId, long employeeId)
        {
            var result = new PagedResponseDTO<GetDesignationResponseDTO>();

            try
            {
                // 🧩 1️⃣ Validate input
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ CreateAsync called with null designation DTO.");
                    throw new ArgumentNullException(nameof(dto), "Designation object cannot be null.");
                }

                // 🧩 2️⃣ Parse and validate DepartmentId
                int departmentId = 0;
                if (!string.IsNullOrWhiteSpace(dto.DepartmentId))
                    departmentId = SafeParser.TryParseInt(dto.DepartmentId);

                if (departmentId <= 0)
                    throw new ArgumentException("❌ Invalid DepartmentId provided.");

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Check if department exists and is active
                var departmentExists = await context.Departments
                    .AnyAsync(d => d.Id == departmentId &&
                                   d.TenantId == tenantId &&
                                   d.IsActive == true &&
                                   d.IsSoftDeleted != true);

                if (!departmentExists)
                {
                    _logger.LogWarning("⚠️ Department with Id {DepartmentId} not found or inactive for TenantId {TenantId}.", departmentId, tenantId);
                    throw new InvalidOperationException($"Department with Id {departmentId} does not exist or is inactive.");
                }

                // 🧩 3️⃣ Check duplicate designation name under same tenant
                bool exists = await context.Designations
                    .AnyAsync(d =>
                        d.TenantId == tenantId &&
                        d.DesignationName.ToLower() == dto.DesignationName.ToLower() &&
                        d.IsSoftDeleted != true);

                if (exists)
                {
                    _logger.LogWarning("⚠️ Designation '{Name}' already exists for TenantId {TenantId}.", dto.DesignationName, tenantId);

                    result.Items = new List<GetDesignationResponseDTO>();
                    result.TotalCount = 0;
                    result.PageNumber = 1;
                    result.PageSize = 10;
                    return result;
                }

                // 🧩 4️⃣ Map DTO → Entity
                var entity = _mapper.Map<Designation>(dto);
                entity.TenantId = tenantId;
                entity.DepartmentId = departmentId;
                entity.AddedById = employeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;

                // 🧩 5️⃣ Insert & Save
                await context.Designations.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Designation '{Name}' created successfully with Id: {Id}", dto.DesignationName, entity.Id);

                // 🧩 6️⃣ Fetch latest 10 designations (with DepartmentName join)
                var latestDesignations = await (
                    from des in context.Designations.AsNoTracking()
                    join dep in context.Departments.AsNoTracking()
                        on des.DepartmentId equals dep.Id into deptGroup
                    from dep in deptGroup.DefaultIfEmpty()
                    where des.TenantId == tenantId
                          && (des.IsSoftDeleted == null || des.IsSoftDeleted == false)
                          && (dep == null || (dep.IsSoftDeleted != true && dep.IsActive == true))
                    orderby des.Id descending
                    select new GetDesignationResponseDTO
                    {
                        Id = des.Id.ToString(),
                        DepartmentId = des.DepartmentId.ToString(),
                        DesignationName = des.DesignationName,
                        DepartmentName = dep != null ? dep.DepartmentName : string.Empty,
                        Description = des.Description,
                        IsActive = des.IsActive
                    })
                    .Take(10)
                    .ToListAsync();

                // 🧩 7️⃣ Prepare paged response
                result.Items = latestDesignations;
                result.TotalCount = await context.Designations.CountAsync(d =>
                    d.TenantId == tenantId && d.IsSoftDeleted != true);
                result.PageNumber = 1;
                result.PageSize = 10;
                result.TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while creating designation for TenantId {TenantId}: {Message}", tenantId, ex.Message);
                throw new Exception("An error occurred while creating the designation.", ex);
            }
        }

        public async Task<bool> DeleteDesignationAsync(DeleteDesignationRequestDTO dto, long EmployeeId, int id)
        {
            try
            {
                // 🧩 1️⃣ Validate input
                if (dto == null || dto.Id == "0")
                {
                    _logger.LogWarning("⚠️ DeleteDesignationAsync called with invalid DesignationId.");
                    throw new ArgumentException("Invalid DesignationId for deletion.");
                }
               

                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 2️⃣ Fetch existing Designation
                var existingEntity = await context.Designations
                    .FirstOrDefaultAsync(d => d.Id == id && (d.IsSoftDeleted != true));

                if (existingEntity == null)
                {
                    _logger.LogWarning("⚠️ Designation not found for Id: {Id}", dto.Id);
                    return false;
                }

                // 🧩 3️⃣ Perform soft delete
                existingEntity.IsSoftDeleted = true;
                existingEntity.IsActive = false;
                existingEntity.SoftDeletedById = EmployeeId;
                existingEntity.SoftDeletedDateTime = DateTime.UtcNow;
                existingEntity.Id = id;

                context.Designations.Update(existingEntity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Designation Id: {Id} soft deleted successfully by EmployeeId: {EmpId}", dto.Id, EmployeeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting designation Id: {Id}", dto?.Id);
                throw new Exception("An error occurred while deleting the designation.", ex);
            }
        }


        #region Get-Complete
        public async Task<PagedResponseDTO<GetDesignationResponseDTO>> GetAsync(GetDesignationRequestDTO request, long tenantId, int id)
        {
            var response = new PagedResponseDTO<GetDesignationResponseDTO>();

            try
            {
                if (request == null)
                {
                    _logger.LogWarning("⚠️ GetAsync called with null request DTO.");
                    return response;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                int departmentId = 0;
                if (!string.IsNullOrWhiteSpace(request.DepartmentId))
                    departmentId = SafeParser.TryParseInt(request.DepartmentId);

                // ✅ Base query (join with Department)
                var query =
                    from des in context.Designations
                    join dep in context.Departments
                        on des.DepartmentId equals dep.Id into deptGroup
                    from dept in deptGroup.DefaultIfEmpty()
                    where des.TenantId == tenantId
                          && des.IsSoftDeleted != true
                          && (dept == null || (dept.IsSoftDeleted != true && dept.IsActive == true))
                    select new
                    {
                        des,
                        DepartmentName = dept != null ? dept.DepartmentName : string.Empty
                    };

                 
                // ✅ Optional Filters
                if (id > 0)
                    query = query.Where(x => x.des.Id == id);

                if (!string.IsNullOrWhiteSpace(request.DesignationName))
                    query = query.Where(x => x.des.DesignationName.ToLower().Contains(request.DesignationName.ToLower()));

                if (request.IsActive.HasValue)
                    query = query.Where(x => x.des.IsActive == request.IsActive.Value);

                if (departmentId > 0)
                    query = query.Where(x => x.des.DepartmentId == departmentId);


                // ✅ Sorting
                query = request.SortBy?.ToLower() switch
                {
                    "designationname" => request.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.des.DesignationName)
                        : query.OrderByDescending(x => x.des.DesignationName),

                    "departmentname" => request.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.DepartmentName)
                        : query.OrderByDescending(x => x.DepartmentName),

                    _ => query.OrderByDescending(x => x.des.Id) // Default sort by Id descending
                };

                // ✅ Pagination
                var totalRecords = await query.CountAsync();
                var designations = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                // ✅ Mapping to DTO (manual mapping with DepartmentName)
                var mappedList = designations.Select(x =>
                {
                    var dto = _mapper.Map<GetDesignationResponseDTO>(x.des);
                    dto.DepartmentName = x.DepartmentName ?? string.Empty;
                    return dto;
                }).ToList();

                response.Items = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                _logger.LogInformation("✅ Retrieved {Count} designations for TenantId: {TenantId}", mappedList.Count, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching designations for TenantId: {TenantId}", tenantId);
                response.Items = new List<GetDesignationResponseDTO>();
            }

            return response;
        }

        #endregion



        public async Task<bool> UpdateDesignationAsync(UpdateDesignationRequestDTO dto, long employeeId, int id)
        {
            try
            {
                // 🧩 1️⃣ Validate Input
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ UpdateDesignationAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "UpdateDesignationRequestDTO cannot be null.");
                }

                int departmentId = SafeParser.TryParseInt(dto.DepartmentId);
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 2️⃣ Fetch existing record
                var existingEntity = await context.Designations
                    .FirstOrDefaultAsync(d => d.Id == id && (d.IsSoftDeleted != true));

                if (existingEntity == null)
                {
                    _logger.LogWarning("❌ Designation not found for Id: {Id}", id);
                    return false;
                }

                // 🧩 3️⃣ Update fields conditionally
                existingEntity.DesignationName = dto.DesignationName?.Trim() ?? existingEntity.DesignationName;
                existingEntity.Description = dto.Description?.Trim() ?? existingEntity.Description;
                existingEntity.DepartmentId = departmentId > 0 ? departmentId : existingEntity.DepartmentId;

                // ✅ IsActive sirf tab update ho jab dto.IsActive me value ho
                if (dto.IsActive.HasValue)
                    existingEntity.IsActive = dto.IsActive.Value;

                existingEntity.UpdatedById = employeeId;
                existingEntity.UpdatedDateTime = DateTime.UtcNow;

                // 🧩 4️⃣ Save Changes
                context.Designations.Update(existingEntity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Designation '{Name}' (Id: {Id}) updated successfully by EmployeeId: {EmployeeId}",
                    existingEntity.DesignationName, existingEntity.Id, employeeId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while updating designation Id: {Id}", dto?.Id);
                throw new Exception("An error occurred while updating the designation.", ex);
            }
        }


        public async Task<GetSingleDesignationResponseDTO?> GetByIdAsync(GetSingleDesignationRequestDTO dto)
        {
            try
            {
                // 🧩 1️⃣ Validation
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ GetByIdAsync called with null DTO.");
                    throw new ArgumentNullException(nameof(dto), "GetSingleDesignationRequestDTO cannot be null.");
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 2️⃣ Record fetch
                var designation = await context.Designations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d =>
                        d.Id == dto.Id &&
                        
                        (d.IsSoftDeleted!=true));

                if (designation == null)
                {
                    _logger.LogWarning("❌ No Designation found for Id: {Id}", dto.Id);
                    return null;
                }

                // 🧩 3️⃣ Mapping (Entity → DTO)
                var response = _mapper.Map<GetSingleDesignationResponseDTO>(designation);

                _logger.LogInformation("✅ Designation fetched successfully. Id: {Id}", dto.Id);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while fetching Designation by Id {Id}", dto.Id);
                throw new Exception("An error occurred while fetching designation details.", ex);
            }
        }



        public async Task<ApiResponse<List<GetDesignationOptionResponseDTO?>>> GetOptionAsync(GetDesignationOptionRequestDTO dto, long tenantId)
        {
            var response = new ApiResponse<List<GetDesignationOptionResponseDTO?>>();

            try
            {
                using var context = _contextFactory.CreateDbContext();

                // ✅ Parse DepartmentId safely
                int departmentId = 0;
                if (!string.IsNullOrWhiteSpace(dto.DepartmentId))
                    departmentId = SafeParser.TryParseInt(dto.DepartmentId);

                // ✅ Base Query
                var query = context.Designations
                    .Where(x => x.TenantId == tenantId && x.IsSoftDeleted != true && x.IsActive == true);

                // ✅ Conditional filter (apply only if departmentId > 0)
                if (departmentId > 0)
                    query = query.Where(x => x.DepartmentId == departmentId);

                // ✅ Projection
                var designations = await query
                    .OrderBy(x => x.DesignationName)
                    .Select(r => new GetDesignationOptionResponseDTO
                    {
                        Id = r.Id.ToString(),
                        DepartmentId = r.DepartmentId.ToString(),
                        DesignationName = r.DesignationName,
                       // IsActive = r.IsActive
                    })
                    .AsNoTracking()
                    .ToListAsync();

                // ✅ Response setup
                response.Data = designations;
                response.Message = designations.Any()
                    ? "✅ Designation options fetched successfully."
                    : "⚠️ No designations found for this tenant.";

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching designation options for TenantId {TenantId}", tenantId);

                return new ApiResponse<List<GetDesignationOptionResponseDTO?>>
                {
                    Message = "❌ An error occurred while fetching designation options.",
                    Data = new List<GetDesignationOptionResponseDTO?>()
                };
            }
        
        
        }

    }
}
 
