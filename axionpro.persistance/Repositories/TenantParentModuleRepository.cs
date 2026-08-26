// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Queries Tenant-enabled Header Module entitlements and stages focused status cascades.
// ================================================================

using axionpro.application.DTOS.Module.TenantParentModule;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace axionpro.persistance.Repositories;

/// <summary>
/// Provides Tenant Parent Module reads and focused status cascades using <see cref="TenantEnabledModule"/> as the entitlement source.
/// </summary>
public sealed class TenantParentModuleRepository : ITenantParentModuleRepository
{
    #region Fields

    private readonly WorkforceDbContext _context;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="TenantParentModuleRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public TenantParentModuleRepository(WorkforceDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #endregion

    #region Tenant Header Tree

    /// <inheritdoc />
    public async Task<List<TenantParentModuleReadModel>> GetHeaderTreeAsync(
        long tenantId,
        short moduleScope,
        bool? isEnabled,
        CancellationToken cancellationToken)
    {
        var parentQuery = _context.TenantEnabledModules
            .AsNoTracking()
            .Where(entitlement =>
                entitlement.TenantId == tenantId &&
                entitlement.ParentModuleId == null &&
                entitlement.IsLeafNode == false &&
                entitlement.Module.ModuleScope == moduleScope &&
                entitlement.Module.IsModuleDisplayInUI);

        if (isEnabled.HasValue)
        {
            parentQuery = parentQuery.Where(entitlement => entitlement.IsEnabled == isEnabled.Value);
        }

        var parentModules = await parentQuery
            .OrderBy(entitlement => entitlement.Module.ItemPriority)
            .ThenBy(entitlement => entitlement.Module.ModuleName)
            .Select(ProjectReadModel)
            .ToListAsync(cancellationToken);

        if (parentModules.Count == 0)
        {
            return new List<TenantParentModuleReadModel>();
        }

        var parentModuleIds = parentModules.Select(module => module.Id).ToList();
        var childQuery = _context.TenantEnabledModules
            .AsNoTracking()
            .Where(entitlement =>
                entitlement.TenantId == tenantId &&
                entitlement.ParentModuleId.HasValue &&
                parentModuleIds.Contains(entitlement.ParentModuleId.Value) &&
                entitlement.IsLeafNode == false &&
                entitlement.Module.ModuleScope == moduleScope);

        if (isEnabled.HasValue)
        {
            childQuery = childQuery.Where(entitlement => entitlement.IsEnabled == isEnabled.Value);
        }

        var childModules = await childQuery
            .OrderBy(entitlement => entitlement.Module.ItemPriority)
            .ThenBy(entitlement => entitlement.Module.ModuleName)
            .Select(ProjectReadModel)
            .ToListAsync(cancellationToken);
        var childrenByParentModuleId = childModules
            .GroupBy(module => module.ParentModuleId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var parentModule in parentModules)
        {
            parentModule.Children = childrenByParentModuleId.TryGetValue(parentModule.Id, out var children)
                ? children
                : new List<TenantParentModuleReadModel>();
        }

        return parentModules;
    }

    #endregion

    #region Tenant Header Listing

