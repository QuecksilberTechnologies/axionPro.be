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

        public Task<GetModuleHierarchyResponseDTO> GetAllTenantEnabledModulesAsync(TenantEnabledModuleRequestDTO dto)
        {
            throw new NotImplementedException();
        }

       
    }

}
