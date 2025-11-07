using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.UserRole;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
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
                    .FirstOrDefaultAsync(d => d.Id == dto.Id && d.IsSoftDeleted!=true && d.IsActive == true);

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
         

        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(GetDepartmentRequestDTO request, long tenantId)
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

                int Id = 0;
                if (request.Id != "0" || request.Id != null)
                {

                    Id = SafeParser.TryParseInt(request.Id);
                }
                var query = context.Departments
                    .Where(d => d.TenantId == tenantId && (d.IsSoftDeleted !=true))
                    .AsQueryable();

                // ✅ Optional Filters
                if ( Id > 0)
                    query = query.Where(d => d.Id == Id);

                if (!string.IsNullOrWhiteSpace(request.DepartmentName))
                    query = query.Where(d => d.DepartmentName.ToLower().Contains(request.DepartmentName.ToLower()));

                if (request.IsActive.HasValue)
                    query = query.Where(d => d.IsActive == request.IsActive.Value);

                // ✅ Sorting
                query = request.SortOrder?.ToLower() == "asc" ? query.OrderBy(x => x.DepartmentName) : query.OrderByDescending(x => x.DepartmentName);

                //query = request.SortOrder?.ToLower() == "asc"
                //    ? query.OrderBy(x => x.DepartmentName)
                //    : query.OrderByDescending(x => x.DepartmentName);

                // ✅ Pagination
                var totalRecords = await query.CountAsync();
                var departments = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                var mappedList = _mapper.Map<List<GetDepartmentResponseDTO>>(departments);

                response.Items = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;
              

                _logger.LogInformation("✅ Retrieved {Count} departments for TenantId: {TenantId}", mappedList.Count, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching departments for TenantId: {TenantId}", tenantId);
                new List<GetDepartmentResponseDTO>();
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
       public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> CreateAsync(CreateDepartmentRequestDTO dto, long TenantId , long EmployeeId )
        {
            var result = new PagedResponseDTO<GetDepartmentResponseDTO>();

            try
            {
                // 🧩 1️⃣ Validate input
                if (dto == null)
                {
                    _logger.LogWarning("⚠️ CreateAsync called with null department DTO.");
                    throw new ArgumentNullException(nameof(dto), "Department object cannot be null.");
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🧩 2️⃣ Check if same department already exists under same tenant
                bool exists = await context.Departments
                    .AnyAsync(d =>
                        d.TenantId == TenantId &&
                        d.DepartmentName.ToLower() == dto.DepartmentName.ToLower() &&
                        (d.IsSoftDeleted == null || d.IsSoftDeleted == false));

                if (exists)
                {
                    _logger.LogWarning("⚠️ Department '{Name}' already exists for TenantId {TenantId}.",
                    dto.DepartmentName, TenantId);                                       
                    result.Items = new List<GetDepartmentResponseDTO>();
                    result.TotalCount = 0;
                    return result;
                }
                
                // 🧩 3️⃣ Map DTO → Entity
                var entity = _mapper.Map<Department>(dto);
                entity.AddedById = EmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.IsExecutiveOffice = false;

                // 🧩 4️⃣ Save to database
                await context.Departments.AddAsync(entity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ Department created successfully with Id: {Id}", entity.Id);

                // 🧩 5️⃣ Fetch all active departments after insert
                var query = context.Departments
                    .AsNoTracking()
                    .Where(d =>
                        d.TenantId == TenantId &&
                        (d.IsSoftDeleted == null || d.IsSoftDeleted == false))
                    .OrderByDescending(d => d.Id);

                // 🧩 6️⃣ Pagination & Count
                int totalCount = await query.CountAsync();
                int pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
                int pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // 🧩 7️⃣ Map to Response DTO
                var mappedData = _mapper.Map<List<GetDepartmentResponseDTO>>(pagedData);

                // 🧩 8️⃣ Prepare response
                result.Items = mappedData;
                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;

             

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while creating department: {Message}", ex.Message);
                throw new Exception("An error occurred while creating the department.", ex);
            }
        }


        /// <summary>
        /// Updates an existing Department record with validation and logging.
        /// </summary>
        /// <param name="department">The DTO containing updated department details.</param>
        /// <returns>True if update succeeds; otherwise false.</returns>
        public async Task<bool> UpdateAsync(UpdateDepartmentRequestDTO requestDTO, long EmployeeId)
        {
            try
            {



                // 🔹 Step 1: Input validation
                if (requestDTO == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    return false;
                }
                int Id = 0;
                if (requestDTO.Id != null)
                { 
                    Id = SafeParser.TryParseInt(requestDTO.Id);
                }

                // 🔹 Step 2: Fetch existing department
                var existing = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == Id && d.IsSoftDeleted != true);


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

                    existing.IsActive = requestDTO.IsActive;
                    existing.UpdatedById = EmployeeId;
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
        public async Task<bool> DeleteAsync(DeleteDepartmentRequestDTO dto, long employeeId)
        {
            try
            {
                // ✅ Null aur invalid ID validation
                if (dto == null || dto.Id == "0")
                {
                    _logger.LogWarning("Invalid DeleteDepartmentRequestDTO or Id: {Id}", dto?.Id);
                    return false;
                }

                int id = SafeParser.TryParseInt(dto.Id);
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid Department Id provided for deletion: {Id}", dto.Id);
                    return false;
                }

                // ✅ Record fetch with safety check
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsSoftDeleted != true);

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
                .Where(d => !d.IsSoftDeleted==true && d.TenantId == tenantId)
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
                    .Where(d => d.IsExecutiveOffice ==true && !d.IsSoftDeleted==true && d.TenantId== tenantId)                  
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

        public async Task<ApiResponse<List<GetDepartmentOptionResponse?>>> GetOptionAsync(GetOptionRequestDTO dto, long TenantId)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var departments = await context.Departments
                    .Where(x => x.TenantId == TenantId && x.IsSoftDeleted !=true && x.IsActive == true)
                    .Select(d => new GetDepartmentOptionResponse
                    {
                        Id = d.Id,
                        DepartmentName = d.DepartmentName,
                        IsActive = d.IsActive 
                        
                    })
                    .ToListAsync();

                // ✅ Always wrap inside ApiResponse
                return new ApiResponse<List<GetDepartmentOptionResponse?>>
                {
                  
                    Message = departments.Count > 0 ? "Department options fetched successfully." : "No departments found.",
                    Data = departments
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department options");

                return new ApiResponse<List<GetDepartmentOptionResponse?>>
                {
                    
                    Message = "An error occurred while fetching department options.",
                    Data = new List<GetDepartmentOptionResponse?>()
                };
            }
        }


    }

}