    /// <inheritdoc />
    public async Task<PagedResponseDTO<TenantParentModuleReadModel>> GetPagedMainParentHeadersAsync(
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var normalizedPageNumber = pageNumber > 0 ? pageNumber : 1;
        var normalizedPageSize = pageSize is > 0 and <= 100 ? pageSize : 10;
        var query = _context.TenantEnabledModules
            .AsNoTracking()
            .Where(entitlement =>
                entitlement.ParentModuleId == null &&
                entitlement.IsLeafNode == false);

        if (isActive.HasValue)
        {
            query = query.Where(entitlement => entitlement.IsEnabled == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderBy(entitlement => entitlement.Module.ItemPriority)
            .ThenBy(entitlement => entitlement.Module.ModuleName)
            .ThenBy(entitlement => entitlement.ModuleId)
            .Skip((normalizedPageNumber - 1) * normalizedPageSize)
            .Take(normalizedPageSize)
            .Select(ProjectReadModel)
            .ToListAsync(cancellationToken);

        return new PagedResponseDTO<TenantParentModuleReadModel>(
            data,
            totalCount,
            normalizedPageNumber,
            normalizedPageSize)
        {
            TotalPages = (int)Math.Ceiling(totalCount / (double)normalizedPageSize)
        };
    }

    #endregion

    #region Tenant Header Lookup

    /// <inheritdoc />
    public Task<TenantParentModuleReadModel?> GetHeaderByModuleIdAsync(
        long tenantId,
        int moduleId,
        short moduleScope,
        CancellationToken cancellationToken)
    {
        return _context.TenantEnabledModules
            .AsNoTracking()
            .Where(entitlement =>
                entitlement.TenantId == tenantId &&
                entitlement.ModuleId == moduleId &&
                entitlement.IsLeafNode == false &&
                entitlement.Module.ModuleScope == moduleScope)
            .Select(ProjectReadModel)
            .FirstOrDefaultAsync(cancellationToken);
    }

    #endregion

    #region Tenant Status Cascade

    /// <inheritdoc />
    public async Task<TenantParentModuleReadModel?> StageStatusCascadeAsync(
        long tenantId,
        int moduleId,
        bool isActive,
        long auditActorId,
        DateTime updatedDateTime,
        CancellationToken cancellationToken)
    {
        var target = await _context.TenantEnabledModules
            .Include(entitlement => entitlement.Module)
            .FirstOrDefaultAsync(
                entitlement =>
                    entitlement.TenantId == tenantId &&
                    entitlement.ModuleId == moduleId &&
                    entitlement.IsLeafNode == false,
                cancellationToken);

        if (target is null)
        {
            return null;
        }

        var tenantEntitlements = await _context.TenantEnabledModules
            .Where(entitlement => entitlement.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        var childModuleIdsByParentModuleId = tenantEntitlements
            .Where(entitlement => entitlement.ParentModuleId.HasValue)
            .GroupBy(entitlement => entitlement.ParentModuleId!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.Select(entitlement => entitlement.ModuleId).Distinct().ToList());
        var affectedModuleIds = new HashSet<int> { target.ModuleId };
        var pendingParentModuleIds = new Queue<int>();
        pendingParentModuleIds.Enqueue(target.ModuleId);

        while (pendingParentModuleIds.TryDequeue(out var parentModuleId))
        {
            if (!childModuleIdsByParentModuleId.TryGetValue(parentModuleId, out var childModuleIds))
            {
                continue;
            }

            foreach (var childModuleId in childModuleIds)
            {
                if (affectedModuleIds.Add(childModuleId))
                {
                    pendingParentModuleIds.Enqueue(childModuleId);
                }
            }
        }

        foreach (var entitlement in tenantEntitlements.Where(entitlement => affectedModuleIds.Contains(entitlement.ModuleId)))
        {
            if (entitlement.IsEnabled == isActive)
            {
                continue;
            }

            entitlement.IsEnabled = isActive;
            entitlement.UpdatedById = auditActorId;
            entitlement.UpdatedDateTime = updatedDateTime;
        }

        var affectedModuleIdList = affectedModuleIds.ToList();
        var operations = await _context.TenantEnabledOperations
            .Where(operation =>
                operation.TenantId == tenantId &&
                affectedModuleIdList.Contains(operation.ModuleId))
            .ToListAsync(cancellationToken);

        foreach (var operation in operations)
        {
            var requestedOperationEnabledState = isActive && operation.IsOperationUsed == true;
            if (operation.IsEnabled == requestedOperationEnabledState)
            {
                continue;
            }

            operation.IsEnabled = requestedOperationEnabledState;
            operation.UpdatedById = auditActorId;
            operation.UpdatedDateTime = updatedDateTime;
        }

        return CreateReadModel(target);
    }

    #endregion

    #region Projection

    private static readonly Expression<Func<TenantEnabledModule, TenantParentModuleReadModel>> ProjectReadModel = entitlement =>
        new TenantParentModuleReadModel
        {
            TenantId = entitlement.TenantId,
            Id = entitlement.ModuleId,
            ModuleCode = entitlement.Module.ModuleCode,
            ModuleName = entitlement.Module.ModuleName,
            DisplayName = entitlement.Module.DisplayName,
            UrlPath = entitlement.Module.Urlpath,
            ImageIconWeb = entitlement.Module.ImageIconWeb,
            ImageIconMobile = entitlement.Module.ImageIconMobile,
            ItemPriority = entitlement.Module.ItemPriority,
            ParentModuleId = entitlement.ParentModuleId,
            IsLeafNode = entitlement.IsLeafNode,
            IsEnabled = entitlement.IsEnabled,
            ModuleScope = entitlement.Module.ModuleScope
        };

    private static TenantParentModuleReadModel CreateReadModel(TenantEnabledModule entitlement)
    {
        return new TenantParentModuleReadModel
        {
            TenantId = entitlement.TenantId,
            Id = entitlement.ModuleId,
            ModuleCode = entitlement.Module.ModuleCode,
            ModuleName = entitlement.Module.ModuleName,
            DisplayName = entitlement.Module.DisplayName,
            UrlPath = entitlement.Module.Urlpath,
            ImageIconWeb = entitlement.Module.ImageIconWeb,
            ImageIconMobile = entitlement.Module.ImageIconMobile,
            ItemPriority = entitlement.Module.ItemPriority,
            ParentModuleId = entitlement.ParentModuleId,
            IsLeafNode = entitlement.IsLeafNode,
            IsEnabled = entitlement.IsEnabled,
            ModuleScope = entitlement.Module.ModuleScope
        };
    }

    #endregion
}
