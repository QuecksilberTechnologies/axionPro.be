using AutoMapper;
using axionpro.application.DTOs.Department;
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
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentRepository> _logger;
        private readonly IEncryptionService _encryptionService;

        public DepartmentRepository(
            WorkforceDbContext context,
            ILogger<DepartmentRepository> logger,
            IMapper mapper,
            IEncryptionService encryptionService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public async Task<GetSingleDepartmentResponseDTO?> GetByIdAsync(
            GetSingleDepartmentRequestDTO dto,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("GetByIdAsync called with null dto.");
                    return null;
                }

                var entity = await _context.Departments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        d => d.Id == dto.Id &&
                             d.IsSoftDeleted != true &&
                             d.IsActive == true,
                        cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("No department found with ID: {Id}", dto.Id);
                    return null;
                }

                return _mapper.Map<GetSingleDepartmentResponseDTO>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department by Id: {Id}", dto?.Id);
                throw;
            }
        }

        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(
            GetDepartmentRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            var response = new PagedResponseDTO<GetDepartmentResponseDTO>();

            try
            {
                if (request == null)
                {
                    _logger.LogWarning("GetAsync called with null request.");
                    return response;
                }

                var query = _context.Departments
                    .AsNoTracking()
                    .Where(d => d.TenantId == request.Prop.TenantId && d.IsSoftDeleted != true)
                    .AsQueryable();

                if (request.Id > 0)
                    query = query.Where(d => d.Id == request.Id);

                if (!string.IsNullOrWhiteSpace(request.DepartmentName))
                    query = query.Where(d => d.DepartmentName.ToLower().Contains(request.DepartmentName.ToLower()));

                if (request.IsActive.HasValue)
                    query = query.Where(d => d.IsActive == request.IsActive.Value);

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

                    _ => query.OrderByDescending(x => x.Id)
                };

                var totalRecords = await query.CountAsync(cancellationToken);

                var departments = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var mappedList = _mapper.Map<List<GetDepartmentResponseDTO>>(departments);

                response.Items = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                _logger.LogInformation("Retrieved {Count} departments for TenantId: {TenantId}", mappedList.Count, request.Prop.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching departments.");
                response.Items = new List<GetDepartmentResponseDTO>();
            }

            return response;
        }

        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> CreateAsync(
            CreateDepartmentRequestDTO dto,
            CancellationToken cancellationToken = default)
        {
            var result = new PagedResponseDTO<GetDepartmentResponseDTO>();

            try
            {
                const int pageNumber = 1;
                const int pageSize = 10;

                if (dto == null)
                {
                    _logger.LogWarning("CreateAsync called with null Department DTO.");
                    throw new ArgumentNullException(nameof(dto), "Department object cannot be null.");
                }

                bool exists = await _context.Departments
                    .AnyAsync(d =>
                        d.TenantId == dto.Prop.TenantId &&
                        d.DepartmentName.ToLower() == dto.DepartmentName.ToLower() &&
                        d.IsSoftDeleted != true,
                        cancellationToken);

                if (exists)
                {
                    _logger.LogWarning(
                        "Department '{Name}' already exists for TenantId {TenantId}.",
                        dto.DepartmentName,
                        dto.Prop.TenantId);

                    result.Items = new List<GetDepartmentResponseDTO>();
                    result.TotalCount = 0;
                    result.PageNumber = pageNumber;
                    result.PageSize = pageSize;
                    result.TotalPages = 0;
                    return result;
                }

                var entity = _mapper.Map<Department>(dto);
                entity.TenantId = dto.Prop.TenantId;
                entity.AddedById = dto.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.IsExecutiveOffice = false;

                await _context.Departments.AddAsync(entity, cancellationToken);

                // NOTE:
                // For independent CRUD screens this is okay for now.
                // For transaction-controlled flows like tenant creation, do not use CreateAsync.
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Department '{Name}' created successfully with Id: {Id} for TenantId: {TenantId}",
                    dto.DepartmentName,
                    entity.Id,
                    dto.Prop.TenantId);

                var query = _context.Departments
                    .AsNoTracking()
                    .Where(d => d.TenantId == dto.Prop.TenantId && d.IsSoftDeleted != true)
                    .OrderByDescending(d => d.Id);

                int totalCount = await query.CountAsync(cancellationToken);

                var pagedData = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var mappedData = _mapper.Map<List<GetDepartmentResponseDTO>>(pagedData);

                result.Items = mappedData;
                result.TotalCount = totalCount;
                result.PageNumber = pageNumber;
                result.PageSize = pageSize;
                result.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating department for TenantId {TenantId}", dto?.Prop?.TenantId);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(
            UpdateDepartmentRequestDTO requestDTO,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (requestDTO == null)
                {
                    _logger.LogWarning("UpdateAsync called with null DTO.");
                    return false;
                }

                var existing = await _context.Departments
                    .FirstOrDefaultAsync(
                        d => d.Id == requestDTO.Id && d.IsSoftDeleted != true,
                        cancellationToken);

                if (existing == null)
                {
                    _logger.LogWarning("Update failed: Department not found for Id: {Id}", requestDTO.Id);
                    return false;
                }

                if (existing.IsExecutiveOffice == true)
                {
                    _logger.LogWarning("Executive Office department cannot be updated. Id: {Id}", requestDTO.Id);
                    return false;
                }

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

                _context.Departments.Update(existing);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department updated successfully. Id: {Id}", requestDTO.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating department with Id: {Id}", requestDTO?.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(
            DeleteDepartmentRequestDTO dto,
            long employeeId,
            int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(
                        d => d.Id == dto.Id && d.IsSoftDeleted != true,
                        cancellationToken);

                if (department == null)
                {
                    _logger.LogWarning("Department not found or already deleted. Id: {Id}", dto.Id);
                    return false;
                }

                department.IsSoftDeleted = true;
                department.IsActive = false;
                department.SoftDeletedById = employeeId;
                department.DeletedDateTime = DateTime.UtcNow;

                _context.Departments.Update(department);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department soft deleted successfully. Id: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department. Id: {Id}", dto?.Id);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(
            long id,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Departments.AnyAsync(
                    d => d.Id == id &&
                         d.TenantId == tenantId &&
                         d.IsSoftDeleted != true,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking existence of department. Id: {Id}", id);
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetDepartmentNameIdMapAsync(
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Departments
                    .AsNoTracking()
                    .Where(d => d.TenantId == tenantId && d.IsSoftDeleted != true)
                    .ToDictionaryAsync(d => d.DepartmentName, d => d.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting department name-id map for TenantId: {TenantId}", tenantId);
                throw;
            }
        }

                    public async Task<bool> AutoCreateDepartmentSeedAsync(
                    List<Department>? departments,
                    CancellationToken cancellationToken = default)
                {
                    try
                    {
                        if (departments == null || !departments.Any())
                        {
                            _logger.LogWarning("Department seed list is null or empty. Seeding aborted.");
                            return false;
                        }

                        await _context.Departments.AddRangeAsync(departments, cancellationToken);

                        _logger.LogInformation(
                            "Department seeds added to DbContext successfully. Count: {Count}",
                            departments.Count);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Exception occurred while adding department seed data to context.");
                        return false;
                    }
                }
        public async Task<ApiResponse<List<GetDepartmentOptionResponse?>>> GetOptionAsync(
            GetOptionRequestDTO dto,
            CancellationToken cancellationToken = default)
        {
            var response = new ApiResponse<List<GetDepartmentOptionResponse?>>();

            try
            {
                var departments = await _context.Departments
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == dto.Prop.TenantId &&
                        x.IsSoftDeleted != true &&
                        x.IsActive == true)
                    .OrderBy(x => x.DepartmentName)
                    .Select(r => new GetDepartmentOptionResponse
                    {
                        Id = r.Id,
                        DepartmentName = r.DepartmentName
                    })
                    .ToListAsync(cancellationToken);

                response.Data = departments;
                response.Message = departments.Any()
                    ? "Department options fetched successfully."
                    : "No department found for this tenant.";

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department options.");

                return new ApiResponse<List<GetDepartmentOptionResponse?>>
                {
                    Message = "An error occurred while fetching department options.",
                    Data = new List<GetDepartmentOptionResponse?>()
                };
            }
        }

       
    }
}