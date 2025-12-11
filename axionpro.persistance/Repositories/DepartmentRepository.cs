using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace axionpro.persistance.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentRepository> _logger;
        private readonly IEncryptionService _encryptionService;

        public DepartmentRepository(
            WorkforceDbContext context,
            ILogger<DepartmentRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory, IEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
            _encryptionService = encryptionService;


        }
        public async Task<GetSingleDepartmentResponseDTO?> GetByIdAsync(GetSingleDepartmentRequestDTO dto)
        {
            try
            {
                var entity = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == dto.Id && d.IsSoftDeleted != true && d.IsActive == true);

                if (entity == null)
                {
                    _logger.LogWarning("No department found with ID: {Id}", dto.Id);
                    return null;
                }

                var dtoResult = _mapper.Map<GetSingleDepartmentResponseDTO>(entity);
                return dtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department by Id: {Id}", dto.Id);
                return null;
            }
        }


        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(GetDepartmentRequestDTO request)
        {
            var response = new PagedResponseDTO<GetDepartmentResponseDTO>();

            try
            {
                if (request == null)
                {
                    _logger.LogWarning("⚠️ GetAsync called with null request.");
                    return response;
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Base Query
                var query = context.Departments
                    .Where(d => d.TenantId == request.Prop.TenantId && d.IsSoftDeleted != true)
                    .AsQueryable();

                // ✅ Optional Filters
                if (request.Id > 0)
                    query = query.Where(d => d.Id == request.Id);

                if (!string.IsNullOrWhiteSpace(request.DepartmentName))
                    query = query.Where(d => d.DepartmentName.ToLower().Contains(request.DepartmentName.ToLower()));

                if (request.IsActive.HasValue)
                    query = query.Where(d => d.IsActive == request.IsActive.Value);

                // ✅ Sorting
                query = request.SortBy?.ToLower() switch
                {
                    "departmentname" => request.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.DepartmentName)
                        : query.OrderByDescending(x => x.DepartmentName),

                    "addedbyid" => request.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.AddedById)
                        : query.OrderByDescending(x => x.AddedById),

                    "addeddatetime" => request.SortOrder?.ToLower() == "asc"
                        ? query.OrderBy(x => x.AddedDateTime)
                        : query.OrderByDescending(x => x.AddedDateTime),

                    _ => query.OrderByDescending(x => x.Id) // ✅ Default sort by Id (desc)
                };

                // ✅ Pagination
                var totalRecords = await query.CountAsync();
                var departments = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                var mappedList = _mapper.Map<List<GetDepartmentResponseDTO>>(departments);

                // ✅ Response Setup
                response.Items = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                _logger.LogInformation("✅ Retrieved departments mappedList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching departments");
                response.Items = new List<GetDepartmentResponseDTO>();
            }

            return response;
        }


        /// <summary>
        /// Creates a new Department entry in the database.
        /// </summary>
        /// <param name="dto">The DTO containing department creation details.</param>
        /// <returns>
        /// Returns a list containing the created department details mapped to <see cref="GetDepartmentResponseDTO"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when the input DTO is null.</exception>
        /// <exception cref="Exception">Thrown when any error occurs during database operations.</exception>
        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> CreateAsync(CreateDepartmentRequestDTO dto)
        {
            var result = new PagedResponseDTO<GetDepartmentResponseDTO>();

            try
            {
                const int pageNumber = 1;
                const int pageSize = 10;

                // 🧩 1️⃣ Validate Input
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ CreateAsync called with null Department DTO for TenantId: {TenantId}", dto.Prop.TenantId);
                    throw new ArgumentNullException(nameof(dto), "Department object cannot be null.");
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 2️⃣ Check for Duplicate Department (case-insensitive)
                bool exists = await context.Departments
                    .AnyAsync(d =>
                        d.TenantId == dto.Prop.TenantId &&
                        EF.Functions.Like(d.DepartmentName.ToLower(), dto.DepartmentName.ToLower()) &&
                        (d.IsSoftDeleted != true));

                if (exists)
                {
                    _logger.LogWarning("⚠️ Department '{Name}' already exists for TenantId {TenantId}.", dto.DepartmentName, dto.Prop.TenantId);

                    result.Items = new List<GetDepartmentResponseDTO>();
                    result.TotalCount = 0;
                    result.PageNumber = pageNumber;
                    result.PageSize = pageSize;
                    return result;
                }

                // 🧩 3️⃣ Map DTO → Entity
                var entity = _mapper.Map<Department>(dto);
                entity.TenantId = dto.Prop.TenantId;
                entity.AddedById = dto.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.IsExecutiveOffice = false;

                // 🧩 4️⃣ Save to Database
                await context.Departments.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Department '{Name}' created successfully with Id: {Id} for TenantId: {TenantId}",
                    dto.DepartmentName, entity.Id, dto.Prop.TenantId);

                // 🧩 5️⃣ Fetch latest 10 active departments (newest first)
                var query = context.Departments
                    .AsNoTracking()
                    .Where(d => d.TenantId == dto.Prop.TenantId && (d.IsSoftDeleted !=true))
                    .OrderByDescending(d => d.Id);

                int totalCount = await query.CountAsync();

                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 🧩 6️⃣ Map to DTO
                var mappedData = _mapper.Map<List<GetDepartmentResponseDTO>>(pagedData);

                // 🧩 7️⃣ Prepare Paged Response
                result.Items = mappedData;
                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
                result.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while creating department for TenantId {TenantId}: {Message}", dto.Prop.TenantId, ex.Message);
                throw new Exception("An error occurred while creating the department.", ex);
            }
        }


        /// <summary>
        /// Updates an existing Department record with validation and logging.
        /// </summary>
        /// <param name="department">The DTO containing updated department details.</param>
        /// <returns>True if update succeeds; otherwise false.</returns>
        public async Task<bool> UpdateAsync(UpdateDepartmentRequestDTO requestDTO)
        {
            try
            {
                // 🔹 Step 1: Input validation
                if (requestDTO == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    return false;
                }
               

                // 🔹 Step 2: Fetch existing department
                var existing = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == requestDTO.Id && d.IsSoftDeleted != true);


                if (existing == null)
                {
                    _logger.LogWarning("Update failed: Department not found for Id: {Id}", requestDTO.Id);
                    return false;
                }
                if (existing.IsExecutiveOffice != true)
                {

                    // 🔹 Step 3: Field updates with null-safety
                    if (!string.IsNullOrWhiteSpace(requestDTO.DepartmentName))
                        existing.DepartmentName = requestDTO.DepartmentName;

                    if (!string.IsNullOrWhiteSpace(requestDTO.Description))
                        existing.Description = requestDTO.Description;

                    if (!string.IsNullOrWhiteSpace(requestDTO.Remark))
                        existing.Remark = requestDTO.Remark;

                    if (requestDTO.IsActive.HasValue)
                        existing.IsActive = requestDTO.IsActive.Value;

                    existing.UpdatedById = requestDTO.Prop.UserEmployeeId;
                    existing.UpdatedDateTime = DateTime.UtcNow;

                    // Optional: Handle TenantIndustryId only if present
                    if (requestDTO.GetType().GetProperty("TenantIndustryId") != null)
                    {
                        var tenantIndustryIdValue = requestDTO.GetType().GetProperty("TenantIndustryId")?.GetValue(requestDTO);
                        if (tenantIndustryIdValue != null)
                            existing.GetType().GetProperty("TenantIndustryId")?.SetValue(existing, tenantIndustryIdValue);
                    }


                    // 🔹 Step 4: Save changes
                    _context.Departments.Update(existing);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Department updated successfully. Id: {Id}", requestDTO.Id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating department with Id: {Id}", requestDTO?.Id);
                return false;
            }
        }
        public async Task<bool> DeleteAsync(DeleteDepartmentRequestDTO dto, long employeeId, int id)
        {
            try
            {
              

                // ✅ Record fetch with safety check
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == dto.Id && d.IsSoftDeleted != true);

                if (department == null)
                {
                    _logger.LogWarning("Department not found or already deleted. Id: {Id}", dto.Id);
                    return false;
                }

                // ✅ Soft delete setup
                department.IsSoftDeleted = true;
                department.IsActive = false;
                department.SoftDeletedById = employeeId;
                department.DeletedDateTime = DateTime.UtcNow; // Use UTC for consistency

                // ✅ Update and save
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Department soft deleted successfully. Id: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department. Id: {Id}", dto?.Id);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(long id, long tenantId)
        {
            try
            {
                return await _context.Departments.AnyAsync(d => d.Id == id && d.TenantId == tenantId && !d.IsSoftDeleted == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of department. Id: {Id}", id);
                return false;
            }
        }
        public async Task<Dictionary<string, int>> GetDepartmentNameIdMapAsync(long tenantId)
        {
            return await _context.Departments
                .Where(d => !d.IsSoftDeleted == true && d.TenantId == tenantId)
                .ToDictionaryAsync(d => d.DepartmentName, d => d.Id);
        }


        public async Task<int> AutoCreateDepartmentSeedAsync(List<Department>? departments)
        {
            try
            {
                if (departments == null || !departments.Any())
                {
                    _logger.LogWarning("Department seed list is null or empty. Seeding aborted.");
                    return -1;
                }

                await _context.Departments.AddRangeAsync(departments);
                int insertedCount = await _context.SaveChangesAsync();

                if (insertedCount != departments.Count)
                {
                    _logger.LogError("Mismatch in inserted department count. Expected: {Expected}, Inserted: {Inserted}", departments.Count, insertedCount);
                    return -1;
                }

                long tenantId = departments.FirstOrDefault()?.TenantId ?? 0;

                // ✅ Fetch the inserted Executive Office department's ID
                var executiveOfficeDeptId = await _context.Departments
                    .Where(d => d.IsExecutiveOffice == true && !d.IsSoftDeleted == true && d.TenantId == tenantId)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync();

                if (executiveOfficeDeptId == 0)
                {
                    _logger.LogWarning("Executive Office department not found after insertion.");
                    return -1;
                }

                _logger.LogInformation("Successfully inserted {Count} departments. Executive Office DepartmentId: {ExecutiveOfficeId}", insertedCount, executiveOfficeDeptId);
                return executiveOfficeDeptId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while inserting department seed data.");
                return -1;
            }
        }

        public async Task<ApiResponse<List<GetDepartmentOptionResponse?>>> GetOptionAsync(GetOptionRequestDTO dto)
        {
            {
                var response = new ApiResponse<List<GetDepartmentOptionResponse?>>();

                try
                {
                    using var context = _contextFactory.CreateDbContext();

                    // ✅ Parse DepartmentId safely


                    var query = context.Departments
                        .Where(x => x.TenantId == dto.Prop.TenantId && x.IsSoftDeleted != true && x.IsActive == true);


                    // ✅ Projection
                    var designations = await query
                        .OrderBy(x => x.DepartmentName)
                        .Select(r => new GetDepartmentOptionResponse
                        {
                            Id = r.Id,
                            DepartmentName = r.DepartmentName.ToString(),

                            // IsActive = r.IsActive
                        })
                        .AsNoTracking()
                        .ToListAsync();

                    // ✅ Response setup
                    response.Data = designations;
                    response.Message = designations.Any()
                        ? "✅ Department options fetched successfully."
                        : "⚠️ No Department found for this tenant.";

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error fetching department options");

                    return new ApiResponse<List<GetDepartmentOptionResponse?>>
                    {
                        Message = "❌ An error occurred while fetching department options.",
                        Data = new List<GetDepartmentOptionResponse?>()
                    };
                }

            }


        }

    }
}
