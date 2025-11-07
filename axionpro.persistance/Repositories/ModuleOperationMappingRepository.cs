using AutoMapper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.ModuleOperation;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.RoleModulePermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class ModuleOperationMappingRepository : IModuleOperationMappingRepository
    {

        private readonly WorkforceDbContext _context;
        private readonly ILogger<ModuleOperationMappingRepository> _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;

        public ModuleOperationMappingRepository(WorkforceDbContext context, ILogger<ModuleOperationMappingRepository> logger, IMapper mapper,
            IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }
        

        public async Task<ModuleOperationMapping> UpdateModuleOperationMappingsAsync(ModuleOperationMapping mom)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                if (mom == null || mom.Id <= 0)
                {
                    _logger.LogWarning("Invalid ModuleOperationMapping object passed to update.");
                    return null;
                }

                var existing = await _context.ModuleOperationMappings
                    .FirstOrDefaultAsync(x => x.Id == mom.Id && x.ModuleId == mom.ModuleId);

                if (existing == null)
                {
                    _logger.LogWarning("No existing ModuleOperationMapping found for Id {Id} and ModuleId {ModuleId}.", mom.Id, mom.ModuleId);
                    return null;
                }

                // 🔁 Update properties
               
                existing.PageUrl = mom.PageUrl;
                existing.IconUrl = mom.IconUrl;
                existing.IsCommonItem = mom.IsCommonItem;
                existing.IsOperational = mom.IsOperational;
                existing.Priority = mom.Priority;
                existing.Remark = mom.Remark;
                existing.IsActive = mom.IsActive;
                existing.UpdatedById = mom.UpdatedById;
                existing.UpdatedDateTime = mom.UpdatedDateTime ?? DateTime.Now;

                await _context.SaveChangesAsync();

                // ✅ Prepare and return response DTO
                

                _logger.LogInformation("ModuleOperationMapping updated successfully for Id {Id}", existing.Id);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ModuleOperationMapping for Id {Id}", mom.Id);
                return null;
            }
        }

        public async Task<GetModuleOperationMappingResponseDTO> SaveModuleOperationMappingsAsync(GetModuleOperationMappingRequestDTO dto)

        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                if (dto == null || dto.Operation == null || !dto.Operation.Any(o => o.IsSelected))
                    throw new ArgumentException("At least one operation must be selected.");

                var selectedOperationIds = dto.Operation
                    .Where(o => o.IsSelected)
                    .Select(o => o.Id)
                    .ToList();

                var mappings = selectedOperationIds.Select(opId => new ModuleOperationMapping
                {
                    ModuleId = dto.ModuleId,
                    OperationId = opId,
                    DataViewStructureId = dto.DataViewStructureId,
                    PageTypeId = dto.PageTypeId,
                    PageUrl = dto.PageURL,
                    IconUrl = dto.IconURL,
                    IsCommonItem = dto.IsCommonItem,
                    IsOperational = dto.IsOperational,
                    Priority = dto.Priority,
                    Remark = dto.Remark,
                    IsActive = dto.IsActive,
                    AddedById = dto.AddedById,
                    AddedDateTime = DateTime.UtcNow
                }).ToList();
            
                await _context.ModuleOperationMappings.AddRangeAsync(mappings);
                int result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new GetModuleOperationMappingResponseDTO
                    {
                        ModuleId = dto.ModuleId,
                        OperationIds = selectedOperationIds,
                        Message = "Module operation mappings saved successfully."
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving ModuleOperationMapping.");
                return null;
            }
        }


        public async Task<ModuleOperationMappingByProductOwnerResponseDTO?> GetModuleOperationMappingsByIdAsync(int id, int moduleId)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                var mapping = await _context.ModuleOperationMappings
                    .FirstOrDefaultAsync(x => x.Id == id && x.ModuleId == moduleId);

                if (mapping == null)
                {
                    _logger.LogWarning("No ModuleOperationMapping found for Id {Id} and ModuleId {ModuleId}", id, moduleId);
                    return null;
                }

                var responseDto = new ModuleOperationMappingByProductOwnerResponseDTO
                {
                    Id = mapping.Id,
                    ModuleId = mapping.ModuleId,
                    OperationIds = new List<int> { mapping.OperationId },
                     
                    PageURL = mapping.PageUrl,
                    IconURL = mapping.IconUrl,
                    IsCommonItem = mapping.IsCommonItem ?? false,
                    IsOperational = mapping.IsOperational ?? false,
                    Priority = mapping.Priority ?? 0,
                    Remark = mapping.Remark,
                    IsActive = mapping.IsActive,
                    AddedById = mapping.AddedById,
                    AddedDateTime = mapping.AddedDateTime,
                    UpdatedById = mapping.UpdatedById,
                    UpdatedDateTime = mapping.UpdatedDateTime
                };

                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching ModuleOperationMapping for Id {Id} and ModuleId {ModuleId}", id, moduleId);
                return null;
            }
        }


        public async Task<List<ModuleOperationMapping>> GetModuleOperationMappings(List<Module> modules)
        {
            try
            {
               // await using var _context = await _contextFactory.CreateDbContextAsync();

                if (modules == null || !modules.Any())
                {
                    return new List<ModuleOperationMapping>();
                }

                var moduleIds = modules.Select(m => m.Id).ToList();

                var mappings = await _context.ModuleOperationMappings.Where(m => moduleIds.Contains(m.ModuleId) && m.IsActive == true).Include(m => m.Operation).ToListAsync();


                return mappings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetModuleOperationMappings");
                return new List<ModuleOperationMapping>();
            }
        }

        public async Task<List<GetModuleOperationRolePermissionsResponseDTO>> GetTenantModulesOperationRolePermissionResponses(GetTenantModuleOperationRolePermissionsRequestDTO dto)
        {
            try
            {
                // 🧱 Step 0: Create DbContext
             //   await using var context = await _contextFactory.CreateDbContextAsync();

                // ---------------------
                // Step 1: Plan modules
                // ---------------------
                // PlanModuleMapping se plan ke active modules nikaal rahe hain
                var planModuleIds = await _context.PlanModuleMappings
                    .Where(p => p.SubscriptionPlanId == (dto.SubscriptionPlanId ?? 0) && p.IsActive == true)
                    .Select(p => p.ModuleId)
                    .Distinct()
                    .ToListAsync();

                if (planModuleIds == null || !planModuleIds.Any())
                {
                    _logger?.LogWarning("No active modules found for SubscriptionPlanId: {SubPlanId}", dto.SubscriptionPlanId);
                    return null;
                }

                // ---------------------------------------
                // Step 2: Tenant enabled modules
                // ---------------------------------------
                var tenantEnabledModuleIds = await _context.TenantEnabledModules
                    .Where(t => t.TenantId == dto.TenantId && t.IsEnabled == true)
                    .Select(t => t.ModuleId)
                    .Distinct()
                    .ToListAsync();

                if (tenantEnabledModuleIds == null || !tenantEnabledModuleIds.Any())
                {
                    _logger?.LogWarning("No enabled modules found for TenantId: {TenantId}", dto.TenantId);
                    return null;
                }

                // ---------------------------------------
                // Step 3: Validation (plan modules match)
                // ---------------------------------------
                bool isMatched = planModuleIds.All(moduleId => tenantEnabledModuleIds.Contains(moduleId));
                if (!isMatched)
                {
                    _logger?.LogWarning("Plan modules not fully enabled for TenantId: {TenantId}", dto.TenantId);
                    return null;
                }

                // -------------------------------------------------------
                // Step 4: Special filtering (ModuleId & ParentModuleId both provided)
                // -------------------------------------------------------
                if (dto.ModuleId > 0 && dto.ParentModuleId > 0)
                {
                    // 🔹 Step 4.1: dono me se badi value nikaal lo
                    int selectedModuleId = Math.Max(dto.ModuleId ?? 0, dto.ParentModuleId ?? 0);

                    // 🔹 Step 4.2: check karo kya ye module tenant ke enabled list me hai
                    bool isModuleEnabled = tenantEnabledModuleIds.Contains(selectedModuleId);
                    if (!isModuleEnabled)
                    {
                        _logger?.LogWarning("Selected module {ModuleId} is not enabled for TenantId {TenantId}", selectedModuleId, dto.TenantId);
                        return null;
                    }

                    // 🔹 Step 4.3: ab Module table se uske saare child modules (Nth level tak) nikaalo
                    // Recursive fetching karenge — manually loop se karenge
                    var allModules = await _context.Modules
                        .Select(m => new { m.Id, m.ModuleName, m.ParentModuleId, m.IsLeafNode })
                        .ToListAsync();

                    // recursion ke bina iterative approach se Nth level children nikaal rahe hain
                    List<int> childModuleIds = new() { selectedModuleId };
                    bool newAdded;

                    do
                    {
                        var currentCount = childModuleIds.Count;
                        var nextLevel = allModules
                            .Where(m => m.ParentModuleId.HasValue && childModuleIds.Contains(m.ParentModuleId.Value))
                            .Select(m => m.Id)
                            .ToList();

                        childModuleIds.AddRange(nextLevel.Except(childModuleIds));
                        newAdded = childModuleIds.Count > currentCount;
                    }
                    while (newAdded);

                    // 🔹 Step 4.4: sirf leaf node wale modules filter kar lo
                    var leafModules = allModules
                        .Where(m => childModuleIds.Contains(m.Id) && m.IsLeafNode == true)
                        .ToList();

                    if (!leafModules.Any())
                    {
                        _logger?.LogInformation("No leaf modules found under ModuleId {ModuleId}", selectedModuleId);
                        return new List<GetModuleOperationRolePermissionsResponseDTO>();
                    }

                    // 🔹 Step 4.5: ab un leaf modules ke liye operations nikaalo
                    var operationsData = await (
                        from teo in _context.TenantEnabledOperations
                        join op in _context.Operations on teo.OperationId equals op.Id
                        where teo.TenantId == dto.TenantId
                              && leafModules.Select(x => x.Id).Contains(teo.ModuleId)
                              && teo.IsEnabled == true
                              && op.IsActive
                        select new
                        {
                            ModuleId = teo.ModuleId,
                            OperationId = op.Id,
                            op.OperationName,
                            teo.IsOperationUsed
                        }
                    ).ToListAsync();

                    if (!operationsData.Any())
                    {
                        _logger?.LogInformation("No operations found for leaf modules for TenantId: {TenantId}", dto.TenantId);
                        return new List<GetModuleOperationRolePermissionsResponseDTO>();
                    }

                    // 🔹 Step 4.6: DTO me map kar lo
                    // 🔹 Step 4.6: Ab RoleModuleAndPermission table ke saath left join karenge
                    var rolePermissions = await (
                        from rp in _context.RoleModuleAndPermissions
                        where rp.RoleId == dto.RoleId                             
                              && rp.IsActive == true
                        select new
                        {
                            rp.RoleId,
                            rp.ModuleId,
                            rp.OperationId,
                            rp.HasAccess
                        }
                    ).ToListAsync();

                    // 🔹 Step 4.7: Ab operationsData ko left join karte hain RolePermission ke saath
                    var response = leafModules
                        .GroupJoin(operationsData,
                            m => m.Id,
                            o => o.ModuleId,
                            (m, ops) => new GetModuleOperationRolePermissionsResponseDTO
                            {
                                ModuleId = m.Id,
                                ModuleName = m.ModuleName,
                                IsLeafNode = true,
                                IsEnabled = true,
                                Level = 0,
                                BreadCrum = "",
                                Operations = ops.Select(op =>
                                {
                                    // matching permission search
                                    var permission = rolePermissions.FirstOrDefault(p =>
                                        p.ModuleId == m.Id &&
                                        p.OperationId == op.OperationId);

                                    return new OperationDTO_Config
                                    {
                                        OperationId = op.OperationId,
                                        OperationName = op.OperationName,
                                        IsOperationUsed = op.IsOperationUsed ?? false,
                                        HasAccess = permission?.HasAccess ?? false  // ← left join ka core point
                                    };
                                }).ToList()
                            })
                        .ToList();

                  
                    _logger?.LogInformation("Fetched {Count} leaf modules and operations for TenantId: {TenantId}", response.Count, dto.TenantId);
                    return response;
                }

                // -------------------------------------------------------
                // Step 5: Default path (if special condition not met)
                // -------------------------------------------------------
                // Yahan normal tenant-operation-module data la rahe hain
                var operationDataDefault = await (
                    from teo in _context.TenantEnabledOperations
                    join op in _context.Operations on teo.OperationId equals op.Id
                    join m in _context.Modules on teo.ModuleId equals m.Id
                    where teo.TenantId == dto.TenantId
                          && teo.IsEnabled == true
                          && op.IsActive
                    select new
                    {
                        m.Id,
                        m.ModuleName,
                        m.ParentModuleId,
                        m.IsLeafNode,
                        OperationId = op.Id,
                        op.OperationName,
                        teo.IsOperationUsed
                    }
                ).ToListAsync();

                if (!operationDataDefault.Any())
                {
                    _logger?.LogInformation("No operations found for TenantId: {TenantId}", dto.TenantId);
                    return new List<GetModuleOperationRolePermissionsResponseDTO>();
                }

                // ✅ Step 6: Group & map to DTO
                var modulesDefault = operationDataDefault
                    .GroupBy(x => new { x.Id, x.ModuleName, x.ParentModuleId, x.IsLeafNode })
                    .Select(g => new GetModuleOperationRolePermissionsResponseDTO
                    {
                        ModuleId = g.Key.Id,
                        ModuleName = g.Key.ModuleName,
                        IsLeafNode = g.Key.IsLeafNode,
                        IsEnabled = true,
                        Level = 0,
                        BreadCrum = "",
                        Operations = g.Select(op => new OperationDTO_Config
                        {
                            OperationId = op.OperationId,
                            OperationName = op.OperationName,
                            IsOperationUsed = op.IsOperationUsed ?? false
                        }).ToList()
                    })
                    .ToList();

                // ✅ Step 7: Hierarchy build karna
                var moduleDict = modulesDefault.ToDictionary(m => m.ModuleId);
                List<GetModuleOperationRolePermissionsResponseDTO> rootModules = new();

                foreach (var module in modulesDefault)
                {
                    var parentId = operationDataDefault.FirstOrDefault(x => x.Id == module.ModuleId)?.ParentModuleId;
                    if (parentId.HasValue && moduleDict.ContainsKey(parentId.Value))
                        moduleDict[parentId.Value].Children.Add(module);
                    else
                        rootModules.Add(module);
                }

                _logger?.LogInformation("Tenant module-operation-role data successfully fetched for TenantId: {TenantId}", dto.TenantId);
                return rootModules;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while fetching Tenant module-operation-role permissions for TenantId: {TenantId}", dto.TenantId);
                return null;
            }
        }





    }

}
