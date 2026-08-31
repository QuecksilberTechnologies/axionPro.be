// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Persists and projects tenant-scoped department data.
// ================================================================

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
    /// <summary>
    /// Provides persistence operations for departments.
    /// </summary>
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

        #region Read

        /// <summary>
        /// Gets a paged department projection scoped to the trusted tenant.
        /// </summary>
        public async Task<PagedResponseDTO<GetDepartmentResponseDTO>> GetAsync(
            GetDepartmentRequestDTO request,
            long tenantId,
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
                    .Where(d => d.TenantId == tenantId && d.IsSoftDeleted != true)
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

                response.Data = mappedList;
                response.TotalCount = totalRecords;
                response.PageNumber = request.PageNumber;
                response.PageSize = request.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                _logger.LogInformation("Retrieved {Count} departments for TenantId: {TenantId}", mappedList.Count, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching departments.");
                throw;
            }

            return response;
        }

        /// <summary>
        /// Gets a mutable department entity after enforcing tenant ownership.
        /// </summary>
        public Task<Department?> GetByIdForTenantAsync(
            int id,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.Departments.FirstOrDefaultAsync(
                department => department.Id == id &&
                              department.TenantId == tenantId &&
                              department.IsSoftDeleted != true,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates a department using the supplied domain entity.
        /// </summary>
        /// <param name="entity">The department entity to persist.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The created entity, or <see langword="null"/> when a duplicate department exists.</returns>
        public async Task<Department?> CreateAsync(
            Department entity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("CreateAsync called with a null Department entity.");
                    throw new ArgumentNullException(nameof(entity), "Department entity cannot be null.");
                }

                bool exists = await _context.Departments
                    .AnyAsync(d =>
                        d.TenantId == entity.TenantId &&
                        d.DepartmentName.ToLower() == entity.DepartmentName.ToLower() &&
                        d.IsSoftDeleted != true,
                        cancellationToken);

                if (exists)
                {
                    _logger.LogWarning(
                        "Department '{Name}' already exists for TenantId {TenantId}.",
                        entity.DepartmentName,
                        entity.TenantId);

                    return null;
                }

                await _context.Departments.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Department '{Name}' created successfully with Id: {Id} for TenantId: {TenantId}",
                    entity.DepartmentName,
                    entity.Id,
                    entity.TenantId);

                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating department for TenantId {TenantId}", entity?.TenantId);
                throw;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Persists a department entity that has already been validated and prepared by the handler.
        /// </summary>
        public async Task<bool> UpdateAsync(
            Department entity,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("UpdateAsync called with a null Department entity.");
                    return false;
                }

                var existing = await _context.Departments
                    .FirstOrDefaultAsync(
                        d => d.Id == entity.Id &&
                             d.TenantId == entity.TenantId &&
                             d.IsSoftDeleted != true,
                        cancellationToken);

                if (existing == null)
                {
                    _logger.LogWarning("Update failed: Department not found for Id: {Id}", entity.Id);
                    return false;
                }

                if (existing.IsExecutiveOffice == true)
                {
                    _logger.LogWarning("Executive Office department cannot be updated. Id: {Id}", entity.Id);
                    return false;
                }

                existing.DepartmentName = entity.DepartmentName;
                existing.Description = entity.Description;
                existing.Remark = entity.Remark;
                existing.IsActive = entity.IsActive;
                existing.UpdatedById = entity.UpdatedById;
                existing.UpdatedDateTime = entity.UpdatedDateTime;

                _context.Departments.Update(existing);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department updated successfully. Id: {Id}", entity.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating department with Id: {Id}", entity?.Id);
                throw;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Determines whether the Department remains assigned to an employee in the same tenant.
        /// The IsActive flag is deliberately excluded: an inactive employee is still a valid
        /// dependency until that employee record is soft deleted.
        /// </summary>
        public Task<bool> HasNonDeletedEmployeesAsync(
            int departmentId,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.Employees
                .AsNoTracking()
                .AnyAsync(
                    employee => employee.TenantId == tenantId &&
                                employee.DepartmentId == departmentId &&
                                !employee.IsSoftDeleted,
                    cancellationToken);
        }

        /// <summary>
        /// Soft deletes a department after enforcing tenant ownership.
        /// </summary>
        public async Task<bool> DeleteAsync(
            int id,
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(
                        d => d.Id == id &&
                             d.TenantId == tenantId &&
                             d.IsSoftDeleted != true,
                        cancellationToken);

                if (department == null)
                {
                    _logger.LogWarning("Department not found or already deleted. Id: {Id}", id);
                    return false;
                }

                department.IsSoftDeleted = true;
                department.IsActive = false;
                department.SoftDeletedById = employeeId;
                department.DeletedDateTime = DateTime.UtcNow;

                _context.Departments.Update(department);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department soft deleted successfully. Id: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department. Id: {Id}", id);
                throw;
            }
        }

        #endregion

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
                throw;
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
        /// <summary>
        /// Gets active department options for a trusted tenant.
        /// </summary>
        public async Task<List<GetDepartmentOptionResponse>> GetOptionAsync(
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var departments = await _context.Departments
                    .AsNoTracking()
                    .Where(x =>
                        x.TenantId == tenantId &&
                        x.IsSoftDeleted != true &&
                        x.IsActive == true)
                    .OrderBy(x => x.DepartmentName)
                    .Select(r => new GetDepartmentOptionResponse
                    {
                        Id = r.Id,
                        DepartmentName = r.DepartmentName
                    })
                    .ToListAsync(cancellationToken);

                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching department options.");

                throw;
            }
        }

       
    }
}
