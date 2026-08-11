using AutoMapper;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;

using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using System.Linq.Expressions;


// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Provides persistence operations for application modules.
// ============================================================================

namespace axionpro.persistance.Repositories
{
    /// <summary>
    /// Provides persistence operations for application modules.
    /// </summary>
    public class ModuleRepository : IModuleRepository
    {
        private readonly WorkforceDbContext? _context;
        private readonly ILogger? _logger;
       
        private readonly IMapper _mapper;
        public ModuleRepository(WorkforceDbContext? context, ILogger<ModuleRepository>? logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            
        }

        #region Host Module Queries

        /// <summary>
        /// Retrieves Host-scope modules, optionally filtered by their active state.
        /// </summary>
        /// <param name="isActive">When supplied, limits results to modules with the specified active state.</param>
        /// <returns>A projected list of Host-scope module response models.</returns>
        public async Task<List<GetHostModuleResponseDTO>> GetHostModulesAsync(bool? isActive)
        {
            if (_context?.Modules == null)
            {
                _logger?.LogWarning("Unable to retrieve Host modules because the Module DbSet is unavailable.");
                return new List<GetHostModuleResponseDTO>();
            }

            try
            {
                // Module has no IsSoftDeleted property, so only its supported scope and activity filters apply.
                return await _context.Modules
                    .AsNoTracking()
                    .Where(x =>
                        x.ModuleScope == AppConstants.HostModuleScope &&
                        (!isActive.HasValue || x.IsActive == isActive.Value))
                    .OrderBy(x => x.ItemPriority)
                    .ThenBy(x => x.ModuleName)
                    .Select(x => new GetHostModuleResponseDTO
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        DisplayName = x.DisplayName,
                        Urlpath = x.Urlpath,
                        ParentModuleId = x.ParentModuleId,
                        IsLeafNode = x.IsLeafNode,
                        IsModuleDisplayInUi = x.IsModuleDisplayInUI,
                        IsCommonMenu = x.IsCommonMenu,
                        ModuleScope = x.ModuleScope,
                        IsActive = x.IsActive,
                        ImageIconWeb = x.ImageIconWeb,
                        ImageIconMobile = x.ImageIconMobile,
                        ItemPriority = x.ItemPriority,
                        Remark = x.Remark,
                        AddedById = x.AddedById,
                        AddedDateTime = x.AddedDateTime,
                        UpdatedById = x.UpdatedById,
                        UpdatedDateTime = x.UpdatedDateTime
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving Host modules. IsActive: {IsActive}", isActive);
                return new List<GetHostModuleResponseDTO>();
            }
        }

        /// <summary>
        /// Retrieves one Host-scope module by identifier, optionally filtered by its active state.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        /// <param name="isActive">When supplied, limits the result to the specified active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the database query.</param>
        /// <returns>The matching Host-scope module, or <see langword="null"/> when none exists.</returns>
        public async Task<GetHostModuleResponseDTO?> GetHostModuleByIdAsync(
            int id,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            if (_context?.Modules == null)
            {
                _logger?.LogWarning("Unable to retrieve Host module {ModuleId} because the Module DbSet is unavailable.", id);
                return null;
            }

            try
            {
                // Module has no IsSoftDeleted property, so only its supported identifier, scope, and activity filters apply.
                return await _context.Modules
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == id &&
                        x.ModuleScope == AppConstants.HostModuleScope &&
                        (!isActive.HasValue || x.IsActive == isActive.Value))
                    .Select(x => new GetHostModuleResponseDTO
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        ModuleCode = x.ModuleCode,
                        ModuleName = x.ModuleName,
                        DisplayName = x.DisplayName,
                        Urlpath = x.Urlpath,
                        ParentModuleId = x.ParentModuleId,
                        IsLeafNode = x.IsLeafNode,
                        IsModuleDisplayInUi = x.IsModuleDisplayInUI,
                        IsCommonMenu = x.IsCommonMenu,
                        ModuleScope = x.ModuleScope,
                        IsActive = x.IsActive,
                        ImageIconWeb = x.ImageIconWeb,
                        ImageIconMobile = x.ImageIconMobile,
                        ItemPriority = x.ItemPriority,
                        Remark = x.Remark,
                        AddedById = x.AddedById,
                        AddedDateTime = x.AddedDateTime,
                        UpdatedById = x.UpdatedById,
                        UpdatedDateTime = x.UpdatedDateTime
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error retrieving Host module {ModuleId}. IsActive: {IsActive}", id, isActive);
                throw;
            }
        }

        #endregion

        public async Task<Module?> GetModuleByIdAsync(long moduleId)
        {
            try
            {
               
                return await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.IsActive == true);
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
               
                return await _context.Modules
                    .FirstOrDefaultAsync(m => m.IsCommonMenu == true && m.IsModuleDisplayInUI == true && m.IsActive == true);
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
                
                    allModules = await _context.Modules
                        .Where(m => m.IsActive && m.IsModuleDisplayInUI)
                        .OrderBy(m => m.Id)
                        .ToListAsync();
               

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
                    URLPath = m.Urlpath,
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
                    SubModuleUrl = module.Urlpath,
                    URLPath = module.Urlpath,
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
                    SubModuleUrl = module.Urlpath,
                    URLPath = module.Urlpath,
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

        /// <summary>
        /// Persists a validated Parent/Header Module and lets PostgreSQL generate its identity value.
        /// </summary>
        /// <param name="entity">The Header Module entity prepared by the application layer.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The persisted entity, including its generated identifier.</returns>
        public async Task<Module> AddParentModuleAsync(Module entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            await context.Modules.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// Retrieves one scope-filtered Parent/Header Module by identifier without tracking it.
        /// </summary>
        /// <param name="id">The requested module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching response model, or <see langword="null"/> when it is not accessible.</returns>
        public async Task<GetParentModuleResponseDTO?> GetParentModuleByIdAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .AsNoTracking()
                .Where(module =>
                    module.Id == id &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId == null &&
                    module.IsLeafNode == false)
                .Select(ParentModuleProjection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves Parent/Header Modules for one validated module scope.
        /// </summary>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="isActive">When supplied, limits the results to the requested active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The ordered Parent/Header Module list.</returns>
        public async Task<List<GetParentModuleResponseDTO>> GetParentModulesAsync(
            short moduleScope,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            var modules = context.Modules
                .AsNoTracking()
                .Where(module =>
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId == null &&
                    module.IsLeafNode == false);

            if (isActive.HasValue)
            {
                modules = modules.Where(module => module.IsActive == isActive.Value);
            }

            return await modules
                .OrderBy(module => module.ItemPriority)
                .ThenBy(module => module.ModuleName)
                .Select(ParentModuleProjection)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Checks whether another Parent/Header Module already uses the supplied code in the same tenant and scope.
        /// </summary>
        /// <param name="moduleCode">The module code to check.</param>
        /// <param name="tenantId">The resolved tenant identifier, or <see langword="null"/> for Host scope.</param>
        /// <param name="moduleScope">The supported tenant module scope.</param>
        /// <param name="excludeModuleId">An existing module identifier to exclude during update.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when a conflicting Header Module exists.</returns>
        public async Task<bool> ExistsParentModuleCodeAsync(
            string moduleCode,
            long? tenantId,
            short moduleScope,
            int? excludeModuleId,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .AsNoTracking()
                .AnyAsync(module =>
                    module.TenantId == tenantId &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId == null &&
                    module.IsLeafNode == false &&
                    module.ModuleCode == moduleCode &&
                    (!excludeModuleId.HasValue || module.Id != excludeModuleId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Retrieves a tracked scope-filtered Parent/Header Module so a validated update can preserve its hierarchy.
        /// </summary>
        /// <param name="id">The requested module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching entity, or <see langword="null"/> when it is not accessible.</returns>
        public async Task<Module?> GetParentModuleForUpdateAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .FirstOrDefaultAsync(module =>
                    module.Id == id &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId == null &&
                    module.IsLeafNode == false,
                    cancellationToken);
        }

        /// <summary>
        /// Saves a validated Parent/Header Module update.
        /// </summary>
        /// <param name="entity">The tracked Header Module entity to save.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The saved Header Module entity.</returns>
        public async Task<Module> UpdateParentModuleAsync(Module entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            context.Modules.Update(entity);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// Determines whether a Parent/Header Module has active direct children in the same scope that would be left orphaned by deactivation.
        /// </summary>
        /// <param name="parentModuleId">The Header Module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when an active direct child exists.</returns>
        public async Task<bool> HasChildrenAsync(
            int parentModuleId,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .AsNoTracking()
                .AnyAsync(module =>
                    module.ParentModuleId == parentModuleId &&
                    module.ModuleScope == moduleScope &&
                    module.IsLeafNode == true &&
                    module.IsActive,
                    cancellationToken);
        }

        #region Parent Module Projection

        /// <summary>
        /// Defines the server-translatable response projection used by Parent Module reads.
        /// </summary>
        private static readonly Expression<Func<Module, GetParentModuleResponseDTO>> ParentModuleProjection = module =>
            new GetParentModuleResponseDTO
            {
                Id = module.Id,
                ModuleCode = module.ModuleCode,
                ModuleName = module.ModuleName,
                DisplayName = module.DisplayName,
                URLPath = module.Urlpath,
                ParentModuleId = module.ParentModuleId,
                IsLeafNode = module.IsLeafNode,
                IsModuleDisplayInUI = module.IsModuleDisplayInUI,
                IsCommonMenu = module.IsCommonMenu,
                ModuleScope = module.ModuleScope,
                IsActive = module.IsActive,
                ImageIconWeb = module.ImageIconWeb,
                ImageIconMobile = module.ImageIconMobile,
                ItemPriority = module.ItemPriority,
                Remark = module.Remark,
                AddedById = module.AddedById,
                AddedDateTime = module.AddedDateTime,
                UpdatedById = module.UpdatedById,
                UpdatedDateTime = module.UpdatedDateTime
            };

        #endregion

        #region SubModule Projection

        /// <summary>
        /// Defines the server-translatable response projection used by direct SubModule reads.
        /// </summary>
        private static readonly Expression<Func<Module, GetSubModuleResponseDTO>> SubModuleProjection = module =>
            new GetSubModuleResponseDTO
            {
                Id = module.Id,
                ModuleCode = module.ModuleCode,
                ModuleName = module.ModuleName,
                DisplayName = module.DisplayName,
                URLPath = module.Urlpath,
                ParentModuleId = module.ParentModuleId,
                IsLeafNode = module.IsLeafNode,
                IsModuleDisplayInUI = module.IsModuleDisplayInUI,
                IsCommonMenu = module.IsCommonMenu,
                ModuleScope = module.ModuleScope,
                IsActive = module.IsActive,
                ImageIconWeb = module.ImageIconWeb,
                ImageIconMobile = module.ImageIconMobile,
                ItemPriority = module.ItemPriority,
                Remark = module.Remark,
                AddedById = module.AddedById,
                AddedDateTime = module.AddedDateTime,
                UpdatedById = module.UpdatedById,
                UpdatedDateTime = module.UpdatedDateTime,
                ParentModule = module.ParentModule == null
                    ? null
                    : new ParentModuleSummaryDTO
                    {
                        Id = module.ParentModule.Id,
                        ModuleCode = module.ParentModule.ModuleCode,
                        ModuleName = module.ParentModule.ModuleName,
                        DisplayName = module.ParentModule.DisplayName,
                        ModuleScope = module.ParentModule.ModuleScope,
                        IsActive = module.ParentModule.IsActive
                    }
            };

        #endregion

        #region SubModule CRUD

        /// <summary>
        /// Retrieves a Header Module that is valid to own a direct child in the requested scope.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching tracked Header Module, or <see langword="null"/> when it is not a Header Module in the requested scope.</returns>
        public async Task<Module?> GetParentModuleForSubModuleAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .FirstOrDefaultAsync(module =>
                    module.Id == id &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId == null &&
                    module.IsLeafNode == false,
                    cancellationToken);
        }

        /// <summary>
        /// Retrieves one direct child SubModule with its Header Module summary.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching direct-child response, or <see langword="null"/> when it does not exist.</returns>
        public async Task<GetSubModuleResponseDTO?> GetSubModuleByIdAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .AsNoTracking()
                .Where(module =>
                    module.Id == id &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId != null &&
                    module.IsLeafNode == true)
                .Select(SubModuleProjection)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves direct child SubModules for one scope, optionally narrowed by Header Module and active state.
        /// </summary>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="parentModuleId">When supplied, limits results to the Header Module identifier.</param>
        /// <param name="isActive">When supplied, limits results to the requested active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The ordered direct-child SubModule list.</returns>
        public async Task<List<GetSubModuleResponseDTO>> GetSubModulesAsync(
            short moduleScope,
            int? parentModuleId,
            bool? isActive,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            var modules = context.Modules
                .AsNoTracking()
                .Where(module =>
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId != null &&
                    module.IsLeafNode == true);

            if (parentModuleId.HasValue)
            {
                modules = modules.Where(module => module.ParentModuleId == parentModuleId.Value);
            }

            if (isActive.HasValue)
            {
                modules = modules.Where(module => module.IsActive == isActive.Value);
            }

            return await modules
                .OrderBy(module => module.ItemPriority)
                .ThenBy(module => module.ModuleName)
                .Select(SubModuleProjection)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a tracked direct child SubModule for an update or status change.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching tracked direct child, or <see langword="null"/> when it does not exist.</returns>
        public async Task<Module?> GetSubModuleForUpdateAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .FirstOrDefaultAsync(module =>
                    module.Id == id &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId != null &&
                    module.IsLeafNode == true,
                    cancellationToken);
        }

        /// <summary>
        /// Checks whether another direct child SubModule already uses the supplied code in the same inherited tenant and scope.
        /// </summary>
        /// <param name="moduleCode">The module code to check.</param>
        /// <param name="tenantId">The tenant identifier inherited from the Header Module.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="excludeModuleId">An existing SubModule identifier to exclude during update.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when a conflicting direct child exists.</returns>
        public async Task<bool> ExistsSubModuleCodeAsync(
            string moduleCode,
            long? tenantId,
            short moduleScope,
            int? excludeModuleId,
            CancellationToken cancellationToken)
        {
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            return await context.Modules
                .AsNoTracking()
                .AnyAsync(module =>
                    module.TenantId == tenantId &&
                    module.ModuleScope == moduleScope &&
                    module.ParentModuleId != null &&
                    module.IsLeafNode == true &&
                    module.ModuleCode == moduleCode &&
                    (!excludeModuleId.HasValue || module.Id != excludeModuleId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Saves a validated tracked direct child SubModule update.
        /// </summary>
        /// <param name="entity">The direct child entity to save.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The saved direct child entity.</returns>
        public async Task<Module> UpdateSubModuleAsync(Module entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            context.Modules.Update(entity);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        #endregion

        public async Task<List<GetModuleChildInversResponseDTO>> GetSubParentModuleAsync(GetParentModuleRequestDTO module)
        {
            try
            {
                if (module == null)
                {
                    _logger.LogWarning("GetAllSubParentModuleAsync called with null module entity.");
                    return new List<GetModuleChildInversResponseDTO>();
                }

               

                if (_context.Modules == null)
                {
                    _logger.LogError("❌ DbSet<Module> is null in context.");
                    return new List<GetModuleChildInversResponseDTO>();
                }

                // ✅ Fetch parent modules based on flags
                var parentModules = await _context.Modules
                    .Where(m => m.IsActive
                             && m.IsLeafNode == false
                             && m.IsModuleDisplayInUI == module.IsModuleDisplayInUi
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

                

                if (_context.Modules == null)
                {
                    _logger.LogError("❌ DbSet<Module> is null in context.");
                    return new List<GetSubModuleResponseDTO>();
                }

                // ✅ Fetch SubModules under the given ParentModule
                var subModules = await _context.Modules
                    .Where(m => m.IsActive
                             && m.IsLeafNode == true
                             && m.IsModuleDisplayInUI == module.IsModuleDisplayInUI
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
               

                var existing = await _context.Modules.FindAsync(module.Id);
                if (existing == null) return false;

                existing.ModuleName = module.ModuleName;
                existing.ParentModuleId = module.ParentModuleId;
                existing.ImageIconWeb = module.ImageIconWeb;
                existing.ImageIconMobile = module.ImageIconMobile;
                existing.IsActive = module.IsActive;
                existing.UpdatedById = module.UpdatedById;
                existing.UpdatedDateTime = DateTime.Now;
                existing.Remark = module.Remark;

				_context.Modules.Update(existing);
                await _context.SaveChangesAsync();

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
              

                var module = await _context.Modules.FindAsync(moduleId);
                if (module == null) return false;

                module.IsActive = false;
                module.UpdatedDateTime = DateTime.Now;

				_context.Modules.Update(module);
                await _context.SaveChangesAsync();

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

              

                // ✅ Step 2: Check for duplicate module under same parent
                bool isDuplicate = await _context.Modules
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
                moduleEntity.IsModuleDisplayInUI = module.IsModuleDisplayInUI;
                moduleEntity.IsCommonMenu = module.IsCommonMenu;
                moduleEntity.IsActive = module.IsActive;

                // ✅ Step 5: Save to DB
                var entity = await _context.Modules.AddAsync(moduleEntity);
                await _context.SaveChangesAsync();

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
                        SubModuleUrl = module.Urlpath,
                        URLPath = module.Urlpath,
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
                        URLPath = parentModule.Urlpath,
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

        /// <summary>
        /// Persists a validated direct child SubModule and lets PostgreSQL generate its identity value.
        /// </summary>
        /// <param name="entity">The direct child entity prepared by the application layer.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The persisted direct child entity, including its generated identifier.</returns>
        public async Task<Module> AddSubModuleAsync(Module entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);
            var context = _context ?? throw new InvalidOperationException("Module context is unavailable.");

            await context.Modules.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return entity;
        }

       
        
    }
}
