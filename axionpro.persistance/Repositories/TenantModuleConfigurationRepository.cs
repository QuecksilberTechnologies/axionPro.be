using AutoMapper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class TenantModuleConfigurationRepository : ITenantModuleConfigurationRepository
    {
        private readonly WorkforceDbContext _context;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantModuleConfigurationRepository> _logger;

        public TenantModuleConfigurationRepository(
            WorkforceDbContext context,
            ILogger<TenantModuleConfigurationRepository> logger,
            IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            this._logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }


       
        public async Task<bool> UpdateTenantModuleAndItsOperationsAsync(TenantModuleOperationsUpdateRequestDTO request)
        {
            try
            {
                foreach (var module in request.Modules)
                {
                    // ✅ Step 1: Update TenantEnabledModule table
                    await _context.Database.ExecuteSqlRawAsync(
                        @"UPDATE [AxionPro].[TenantEnabledModule]
                        SET IsEnabled = {0}, UpdatedDateTime = GETUTCDATE()
                        WHERE TenantId = {1} AND ModuleId = {2}",
                        module.IsEnabled, request.TenantId, module.ModuleId
                    );

                    if (module.IsEnabled == false)
                    {
                        // ✅ Step 2: If module is disabled, disable all its operations
                        await _context.Database.ExecuteSqlRawAsync(
                            @"UPDATE [AxionPro].[TenantEnabledOperation]
                              SET IsEnabled = 0, IsOperationUsed = 0, UpdatedDateTime = GETUTCDATE()
                              WHERE TenantId = {0} AND ModuleId = {1}",
                            request.TenantId, module.ModuleId
                        );
                    }
                    else
                    {
                        // ✅ Step 3: If module is enabled, update only selected operations
                        foreach (var operation in module.Operations)
                        {
                            await _context.Database.ExecuteSqlRawAsync(
                                @"UPDATE [AxionPro].[TenantEnabledOperation]
                                   SET IsEnabled = {0}, IsOperationUsed = {1}, UpdatedDateTime = GETUTCDATE()
                                   WHERE TenantId = {2} AND ModuleId = {3} AND OperationId = {4}",
                                   operation.IsEnabled, operation.IsOperationUsed, request.TenantId, module.ModuleId, operation.OperationId
                            );
                        }
                    }
                }

            

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update failed while updating module & operations");
                return false;
            }
        }

        //public async Task<List<TenantEnabledModule>> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(long? tenantId)
        //{
        //    try
        //    {
        //        var modules = await _context.TenantEnabledModules
        //            .Where(m => m.TenantId == tenantId )
        //            .Include(m => m.Module)
        //                .ThenInclude(mod => mod.ModuleOperationMappings
        //                    .Where(mop => mop.IsActive == true)) // Only active mappings
        //            .ToListAsync();

        //        return modules;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching enabled modules and operations for TenantId: {TenantId}", tenantId);
        //        return new List<TenantEnabledModule>(); // Return empty list on failure
        //    }
        //}



        //yeh function sirf enabled module or operation laata hai , login mei bhi used
        public async Task<List<TenantEnabledModule>> GetAllTenantEnabledModulesWithOperationsAsync(long? tenantId)
        {
            try
            {
                var modules = await _context.TenantEnabledModules
                    .Where(m => m.TenantId == tenantId && m.IsEnabled)
                    .Include(m => m.Module)
                        .ThenInclude(mod => mod.ModuleOperationMappings
                            .Where(mop => mop.IsActive ==true)) // ✅ filtered include EF Core 5+
                    .ToListAsync();

                return modules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching enabled modules and operations for TenantId: {TenantId}", tenantId);
                return new List<TenantEnabledModule>();
            }
        }



        public async Task<TenantEnabledModuleOperationsResponseDTO> GetAllEnabledTrueModulesWithOperationsByTenantIdAsync(TenantEnabledModuleRequestDTO tenantEnabledModuleOperationsRequestDTO)
        {
            try
            {
                var tenantId = tenantEnabledModuleOperationsRequestDTO.TenantId;

                // 🟢 Step 1: Get all enabled modules
                var tenantModules = await _context.TenantEnabledModules
                    .Where(t => t.TenantId == tenantId )
                    .Include(t => t.Module)
                    .ThenInclude(m => m.ParentModule)
                    .ToListAsync();

                // 🟢 Step 2: Get all enabled operations
                var tenantOperations = await _context.TenantEnabledOperations
                    .Where(op => op.TenantId == tenantId && op.IsEnabled)
                    .Include(op => op.Operation)
                    .ToListAsync();

                // 🧠 Step 3: Map manually in memory
                var modules = tenantModules.Select(t => new EnabledModuleActiveDTO
                {
                    Id = t.Module.Id,
                    ModuleName = t.Module.ModuleName,
                    ParentModuleId = t.Module.ParentModuleId,
                    ParentModuleName = t.Module.ParentModule != null ? t.Module.ParentModule.ModuleName : "",
                    IsEnabled = t.IsEnabled,
                    Operations = tenantOperations
                        .Where(op => op.ModuleId == t.ModuleId)
                        .Select(op => new EnabledOperationActiveDTO
                        {
                            Id = op.OperationId,
                            OperationName = op.Operation?.OperationName ?? "",
                            IsEnabled = op.IsEnabled
                        }).ToList()
                }).ToList();

                return new TenantEnabledModuleOperationsResponseDTO
                {
                    TenantId = tenantId,
                    Modules = modules
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching enabled modules and operations for tenant.");
                return new TenantEnabledModuleOperationsResponseDTO
                {
                    TenantId = tenantEnabledModuleOperationsRequestDTO.TenantId,
                    Modules = null
                };
            }
        }
        public async Task<GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto)
        {
            try
            {
                long? tenantId = dto.TenantId;

                // Step 1: Get all enabled modules for tenant
                var moduleEntities = await _context.TenantEnabledModules
                    .Where(t => t.TenantId == tenantId && t.IsEnabled && t.IsLeafNode!=true)
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

        public async Task<TenantEnabledModuleOperationsResponseDTO> GetEnabledModuleWithOperation(TenantEnabledModuleRequestDTO tenantEnabledModuleOperationsRequestDTO)
        {
            try
            {
                var tenantId = tenantEnabledModuleOperationsRequestDTO.TenantId;
                TenantEnabledModuleOperationsResponseDTO response = new();

                // Step 1: Get all enabled modules for this tenant
                var allModules = await _context.TenantEnabledModules
                    .Where(x => x.TenantId == tenantId && x.IsEnabled)
                    .ToListAsync();

                if (allModules == null || !allModules.Any())
                    return response;

                // Step 2: Get module with smallest ModuleId
                var rootModule = allModules
                    .OrderBy(x => x.ModuleId)
                    .FirstOrDefault(x => x.ParentModuleId == null);

                if (rootModule == null)
                    return response;

                // Step 3: Traverse hierarchy to find all child modules
                List<int> moduleChain = new();
                var currentModule = rootModule;

                while (currentModule != null)
                {
                    moduleChain.Add(currentModule.ModuleId);

                    // Find direct child of current module
                    currentModule = allModules.FirstOrDefault(x => x.ParentModuleId == currentModule.ModuleId);
                }

                // Step 4: Get operations for last (leaf) module in chain
                int lastModuleId = moduleChain.Last();

                var operationIds = await _context.TenantEnabledOperations
                    .Where(x => x.TenantId == tenantId && x.ModuleId == lastModuleId)
                    .Select(x => x.OperationId)
                    .ToListAsync();

                // Optional: fetch Operation Names from Operation table if needed
                var operationNames = await _context.Operations
                    .Where(o => operationIds.Contains(o.Id))
                    .Select(o => o.OperationName)
                    .ToListAsync();

                // Step 5: Prepare response
              

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching enabled module operations");
                throw;
            }
        }

        public async Task CreateByDefaultEnabledModulesAsync(long tenantId,
            List<TenantEnabledModule> moduleEntities, List<TenantEnabledOperation> operationEntities)
        {
            try
            {
                operationEntities.ForEach(op => op.TenantId = tenantId);

                // Null/empty check
                if ((moduleEntities == null || !moduleEntities.Any()) &&
                    (operationEntities == null || !operationEntities.Any()))
                {
                    _logger.LogWarning("No module or operation entities to insert for tenantId: {TenantId}", tenantId);
                    return;
                }

                // Insert enabled modules
                if (moduleEntities != null && moduleEntities.Any())
                {

                    await _context.TenantEnabledModules.AddRangeAsync(moduleEntities);
                    _logger.LogInformation("Inserted {Count} enabled modules for tenantId: {TenantId}", moduleEntities.Count, tenantId);
                }

                // Insert enabled operations
                if (operationEntities != null && operationEntities.Any())
                {
                    operationEntities.ForEach(op =>
                    {
                        op.TenantId = tenantId;
                        op.IsEnabled = true;
                        op.AddedById = tenantId;
                        op.IsOperationUsed = true;
                        op.AddedDateTime = DateTime.Now;
                    });
                    await _context.TenantEnabledOperations.AddRangeAsync(operationEntities);
                    _logger.LogInformation("Inserted {Count} enabled operations for tenantId: {TenantId}", operationEntities.Count, tenantId);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Tenant default modules and operations saved successfully for tenantId: {TenantId}", tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while inserting default enabled modules/operations for tenantId: {TenantId}", tenantId);
                throw; // Bubble up the error for global handling
            }

        }

    }

}
