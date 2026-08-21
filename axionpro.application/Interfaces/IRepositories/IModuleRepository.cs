// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines module, module-operation, and shared Common-navigation persistence operations.
// ================================================================

using axionpro.application.DTOs;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.CommonMenu;
using axionpro.application.DTOS.Module.ManualModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.Features.ModuleCmd.Parent.Commands;
using axionpro.application.Features.ModuleCmd.SubModule.Commands;
using axionpro.application.Features.OperationCmd.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Host;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IModuleRepository
    {
        #region Host Module Queries

        /// <summary>
        /// Retrieves Host-scope modules, optionally filtered by their active state.
        /// </summary>
        /// <param name="isActive">When supplied, limits results to modules with the specified active state.</param>
        /// <returns>A list of Host-scope module response models.</returns>
        Task<List<GetHostModuleResponseDTO>> GetHostModulesAsync(bool? isActive);

        /// <summary>
        /// Retrieves one Host-scope module by identifier, optionally filtered by its active state.
        /// </summary>
        /// <param name="id">The module identifier.</param>
        /// <param name="isActive">When supplied, limits the result to the specified active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the database query.</param>
        /// <returns>The matching Host-scope module, or <see langword="null"/> when none exists.</returns>
        Task<GetHostModuleResponseDTO?> GetHostModuleByIdAsync(
            int id,
            bool? isActive,
            CancellationToken cancellationToken);

        #endregion

        #region Common Navigation Queries

        /// <summary>
        /// Retrieves the unique active, UI-visible structural root of the shared Common navigation hierarchy.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the database operation.</param>
        /// <returns>The unique Common root, or <see langword="null"/> when none is configured.</returns>
        Task<Module?> GetCommonMenuParentAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the visible active navigation children beneath the supplied Common root.
        /// </summary>
        /// <param name="parentModuleId">The validated Common root identifier.</param>
        /// <param name="cancellationToken">A token used to cancel the database operation.</param>
        /// <returns>The ordered child hierarchy below the supplied root.</returns>
        Task<List<ModuleDTO>> GetCommonMenuTreeAsync(
            int? parentModuleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the deterministic active Common-menu hierarchy displayed across the authenticated application.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the database operation.</param>
        /// <returns>The configured Common navigation items, or <see langword="null"/> when no Common root exists.</returns>
        Task<IReadOnlyCollection<CommonMenuItemResponseDTO>?> GetCommonMenuHierarchyAsync(
            CancellationToken cancellationToken = default);

        #endregion

        Task<Module?> GetModuleByIdAsync(long moduleId);

        #region ModuleOperation Mapping CRUD

        /// <summary>
        /// Persists a validated module-operation mapping.
        /// </summary>
        /// <param name="entity">The mapping entity to persist.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The persisted mapping, including its generated identifier.</returns>
        Task<ModuleOperationMapping> CreateModuleOperationMappingAsync(
            ModuleOperationMapping entity,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves one module-operation mapping with its module configuration lookups.
        /// </summary>
        /// <param name="id">The mapping identifier.</param>
        /// <param name="cancellationToken">A token to observe while querying.</param>
        /// <returns>The matching mapping, or <see langword="null"/> when it does not exist.</returns>
        Task<ModuleOperationMapping?> GetModuleOperationMappingByIdAsync(
            int id,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all module-operation mappings with their module configuration lookups.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while querying.</param>
        /// <returns>The ordered module-operation mappings.</returns>
        Task<List<ModuleOperationMapping>> GetAllModuleOperationMappingsAsync(
            CancellationToken cancellationToken);

        /// <summary>
        /// Saves changes to a validated module-operation mapping.
        /// </summary>
        /// <param name="entity">The mapping entity to update.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The updated mapping.</returns>
        Task<ModuleOperationMapping> UpdateModuleOperationMappingAsync(
            ModuleOperationMapping entity,
            CancellationToken cancellationToken);

        /// <summary>
        /// Deactivates a module-operation mapping and records the acting Host user.
        /// </summary>
        /// <param name="id">The mapping identifier.</param>
        /// <param name="hostUserId">The authenticated Host user identifier.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns><see langword="true"/> when a mapping was deactivated; otherwise <see langword="false"/>.</returns>
        Task<bool> DeactivateModuleOperationMappingAsync(
            int id,
            long hostUserId,
            CancellationToken cancellationToken);

        #endregion

        #region Operation Dependency Queries

        /// <summary>
        /// Determines whether an operation is referenced by any persisted module-operation mapping.
        /// The current mapping schema has no soft-delete property; a mapping is ignored only after it is removed from the database.
        /// Mapping activity is intentionally not used because an inactive mapping still represents a relationship.
        /// </summary>
        /// <param name="operationId">The operation identifier to check.</param>
        /// <param name="cancellationToken">A token to observe while executing the database existence query.</param>
        /// <returns><see langword="true"/> when a module-operation mapping still references the operation; otherwise <see langword="false"/>.</returns>
        Task<bool> IsOperationLinkedToAnyModuleAsync(
            int operationId,
            CancellationToken cancellationToken);

        #endregion

        /// <summary>
        /// Sare modules laata hai (optionally filterable)
        /// </summary>
    

        Task<List<ModuleDTO>> GetAllActiveModulesAsync(List<ModuleDTO> modules);

        /// <summary>
        /// Creates a Parent/Header Module after the caller has established its tenant, scope, hierarchy, and audit values.
        /// </summary>
        #region Create All module

        /// <summary>
        /// Persists a Parent/Header Module and returns the database-generated entity.
        /// </summary>
        /// <param name="entity">The Header Module entity to persist.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The persisted entity, including its database-generated identifier.</returns>
        Task<Module> AddParentModuleAsync(Module entity, CancellationToken cancellationToken);

        Task<List<GetCommonModuleResponseDTO>> AddCommonModuleAsync(CreateCommonModuleRequestDTO Dto);
        Task<List<GetModuleChildInversResponseDTO>> AddSubModuleAsync(CreateSubModuleRequestDTO Dto);     

        #endregion
        #region Get All module

     //   Task<List<GetSubModuleResponseDTO>> GetSubModuleAsync(GetCommonModuleRequestDTO Dto);

        /// <summary>
        /// Retrieves one scope-filtered Parent/Header Module by identifier.
        /// </summary>
        /// <param name="id">The requested module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching response model, or <see langword="null"/> when it does not exist.</returns>
        Task<GetParentModuleResponseDTO?> GetParentModuleByIdAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves scope-filtered Parent/Header Modules, optionally filtered by active state.
        /// </summary>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="isActive">When supplied, limits results to the requested active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The ordered Parent/Header Module list.</returns>
        Task<List<GetParentModuleResponseDTO>> GetParentModulesAsync(
            short moduleScope,
            bool? isActive,
            CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether a Header Module code is already used in the tenant and scope.
        /// </summary>
        /// <param name="moduleCode">The module code to check.</param>
        /// <param name="tenantId">The resolved tenant identifier, or <see langword="null"/> for Host scope.</param>
        /// <param name="moduleScope">The supported tenant module scope.</param>
        /// <param name="excludeModuleId">An existing module identifier to exclude during update.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when a conflicting Header Module exists.</returns>
        Task<bool> ExistsParentModuleCodeAsync(
            string moduleCode,
            long? tenantId,
            short moduleScope,
            int? excludeModuleId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a tracked scope-filtered Parent/Header Module for a guarded update operation.
        /// </summary>
        /// <param name="id">The requested module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching entity, or <see langword="null"/> when it does not exist.</returns>
        Task<Module?> GetParentModuleForUpdateAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Saves changes made to a tracked Parent/Header Module.
        /// </summary>
        /// <param name="entity">The validated Header Module entity to update.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The saved entity.</returns>
        Task<Module> UpdateParentModuleAsync(Module entity, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether a Header Module has active direct child modules in the same scope.
        /// </summary>
        /// <param name="parentModuleId">The Header Module identifier.</param>
        /// <param name="moduleScope">The requested validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when an active direct child exists.</returns>
        Task<bool> HasChildrenAsync(
            int parentModuleId,
            short moduleScope,
            CancellationToken cancellationToken);

        #region Module Status Cascade

        /// <summary>
        /// Retrieves tracked direct child modules for a Parent Module status cascade.
        /// The current Module schema has no soft-delete field, so every direct child in the requested scope is returned.
        /// </summary>
        /// <param name="parentModuleId">The validated Parent Module identifier.</param>
        /// <param name="moduleScope">The validated module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The tracked direct child modules.</returns>
        Task<List<Module>> GetDirectChildModulesForStatusUpdateAsync(
            int parentModuleId,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves tracked operation mappings for the supplied direct child module identifiers.
        /// The current mapping schema has no soft-delete field, so every mapping for the affected children is returned.
        /// </summary>
        /// <param name="moduleIds">The affected direct child module identifiers.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The tracked operation mappings for the affected child modules.</returns>
        Task<List<ModuleOperationMapping>> GetModuleOperationMappingsForStatusUpdateAsync(
            IReadOnlyCollection<int> moduleIds,
            CancellationToken cancellationToken);

        /// <summary>
        /// Persists all tracked changes prepared for one Parent Module status cascade.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while saving the complete cascade.</param>
        /// <returns>A task that completes after the cascade is persisted.</returns>
        Task SaveModuleStatusCascadeAsync(CancellationToken cancellationToken);

        #endregion

        #region SubModule CRUD

        /// <summary>
        /// Persists a validated direct child SubModule and returns the database-generated entity.
        /// </summary>
        /// <param name="entity">The direct child entity prepared by the application layer.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The persisted direct child entity.</returns>
        Task<Module> AddSubModuleAsync(Module entity, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves one Header Module validated for direct child operations in the requested scope.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching tracked Header Module, or <see langword="null"/> when it does not exist.</returns>
        Task<Module?> GetParentModuleForSubModuleAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves one direct child SubModule with a Header Module summary in the requested scope.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching response, or <see langword="null"/> when it does not exist.</returns>
        Task<GetSubModuleResponseDTO?> GetSubModuleByIdAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves direct child SubModules in a requested scope with parent summaries.
        /// </summary>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="parentModuleId">When supplied, limits results to this Header Module.</param>
        /// <param name="isActive">When supplied, limits results to the specified active state.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The ordered direct-child SubModule list.</returns>
        Task<List<GetSubModuleResponseDTO>> GetSubModulesAsync(
            short moduleScope,
            int? parentModuleId,
            bool? isActive,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a tracked direct child SubModule for a guarded update operation.
        /// </summary>
        /// <param name="id">The SubModule identifier.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns>The matching tracked direct child, or <see langword="null"/> when it does not exist.</returns>
        Task<Module?> GetSubModuleForUpdateAsync(
            int id,
            short moduleScope,
            CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether a direct child module code already exists in the same inherited tenant and scope.
        /// </summary>
        /// <param name="moduleCode">The module code to check.</param>
        /// <param name="tenantId">The tenant identifier inherited from the Header Module.</param>
        /// <param name="moduleScope">The required module scope.</param>
        /// <param name="excludeModuleId">An existing SubModule identifier to exclude during update.</param>
        /// <param name="cancellationToken">A token to observe while executing the query.</param>
        /// <returns><see langword="true"/> when a conflicting direct child exists.</returns>
        Task<bool> ExistsSubModuleCodeAsync(
            string moduleCode,
            long? tenantId,
            short moduleScope,
            int? excludeModuleId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Saves changes made to a validated tracked direct child SubModule.
        /// </summary>
        /// <param name="entity">The validated direct child entity to update.</param>
        /// <param name="cancellationToken">A token to observe while saving changes.</param>
        /// <returns>The saved direct child entity.</returns>
        Task<Module> UpdateSubModuleAsync(Module entity, CancellationToken cancellationToken);

        #endregion

        Task<List<GetParentModuleResponseDTO>> GetSubParentModuleAsync(GetSubParentModulRequestDTO Dto);
        Task<List<GetCommonModuleResponseDTO>> GetCommonModuleAsync(GetCommonModuleRequestDTO Dto);
        Task<List<GetModuleChildInversResponseDTO>> GetAllOnlyModuleTreeAsync();
        Task<List<GetModuleChildInversResponseDTO>> GetAllModuleTreeAsync();
        #endregion
        /// <summary>
        /// Module ko update karta hai
        /// </summary>
        Task<bool> UpdateModuleAsync(Module module);

        /// <summary>
        /// Module ko soft/hard delete karta hai
        /// </summary>
        Task<bool> DeleteModuleAsync(long moduleId);


    }
}
