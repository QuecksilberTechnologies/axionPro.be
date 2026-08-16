// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Persists and retrieves tenant-owned asset statuses.
// ================================================================

using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories;

/// <summary>
/// Provides database operations for asset statuses.
/// </summary>
public class AssetStatusRepository : IAssetStatusRepository
{
    private readonly WorkforceDbContext _context;
    private readonly ILogger<AssetStatusRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssetStatusRepository"/> class.
    /// </summary>
    public AssetStatusRepository(
        WorkforceDbContext context,
        ILogger<AssetStatusRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Create

    /// <inheritdoc />
    public async Task<AssetStatus?> CreateAsync(
        AssetStatus entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _context.AssetStatuses.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Asset Status {AssetStatusId} was created for tenant {TenantId}.",
            entity.Id,
            entity.TenantId);
        return entity;
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(AssetStatus entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var affected = await _context.SaveChangesAsync(cancellationToken);
        if (affected > 0)
        {
            _logger.LogInformation(
                "Asset Status {AssetStatusId} was updated for tenant {TenantId}.",
                entity.Id,
                entity.TenantId);
            return true;
        }

        return false;
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(
        int id,
        long tenantId,
        long employeeId,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdForTenantAsync(id, tenantId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        entity.IsSoftDeleted = true;
        entity.IsActive = false;
        entity.SoftDeletedById = employeeId;
        entity.DeletedDateTime = DateTime.UtcNow;
        entity.UpdatedById = employeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    #endregion

    #region Queries

    /// <inheritdoc />
    public async Task<AssetStatus?> GetByIdForTenantAsync(
        int id,
        long tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _context.AssetStatuses.FirstOrDefaultAsync(
            status => status.Id == id
                && status.TenantId == tenantId
                && status.IsSoftDeleted != true,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssetStatus?> GetByIdAsync(int? id)
    {
        return await _context.AssetStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(status => status.Id == id && status.IsSoftDeleted != true);
    }

    /// <inheritdoc />
    public async Task<PagedResponseDTO<GetStatusResponseDTO>> GetAllAsync(
        long tenantId,
        GetStatusRequestDTO dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
        var pageSize = dto.PageSize <= 0 ? 10 : dto.PageSize;

        IQueryable<AssetStatus> query = _context.AssetStatuses
            .AsNoTracking()
            .Where(status => status.TenantId == tenantId && status.IsSoftDeleted != true);

        if (dto.IsActive)
        {
            query = query.Where(status => status.IsActive == true);
        }

        if (dto.Id > 0)
        {
            query = query.Where(status => status.Id == dto.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(status => status.AddedDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(status => new GetStatusResponseDTO
            {
                Id = status.Id,
                StatusName = status.StatusName,
                Description = status.Description,
                IsActive = status.IsActive ?? false,
                ColorKey = status.ColorKey
            })
            .ToListAsync(cancellationToken);

        return new PagedResponseDTO<GetStatusResponseDTO>(data, totalCount, pageNumber, pageSize);
    }

    #endregion
}
