using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;

using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;
        public ModuleRepository(WorkforceDbContext? context, ILogger<ModuleRepository>? logger, IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }
        public async Task<Module?> GetModuleByIdAsync(long moduleId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.IsActive == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetModuleByIdAsync for ID {ModuleId}", moduleId);
                return null;
            }
        }

        public async Task<Module?> GetCommonMenuParentAsync()
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                return await context.Modules
                    .FirstOrDefaultAsync(m => m.IsCommonMenu == true && m.IsModuleDisplayInUi == true && m.IsActive == true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching CommonMenu parent module.", ex);
            }
        }

        public async Task<List<ModuleDTO>> GetCommonMenuTreeAsync(int? parentId)
        {
            try
            {
                List<Module> allModules;

                // ✅ DbContext used only here
                await using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    allModules = await context.Modules
                        .Where(m => m.IsActive && m.IsModuleDisplayInUi)
                        .OrderBy(m => m.Id)
                        .ToListAsync();
                }

                // ✅ Outside context — Safe recursion
                var result = BuildMenuTree(allModules, parentId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetCommonMenuTreeAsync with ParentId={ParentId}", parentId);
                throw;
            }
        }


        private List<ModuleDTO> BuildMenuTree(List<Module> allModules, int? parentId)
        {
            return allModules
                .Where(m => m.ParentModuleId == parentId)
                .OrderBy(m => m.ItemPriority < 0 ? int.MaxValue : m.ItemPriority) // ✅ custom priority sort
                .ThenBy(m => m.ModuleName) // optional fallback
                .Select(m => new ModuleDTO
                {
                    Id = m.Id,
                    ModuleName = m.ModuleName,
                    URLPath = m.URLPath,
                    IsLeafNode = m.IsLeafNode,
                    DisplayName = m.DisplayName,
                    ImageIconMobile = m.ImageIconMobile,
                    ImageIconWeb = m.ImageIconWeb,
                    Children = BuildMenuTree(allModules, m.Id)
                })
                .ToList();
        }

        public async Task<List<GetModuleChildInversResponseDTO>> GetAllOnlyModuleTreeAsync()
        {
            // ✅ Step 1: Load all active modules
            var allModules = await _context.Modules
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.ItemPriority)
                .ToListAsync();

            if (allModules == null || allModules.Count == 0)
                return new List<GetModuleChildInversResponseDTO>();

            // ✅ Step 2: Prepare lookup by parent ID
            var childrenLookup = allModules
                .GroupBy(m => m.ParentModuleId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            // ✅ Step 3: Recursive builder — include only non-leaf nodes
            GetModuleChildInversResponseDTO? BuildTree(Module module)
            {
                // Skip if this module is a leaf node
                if (module.IsLeafNode==true)
                    return null;

                var dto = new GetModuleChildInversResponseDTO
                {
                    Id = module.Id,
                    ModuleName = module.ModuleName,
                    DisplayName = module.DisplayName,
                    SubModuleUrl = module.URLPath,
                    URLPath = module.URLPath,
                    ImageIconWeb = module.ImageIconWeb,
                    ImageIconMobile = module.ImageIconMobile,
                    ItemPriority = module.ItemPriority,
                    IsLeafNode = module.IsLeafNode,
                    Children = new List<GetModuleChildInversResponseDTO>()
                };

                // If this module has children
                if (childrenLookup.TryGetValue(module.Id, out var childModules))
                {
                    foreach (var child in childModules.OrderBy(c => c.ItemPriority))
                    {
                        // Recursive build — only add non-leaf children
                        var childDto = BuildTree(child);
                        if (childDto != null)
                            dto.Children.Add(childDto);
                    }
                }

                // ⚠️ Extra Safety: if a non-leaf node has only leaf children → keep it (because itself is non-leaf)
                return dto;
            }

            // ✅ Step 4: Select root-level non-leaf modules
            var rootModules = allModules
                .Where(m => (m.ParentModuleId == null || m.ParentModuleId == 0) && m.IsLeafNode == false)
                .OrderBy(m => m.ItemPriority)
                .ToList();

           

            // ✅ Step 5: Build final hierarchy
            var result = rootModules .Select(BuildTree).Where(x => x != null)
                .ToList();
            result = result
                      .Where(x => x.Children != null && x.Children.Count > 0)
                         .ToList();
            return result!;

        }

        public async Task<List<GetModuleChildInversResponseDTO>> GetAllModuleTreeAsync()
        {
            // Step 1: Load all active modules
            var allModules = await _context.Modules
                .Where(m => m.IsActive == true)
                .OrderBy(m => m.ItemPriority)
                .ToListAsync();

            if (!allModules.Any())
                return new List<GetModuleChildInversResponseDTO>();

            // Step 2: Prepare lookup (ParentId -> List of children)
            var childrenLookup = allModules
                .GroupBy(m => m.ParentModuleId ?? 0)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Step 3: Recursive builder (include all children, leaf or not)
            GetModuleChildInversResponseDTO BuildTree(Module module)
            {
                var dto = new GetModuleChildInversResponseDTO
                {
                    Id = module.Id,
                    ModuleName = module.ModuleName,
                    DisplayName = module.DisplayName,
                    SubModuleUrl = module.URLPath,
                    URLPath = module.URLPath,
                    ImageIconWeb = module.ImageIconWeb,
                    ImageIconMobile = module.ImageIconMobile,
                    ItemPriority = module.ItemPriority,
                    IsLeafNode = module.IsLeafNode,
                    Children = new List<GetModuleChildInversResponseDTO>()
                };

                if (childrenLookup.TryGetValue(module.Id, out var childModules))
                {
                    foreach (var child in childModules.OrderBy(c => c.ItemPriority))
                    {
                        dto.Children.Add(BuildTree(child)); // include all children
                    }
                }

                return dto;
            }

            // Step 4: Root modules (ParentModuleId null or 0)
            var rootModules = allModules
                .Where(m => m.ParentModuleId == null || m.ParentModuleId == 0)
                .OrderBy(m => m.ItemPriority)
                .ToList();

            // Step 5: Build tree for each root
            return rootModules.Select(BuildTree).ToList();
        }



        #region ParentAdded

        public async Task<List<GetParentModuleResponseDTO>> AddParentModuleAsync(CreateParentModuleRequestDTO module)
        {
            try
            {
                // ✅ Step 1: Validation
                if (module == null)
                {
                    _logger.LogWarning("AddParentModuleAsync called with null module DTO.");
                    return new List<GetParentModuleResponseDTO>(); // empty list
                }

                if (string.IsNullOrWhiteSpace(module.ModuleName))
                {
                    _logger.LogWarning("Module name is missing in AddParentModuleAsync.");
                    return new List<GetParentModuleResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Step 2: Check for duplicate name
                bool isDuplicate = await context.Modules
                    .AnyAsync(m => m.ModuleName == module.ModuleName && m.IsActive);

                if (isDuplicate)
                {
                    _logger.LogWarning("Duplicate module name found: {ModuleName}", module.ModuleName);
                    return new List<GetParentModuleResponseDTO>(); // return empty if duplicate
                }

                // ✅ Step 3: Mapping DTO → Entity
                var moduleEntity = _mapper.Map<Module>(module);

                // ✅ Step 4: Assign default/system values
                moduleEntity.IsLeafNode = false;
                moduleEntity.AddedById = module.EmployeeId;
                moduleEntity.AddedDateTime = DateTime.UtcNow;
                moduleEntity.ParentModuleId = null;
                moduleEntity.IsModuleDisplayInUi = module.IsModuleDisplayInUI;
                moduleEntity.IsCommonMenu = false;
                moduleEntity.IsActive = true;

                // ✅ Step 5: Save to DB
                await context.Modules.AddAsync(moduleEntity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ New parent module '{ModuleName}' added successfully with ID {Id}", moduleEntity.ModuleName, moduleEntity.Id);
                var moduleDTO = _mapper.Map<GetParentModuleRequestDTO>(module);
                moduleDTO.EmployeeId = module.EmployeeId;             
                // ✅ Step 6: Return updated list of parent modules
                return await GetParentModuleAsync(moduleDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding parent module.");
                return new List<GetParentModuleResponseDTO>(); // return empty in case of exception
            }
        }

        public async Task<List<GetParentModuleResponseDTO>> GetParentModuleAsync(GetParentModuleRequestDTO module)
        {
            try
            {
                if (module == null)
                {
                    _logger.LogWarning("GetAllParentModuleAsync called with null module entity.");
                    return new List<GetParentModuleResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                if (context.Modules == null)
                {
                    _logger.LogError("❌ DbSet<Module> is null in context.");
                    return new List<GetParentModuleResponseDTO>();
                }

                // ✅ Fetch parent modules based on flags
                var parentModules = await context.Modules
                    .Where(m => m.IsActive
                             && m.IsLeafNode == false
                             && m.IsModuleDisplayInUi == module.IsModuleDisplayInUi
                             && m.ParentModuleId == null && m.IsCommonMenu == false)
                    .OrderBy(m => m.ModuleName)
                    .ToListAsync();

                // ✅ Return mapped list
                return _mapper.Map<List<GetParentModuleResponseDTO>>(parentModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllParentModuleAsync.");
                return new List<GetParentModuleResponseDTO>();
            }
        }

        public async Task<List<GetModuleChildInversResponseDTO>> GetSubParentModuleAsync(GetParentModuleRequestDTO module)
        {
            try
            {
                if (module == null)
                {
                    _logger.LogWarning("GetAllSubParentModuleAsync called with null module entity.");
                    return new List<GetModuleChildInversResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                if (context.Modules == null)
                {
                    _logger.LogError("❌ DbSet<Module> is null in context.");
                    return new List<GetModuleChildInversResponseDTO>();
                }

                // ✅ Fetch parent modules based on flags
                var parentModules = await context.Modules
                    .Where(m => m.IsActive
                             && m.IsLeafNode == false
                             && m.IsModuleDisplayInUi == module.IsModuleDisplayInUi
                             && m.ParentModuleId == null && m.IsCommonMenu == false)
                    .OrderBy(m => m.ModuleName)
                    .ToListAsync();

                // ✅ Return mapped list
                return _mapper.Map<List<GetModuleChildInversResponseDTO>>(parentModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllParentModuleAsync.");
                return new List<GetModuleChildInversResponseDTO>();
            }
        }


        #endregion
        #region SubModuleAdded



        public async Task<List<GetSubModuleResponseDTO>> GetAllSubModuleAsync(Module module)
        {
            try
            {
                if (module == null)
                {
                    _logger.LogWarning("GetAllSubModuleAsync called with null module entity.");
                    return new List<GetSubModuleResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                if (context.Modules == null)
                {
                    _logger.LogError("❌ DbSet<Module> is null in context.");
                    return new List<GetSubModuleResponseDTO>();
                }

                // ✅ Fetch SubModules under the given ParentModule
                var subModules = await context.Modules
                    .Where(m => m.IsActive
                             && m.IsLeafNode == true
                             && m.IsModuleDisplayInUi == module.IsModuleDisplayInUi
                             && m.IsCommonMenu == module.IsCommonMenu
                             && m.ParentModuleId == module.ParentModuleId)
                    .OrderBy(m => m.ItemPriority)
                    .ThenBy(m => m.ModuleName)
                    .ToListAsync();

                // ✅ Return mapped list
                return _mapper.Map<List<GetSubModuleResponseDTO>>(subModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllSubModuleAsync.");
                return new List<GetSubModuleResponseDTO>();
            }
        }

        #endregion

        //public async Task<Module> AddSubModuleAsync(Module module)
        //{
        //    return await AddParentModuleAsync(module); // Same logic as AddModule
        //}

        public async Task<bool> UpdateModuleAsync(Module module)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var existing = await context.Modules.FindAsync(module.Id);
                if (existing == null) return false;

                existing.ModuleName = module.ModuleName;
                existing.ParentModuleId = module.ParentModuleId;
                existing.ImageIconWeb = module.ImageIconWeb;
                existing.ImageIconMobile = module.ImageIconMobile;
                existing.IsActive = module.IsActive;
                existing.UpdatedById = module.UpdatedById;
                existing.UpdatedDateTime = DateTime.Now;
                existing.Remark = module.Remark;

                context.Modules.Update(existing);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating module with ID {ModuleId}", module.Id);
                return false;
            }
        }

        public async Task<bool> DeleteModuleAsync(long moduleId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var module = await context.Modules.FindAsync(moduleId);
                if (module == null) return false;

                module.IsActive = false;
                module.UpdatedDateTime = DateTime.Now;

                context.Modules.Update(module);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting module with ID {ModuleId}", moduleId);
                return false;
            }
        }
        public async Task<List<GetModuleChildInversResponseDTO>> AddSubModuleAsync(CreateSubModuleRequestDTO module)
        {
            try
            {
                // ✅ Step 1: Validation
                if (module == null)
                {
                    _logger.LogWarning("AddSubModuleAsync called with null module DTO.");
                    return new List<GetModuleChildInversResponseDTO>(); // empty list
                }

                if (string.IsNullOrWhiteSpace(module.ModuleName))
                {
                    _logger.LogWarning("Module name is missing in AddSubModuleAsync.");
                    return new List<GetModuleChildInversResponseDTO>();
                }

                await using var context = await _contextFactory.CreateDbContextAsync();

                // ✅ Step 2: Check for duplicate module under same parent
                bool isDuplicate = await context.Modules
                    .AnyAsync(m => m.TenantId == module.TenantId &&
                        m.ModuleName == module.ModuleName &&
                        m.ParentModuleId == module.ParentModuleId &&
                        m.IsActive);

                if (isDuplicate)
                {
                    _logger.LogWarning("Duplicate SubModule name found under Parent ID: {ParentId}, Name: {ModuleName}",
                        module.ParentModuleId, module.ModuleName);
                    return new List<GetModuleChildInversResponseDTO>(); // return empty if duplicate
                }

                // ✅ Step 3: Mapping DTO → Entity
                var moduleEntity = _mapper.Map<Module>(module);

                // ✅ Step 4: Assign SubModule-specific values
                moduleEntity.IsLeafNode = module.IsLeafNode;
                moduleEntity.AddedById = module.EmployeeId;
                moduleEntity.AddedDateTime = DateTime.UtcNow;
                moduleEntity.IsModuleDisplayInUi = module.IsModuleDisplayInUi;
                moduleEntity.IsCommonMenu = module.IsCommonMenu;
                moduleEntity.IsActive = module.IsActive;

                // ✅ Step 5: Save to DB
                var entity = await context.Modules.AddAsync(moduleEntity);
                await context.SaveChangesAsync();

                _logger.LogInformation("✅ New SubModule '{ModuleName}' added successfully under Parent ID {ParentId}",
                    moduleEntity.ModuleName, moduleEntity.ParentModuleId);
                int moduleId = entity.Entity.Id;

                // ✅ Call and wrap result into list
                var hierarchy = await GetModuleHierarchyByIdAsync(moduleId);

                return hierarchy != null
                    ? new List<GetModuleChildInversResponseDTO> { hierarchy }
                    : new List<GetModuleChildInversResponseDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while adding SubModule.");
                return new List<GetModuleChildInversResponseDTO>(); // return empty in case of exception
            }
        }
        public async Task<GetModuleChildInversResponseDTO?> GetModuleHierarchyByIdAsync(int moduleId)
        {
            try
            {
                // ✅ Step 1: Load all active modules
                var allModules = await _context.Modules
                    .Where(m => m.IsActive == true)
                    .ToListAsync();

                if (allModules == null || allModules.Count == 0)
                {
                    _logger?.LogWarning("⚠️ No active modules found.");
                    return null;
                }

                // ✅ Step 2: Build lookup dictionaries for traversal
                var childrenLookup = allModules
                    .GroupBy(m => m.ParentModuleId ?? 0)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var parentLookup = allModules.ToDictionary(m => m.Id, m => m.ParentModuleId);

                // ✅ Step 3: Recursive builder for children
                GetModuleChildInversResponseDTO BuildChildTree(Module module)
                {
                    var dto = new GetModuleChildInversResponseDTO
                    {
                        Id = module.Id,
                        ModuleName = module.ModuleName,
                        DisplayName = module.DisplayName,
                        SubModuleUrl = module.URLPath,
                        URLPath = module.URLPath,
                        ImageIconWeb = module.ImageIconWeb,
                        ImageIconMobile = module.ImageIconMobile,
                        ItemPriority = module.ItemPriority,
                        IsLeafNode = module.IsLeafNode,
                        Children = new List<GetModuleChildInversResponseDTO>()
                    };

                    if (childrenLookup.TryGetValue(module.Id, out var childModules))
                    {
                        foreach (var child in childModules.OrderBy(c => c.ItemPriority))
                        {
                            dto.Children.Add(BuildChildTree(child));
                        }
                    }

                    return dto;
                }

                // ✅ Step 4: Find the requested module
                var targetModule = allModules.FirstOrDefault(m => m.Id == moduleId);
                if (targetModule == null)
                {
                    _logger?.LogWarning("⚠️ Module with Id {Id} not found.", moduleId);
                    return null;
                }

                // ✅ Step 5: Build child tree for the target node
                var hierarchy = BuildChildTree(targetModule);

                // ✅ Step 6: Build upward chain (parents)
                var parentList = new List<GetModuleChildInversResponseDTO>();
                long? currentParentId = targetModule.ParentModuleId;

                while (currentParentId.HasValue && currentParentId != 0)
                {
                    var parentModule = allModules.FirstOrDefault(m => m.Id == currentParentId);
                    if (parentModule == null) break;

                    var parentDto = new GetModuleChildInversResponseDTO
                    {
                        Id = parentModule.Id,
                        ModuleName = parentModule.ModuleName,
                        DisplayName = parentModule.DisplayName,
                        URLPath = parentModule.URLPath,
                        ItemPriority = parentModule.ItemPriority,
                        Children = new List<GetModuleChildInversResponseDTO> { hierarchy }
                    };

                    hierarchy = parentDto; // move up
                    currentParentId = parentModule.ParentModuleId;
                }

                _logger?.LogInformation("✅ Hierarchy for Module Id {Id} fetched successfully.", moduleId);
                return hierarchy;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "❌ Error occurred while fetching hierarchy for module Id {Id}.", moduleId);
                return null;
            }
        }

        public Task<List<ModuleDTO>> GetAllActiveModulesAsync(List<ModuleDTO> modules)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetCommonModuleResponseDTO>> AddCommonModuleAsync(CreateCommonModuleRequestDTO Dto)
        {
            throw new NotImplementedException();
        }

      
        public Task<List<GetParentModuleResponseDTO>> GetSubParentModuleAsync(GetSubParentModulRequestDTO Dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetCommonModuleResponseDTO>> GetCommonModuleAsync(GetCommonModuleRequestDTO Dto)
        {
            throw new NotImplementedException();
        }

        public Task<Module> AddSubModuleAsync(Module module)
        {
            throw new NotImplementedException();
        }

       
        
    }
}
