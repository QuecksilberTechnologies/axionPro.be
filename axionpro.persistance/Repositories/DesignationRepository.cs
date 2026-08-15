// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Persists and projects tenant-scoped designation data.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Provides tenant-scoped persistence and projection operations for designations.
    /// </summary>
    public class DesignationRepository : IDesignationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DesignationRepository> _logger;

        /// <summary>
        /// Initializes repository dependencies.
        /// </summary>
        public DesignationRepository(
            WorkforceDbContext context,
            ILogger<DesignationRepository> logger,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        #region Create

        /// <summary>
        /// Persists designation seed entities and returns the executive-office designation identifier.
        /// </summary>
        public async Task<int> AutoCreateDesignationAsync(List<Designation> designations, int departmentId)
        {
            try
            {
                if (designations == null || !designations.Any())
                {
                    _logger.LogWarning("Designation seed list is null or empty. Seeding aborted.");
                    return 0;
                }

                var tenantId = designations.First().TenantId;
                await _context.Designations.AddRangeAsync(designations);
                var savedCount = await _context.SaveChangesAsync();

                if (savedCount != designations.Count)
                {
                    _logger.LogWarning(
                        "Designation seed count mismatch. Expected: {Expected}; saved: {Saved}.",
                        designations.Count,
                        savedCount);
                    return 0;
                }

                return designations.FirstOrDefault(
                    designation => designation.Department?.TenantId == tenantId &&
                                   designation.Department.IsExecutiveOffice)?.Id ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating designation seeds.");
                return 0;
            }
        }

        /// <summary>
        /// Persists a designation entity after enforcing tenant and department constraints.
        /// </summary>
        public async Task<Designation?> CreateAsync(
            Designation entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var departmentExists = await _context.Departments.AnyAsync(
                department => department.Id == entity.DepartmentId &&
                              department.TenantId == entity.TenantId &&
                              department.IsActive == true &&
                              department.IsSoftDeleted != true,
                cancellationToken);

            if (!departmentExists)
            {
                _logger.LogWarning(
                    "Department {DepartmentId} is unavailable for TenantId {TenantId}.",
                    entity.DepartmentId,
                    entity.TenantId);
                return null;
            }

            var exists = await _context.Designations.AnyAsync(
                designation => designation.TenantId == entity.TenantId &&
                               designation.DesignationName.ToLower() == entity.DesignationName.ToLower() &&
                               designation.IsSoftDeleted != true,
                cancellationToken);

            if (exists)
            {
                _logger.LogWarning(
                    "Designation {DesignationName} already exists for TenantId {TenantId}.",
                    entity.DesignationName,
                    entity.TenantId);
                return null;
            }

            await _context.Designations.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        #endregion

        #region Update

        /// <summary>
        /// Gets a mutable designation entity after enforcing tenant ownership.
        /// </summary>
        public Task<Designation?> GetByIdForTenantAsync(
            int id,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            return _context.Designations.FirstOrDefaultAsync(
                designation => designation.Id == id &&
                               designation.TenantId == tenantId &&
                               designation.IsSoftDeleted != true,
                cancellationToken);
        }

        /// <summary>
        /// Persists changes to a prepared designation entity.
        /// </summary>
        public async Task<bool> UpdateDesignationAsync(
            Designation entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var existing = await _context.Designations.FirstOrDefaultAsync(
                designation => designation.Id == entity.Id &&
                               designation.TenantId == entity.TenantId &&
                               designation.IsSoftDeleted != true,
                cancellationToken);

            if (existing == null)
                return false;

            var departmentExists = await _context.Departments.AnyAsync(
                department => department.Id == entity.DepartmentId &&
                              department.TenantId == entity.TenantId &&
                              department.IsActive == true &&
                              department.IsSoftDeleted != true,
                cancellationToken);

            if (!departmentExists)
                return false;

            existing.DesignationName = entity.DesignationName;
            existing.Description = entity.Description;
            existing.DepartmentId = entity.DepartmentId;
            existing.IsActive = entity.IsActive;
            existing.UpdatedById = entity.UpdatedById;
            existing.UpdatedDateTime = entity.UpdatedDateTime;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Soft deletes a designation after enforcing tenant ownership.
        /// </summary>
        public async Task<bool> DeleteDesignationAsync(
            int id,
            long tenantId,
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.Designations.FirstOrDefaultAsync(
                designation => designation.Id == id &&
                               designation.TenantId == tenantId &&
                               designation.IsSoftDeleted != true,
                cancellationToken);

            if (entity == null)
                return false;

            entity.IsSoftDeleted = true;
            entity.IsActive = false;
            entity.SoftDeletedById = employeeId;
            entity.SoftDeletedDateTime = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        #endregion

        #region Queries

        /// <summary>
        /// Determines whether a designation name already exists within a tenant.
        /// </summary>
        public Task<bool> CheckDuplicateValueAsync(long tenantId, string value)
        {
            return _context.Designations.AnyAsync(
                designation => designation.TenantId == tenantId &&
                               designation.IsSoftDeleted != true &&
                               designation.DesignationName.ToLower() == value.Trim().ToLower());
        }

        /// <summary>
        /// Gets paged designation projections for a trusted tenant.
        /// </summary>
        public async Task<PagedResponseDTO<GetDesignationResponseDTO>> GetAsync(
            GetDesignationRequestDTO request,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var query =
                from designation in _context.Designations.AsNoTracking()
                join department in _context.Departments.AsNoTracking()
                    on designation.DepartmentId equals department.Id into departments
                from department in departments.DefaultIfEmpty()
                where designation.TenantId == tenantId &&
                      designation.IsSoftDeleted != true &&
                      (department == null || (department.IsSoftDeleted != true && department.IsActive == true))
                select new
                {
                    Designation = designation,
                    DepartmentName = department != null ? department.DepartmentName : string.Empty
                };

            if (request.DepartmentId > 0)
                query = query.Where(item => item.Designation.DepartmentId == request.DepartmentId);
            if (!string.IsNullOrWhiteSpace(request.DesignationName))
                query = query.Where(item => item.Designation.DesignationName.ToLower().Contains(request.DesignationName.ToLower()));
            if (request.IsActive.HasValue)
                query = query.Where(item => item.Designation.IsActive == request.IsActive.Value);

            query = request.SortBy?.ToLower() switch
            {
                "designationname" => request.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(item => item.Designation.DesignationName)
                    : query.OrderByDescending(item => item.Designation.DesignationName),
                "departmentname" => request.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(item => item.DepartmentName)
                    : query.OrderByDescending(item => item.DepartmentName),
                _ => query.OrderByDescending(item => item.Designation.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(item => new GetDesignationResponseDTO
                {
                    Id = item.Designation.Id,
                    DepartmentId = item.Designation.DepartmentId,
                    DesignationName = item.Designation.DesignationName,
                    DepartmentName = item.DepartmentName,
                    Description = item.Designation.Description,
                    IsActive = item.Designation.IsActive
                })
                .ToListAsync(cancellationToken);

            return new PagedResponseDTO<GetDesignationResponseDTO>(data, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Gets a designation projection by identifier.
        /// </summary>
        public async Task<GetSingleDesignationResponseDTO?> GetByIdAsync(GetSingleDesignationRequestDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var designation = await _context.Designations
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == dto.Id && item.IsSoftDeleted != true);

            return designation == null
                ? null
                : _mapper.Map<GetSingleDesignationResponseDTO>(designation);
        }

        /// <summary>
        /// Gets active designation options for a department within a trusted tenant.
        /// </summary>
        public Task<List<GetDesignationOptionResponseDTO>> GetOptionAsync(
            int departmentId,
            long tenantId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Designations
                .AsNoTracking()
                .Where(designation => designation.TenantId == tenantId &&
                                      designation.IsSoftDeleted != true &&
                                      designation.IsActive == true);

            if (departmentId > 0)
                query = query.Where(designation => designation.DepartmentId == departmentId);

            return query
                .OrderBy(designation => designation.DesignationName)
                .Select(designation => new GetDesignationOptionResponseDTO
                {
                    Id = designation.Id,
                    DepartmentId = designation.DepartmentId,
                    DesignationName = designation.DesignationName
                })
                .ToListAsync(cancellationToken);
        }

        #endregion
    }
}
