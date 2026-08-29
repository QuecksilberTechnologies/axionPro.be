using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using axionpro.domain.Entity;

namespace axionpro.persistance.Repositories
{
    public class TenantModuleConfigurationRepository : ITenantModuleConfigurationRepository
    {
        private readonly WorkforceDbContext _context;

        private readonly IMapper _mapper;
        private readonly ILogger<TenantModuleConfigurationRepository> _logger;

        public TenantModuleConfigurationRepository(
            WorkforceDbContext context,
            ILogger<TenantModuleConfigurationRepository> logger,
            IMapper mapper
            )
        {
            _context = context;
            this._logger = logger;
            _mapper = mapper;

        }


       
      
        public async Task<GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledOperation dto)
        {
            try
            {
                long? tenantId = dto.TenantId;

                // Step 1: Get all enabled modules for tenant
                var moduleEntities = await _context.TenantEnabledModules
                    .Where(t => t.TenantId == tenantId && t.IsEnabled && t.IsLeafNode != true)
                    .Include(t => t.Module)
                    .ThenInclude(m => m.ParentModule)
                    .ToListAsync();

                // Step 2: Map to flat list
                var flatList = moduleEntities.Select(t => new ModuleNodedto
                {
                    Id = t.Module.Id,
                    ModuleName = t.Module.ModuleName,
                    ParentModuleId = t.Module.ParentModuleId,
                    IsEnabled = t.IsEnabled
                }).ToList();

                // Step 3: Build hierarchy
                var lookup = flatList.ToDictionary(x => x.Id, x => x);
                List<ModuleNodedto> rootModules = new();

                foreach (var module in flatList)
                {
                    if (module.ParentModuleId.HasValue && lookup.ContainsKey(module.ParentModuleId.Value))
                    {
                        lookup[module.ParentModuleId.Value].Children.Add(module);
                    }
                    else
                    {
                        rootModules.Add(module);
                    }
                }

                return new GetModuleHierarchyResponseDTO
                {
                    TenantId = tenantId,
                    Modules = rootModules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenant enabled modules.");
                return new GetModuleHierarchyResponseDTO
                {
                    TenantId = dto.TenantId,
                    Modules = new List<ModuleNodedto>()
                };
            }
        }

        

        public async Task CreateByDefaultEnabledModulesAsync(
      long tenantId,
      List<TenantEnabledModule> moduleEntities,
      List<TenantEnabledOperation> operationEntities)
        {
            try
            {
                if ((moduleEntities == null || !moduleEntities.Any()) &&
                    (operationEntities == null || !operationEntities.Any()))
                {
                    _logger.LogWarning(
                        "No module or operation entities found to add for TenantId: {TenantId}",
                        tenantId);
                    return;
                }

                if (moduleEntities != null && moduleEntities.Any())
                {
                    foreach (var module in moduleEntities)
                    {
                        module.TenantId = tenantId;
                        module.IsEnabled = true;
                        module.AddedById = tenantId;
                        module.AddedDateTime = DateTime.UtcNow;
                    }

                    await _context.TenantEnabledModules.AddRangeAsync(moduleEntities);

                    _logger.LogInformation(
                        "Tenant enabled modules added to DbContext. Count: {Count}, TenantId: {TenantId}",
                        moduleEntities.Count,
                        tenantId);
                }

                if (operationEntities != null && operationEntities.Any())
                {
                    foreach (var operation in operationEntities)
                    {
                        operation.TenantId = tenantId;
                        operation.IsEnabled = true;
                        operation.IsOperationUsed = true;
                        operation.AddedById = tenantId;
                        operation.AddedDateTime = DateTime.UtcNow;
                    }

                    await _context.TenantEnabledOperations.AddRangeAsync(operationEntities);

                    _logger.LogInformation(
                        "Tenant enabled operations added to DbContext. Count: {Count}, TenantId: {TenantId}",
                        operationEntities.Count,
                        tenantId);
                }

                _logger.LogInformation(
                    "Tenant default module/operation entities prepared successfully for TenantId: {TenantId}",
                    tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while preparing default enabled modules/operations for TenantId: {TenantId}",
                    tenantId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<TenantPlanEntitlementSyncResult?> SynchronizeMissingActivePlanEntitlementsAsync(
            long tenantId,
            long addedById,
            CancellationToken cancellationToken = default)
        {
            var activeSubscription = await _context.TenantSubscriptions
                .AsNoTracking()
                .Where(subscription =>
                    subscription.TenantId == tenantId &&
                    subscription.IsActive &&
                    subscription.SubscriptionPlan.IsActive &&
                    !subscription.SubscriptionPlan.IsSoftDeleted)
                .OrderByDescending(subscription => subscription.SubscriptionStartDate)
                .ThenByDescending(subscription => subscription.Id)
                .Select(subscription => new { subscription.SubscriptionPlanId })
                .FirstOrDefaultAsync(cancellationToken);

            if (activeSubscription is null)
            {
                return null;
            }

            // TenantEnabledModule receives every directly mapped, active Tenant-scope Module.
            // Common and leaf-node restrictions do not apply to this module snapshot.
            var sourceModules = (await _context.PlanModuleMappings
                    .AsNoTracking()
                    .Where(mapping =>
                        mapping.SubscriptionPlanId == activeSubscription.SubscriptionPlanId &&
                        mapping.IsActive == true &&
                        mapping.Module.IsActive &&
                        mapping.Module.ModuleScope == (short)axionpro.application.Constants.AppConstants.TenantModuleScope)
                    .Select(mapping => new TenantPlanEntitlementModuleSyncResponseDTO
                    {
                        ModuleId = mapping.ModuleId,
                        ModuleName = mapping.Module.ModuleName,
                        DisplayName = mapping.Module.DisplayName,
                        ParentModuleId = mapping.Module.ParentModuleId,
                        IsLeafNode = mapping.Module.IsLeafNode
                    })
                    .ToListAsync(cancellationToken))
                .GroupBy(module => module.ModuleId)
                .Select(group => group.First())
                .ToList();

            var sourceModuleIds = sourceModules.Select(module => module.ModuleId).ToList();
            var existingModuleIds = (await _context.TenantEnabledModules
                    .Where(module => module.TenantId == tenantId && sourceModuleIds.Contains(module.ModuleId))
                    .Select(module => module.ModuleId)
                    .ToListAsync(cancellationToken))
                .ToHashSet();

            sourceModules.ForEach(module => module.AlreadyEnabled = existingModuleIds.Contains(module.ModuleId));

            var utcNow = DateTime.UtcNow;
            var modulesToAdd = sourceModules
                .Where(module => !existingModuleIds.Contains(module.ModuleId))
                .Select(module => new TenantEnabledModule
                {
                    TenantId = tenantId,
                    ModuleId = module.ModuleId,
                    ParentModuleId = module.ParentModuleId,
                    IsLeafNode = module.IsLeafNode,
                    IsEnabled = true,
                    AddedById = addedById,
                    AddedDateTime = utcNow
                })
                .ToList();

            // Keep TenantEnabledOperation sourcing unchanged: only non-common Tenant leaf Modules
            // feed the active, non-common ModuleOperationMapping snapshot.
            var operationSourceModuleIds = await _context.PlanModuleMappings
                .AsNoTracking()
                .Where(mapping =>
                    mapping.SubscriptionPlanId == activeSubscription.SubscriptionPlanId &&
                    mapping.IsActive == true &&
                    mapping.Module.IsActive &&
                    !mapping.Module.IsCommonMenu &&
                    mapping.Module.ModuleScope == (short)axionpro.application.Constants.AppConstants.TenantModuleScope &&
                    mapping.Module.IsLeafNode == true)
                .Select(mapping => mapping.ModuleId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var sourceOperations = operationSourceModuleIds.Count == 0
                ? new List<TenantPlanEntitlementOperationSyncResponseDTO>()
                : (await _context.ModuleOperationMappings
                        .AsNoTracking()
                        .Where(mapping =>
                            operationSourceModuleIds.Contains(mapping.ModuleId) &&
                            mapping.IsActive == true &&
                            mapping.IsCommonItem != true &&
                            mapping.Operation.IsActive)
                        .Select(mapping => new TenantPlanEntitlementOperationSyncResponseDTO
                        {
                            ModuleId = mapping.ModuleId,
                            ModuleName = mapping.Module!.ModuleName,
                            OperationId = mapping.OperationId,
                            OperationName = mapping.Operation.OperationName,
                            IsOperational = mapping.IsOperational,
                            PageUrl = mapping.PageUrl
                        })
                        .ToListAsync(cancellationToken))
                    .GroupBy(operation => new { operation.ModuleId, operation.OperationId })
                    .Select(group => group.First())
                    .ToList();

            var sourceOperationKeys = sourceOperations
                .Select(operation => (operation.ModuleId, operation.OperationId))
                .ToHashSet();
            var existingOperationKeys = (await _context.TenantEnabledOperations
                    .Where(operation => operation.TenantId == tenantId)
                    .Select(operation => new { operation.ModuleId, operation.OperationId })
                    .ToListAsync(cancellationToken))
                .Select(operation => (operation.ModuleId, operation.OperationId))
                .Where(sourceOperationKeys.Contains)
                .ToHashSet();

            sourceOperations.ForEach(operation =>
                operation.AlreadyEnabled = existingOperationKeys.Contains((operation.ModuleId, operation.OperationId)));

            var operationsToAdd = sourceOperations
                .Where(operation => !existingOperationKeys.Contains((operation.ModuleId, operation.OperationId)))
                .Select(operation => new TenantEnabledOperation
                {
                    TenantId = tenantId,
                    ModuleId = operation.ModuleId,
                    OperationId = operation.OperationId,
                    IsOperationUsed = operation.IsOperational ?? true,
                    IsEnabled = true,
                    AddedById = addedById,
                    AddedDateTime = utcNow
                })
                .ToList();

            if (modulesToAdd.Count > 0)
            {
                await _context.TenantEnabledModules.AddRangeAsync(modulesToAdd, cancellationToken);
            }

            if (operationsToAdd.Count > 0)
            {
                await _context.TenantEnabledOperations.AddRangeAsync(operationsToAdd, cancellationToken);
            }

            _logger.LogInformation(
                "Prepared additive plan-entitlement sync for TenantId: {TenantId}, PlanId: {PlanId}, AddedModules: {AddedModules}, AddedOperations: {AddedOperations}",
                tenantId,
                activeSubscription.SubscriptionPlanId,
                modulesToAdd.Count,
                operationsToAdd.Count);

            return new TenantPlanEntitlementSyncResult
            {
                SubscriptionPlanId = activeSubscription.SubscriptionPlanId,
                SourceModuleCount = sourceModules.Count,
                AddedModuleCount = modulesToAdd.Count,
                ExistingModuleCount = existingModuleIds.Count,
                SourceOperationCount = sourceOperations.Count,
                AddedOperationCount = operationsToAdd.Count,
                ExistingOperationCount = existingOperationKeys.Count,
                Modules = sourceModules,
                Operations = sourceOperations
            };
        }

        public Task<GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto)
        {
            throw new NotImplementedException();
        }

       
    }

}
