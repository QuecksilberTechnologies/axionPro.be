// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles Parent Module status changes and cascades active state to direct Child Modules and their operation mappings.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to change a Parent/Header Module active state in a required scope.
    /// </summary>
    public class UpdateParentModuleStatusCommand : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommand"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="dto">The target active state and required module scope.</param>
        public UpdateParentModuleStatusCommand(int id, UpdateParentModuleStatusRequestDTO? dto)
        {
            Id = id;
            DTO = dto;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the target active state and required module scope.</summary>
        public UpdateParentModuleStatusRequestDTO? DTO { get; }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles Host-authorized Parent Module status changes and cascades the requested state to direct child modules and their operation mappings.
    /// </summary>
    public class UpdateParentModuleStatusCommandHandler : IRequestHandler<UpdateParentModuleStatusCommand, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the module configuration repository.</param>
        /// <param name="commonRequestService">Validates the authenticated Host request and resolves its trusted actor.</param>
        public UpdateParentModuleStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Updates a Parent Module's active state and cascades the same value to its direct child modules and their operation mappings.
        /// </summary>
        /// <param name="request">The status change request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The Parent Module response after the complete cascade succeeds.</returns>
        /// <exception cref="ValidationErrorException">Thrown when the request or module scope is invalid.</exception>
        /// <exception cref="NotFoundException">Thrown when the requested identifier does not resolve to a Parent Module in the requested scope.</exception>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            UpdateParentModuleStatusCommand request,
            CancellationToken cancellationToken)
        {
            // Resolve the authenticated Host actor before processing client-provided data.
            var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();
            cancellationToken.ThrowIfCancellationRequested();

            if (request == null || request.Id <= 0 || request.DTO == null)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            var dto = request.DTO;
            if (!IsSupportedModuleScope(dto.ModuleScope))
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            // Load the tracked Parent Module; a child identifier does not match this parent-only query.
            var parentModule = await _unitOfWork.ModuleRepository.GetParentModuleForUpdateAsync(
                request.Id,
                request.DTO.ModuleScope,
                cancellationToken);

            if (parentModule == null)
            {
                throw new NotFoundException(AppConstants.ErrorMessages.ParentModuleNotFound);
            }

            // Load all direct child modules in one database query.
            var childModules = await _unitOfWork.ModuleRepository.GetDirectChildModulesForStatusUpdateAsync(
                parentModule.Id,
                parentModule.ModuleScope,
                cancellationToken);

            var childModuleIds = childModules.Select(module => module.Id).ToList();
            var operationMappings = childModuleIds.Count == 0
                ? new List<ModuleOperationMapping>()
                : await _unitOfWork.ModuleRepository.GetModuleOperationMappingsForStatusUpdateAsync(
                    childModuleIds,
                    cancellationToken);

            var updatedDateTime = DateTime.UtcNow;

            // Apply the requested active state while preserving module hierarchy and creation data.
            parentModule.IsActive = dto.IsActive;
            parentModule.UpdatedById = hostUserId;
            parentModule.UpdatedDateTime = updatedDateTime;

            // Apply the requested active state to the affected direct children only.
            foreach (var childModule in childModules)
            {
                childModule.IsActive = dto.IsActive;
                childModule.UpdatedById = hostUserId;
                childModule.UpdatedDateTime = updatedDateTime;
            }

            // Apply the requested active state to mappings belonging to the affected child modules only.
            foreach (var operationMapping in operationMappings)
            {
                operationMapping.IsActive = dto.IsActive;
                operationMapping.UpdatedById = hostUserId;
                operationMapping.UpdatedDateTime = updatedDateTime;
            }

            // Persist the complete tracked cascade with one SaveChangesAsync call.
            await _unitOfWork.ModuleRepository.SaveModuleStatusCascadeAsync(cancellationToken);

            return ApiResponse<GetParentModuleResponseDTO>.Success(
                ToResponse(parentModule),
                AppConstants.SuccessMessages.ParentModuleStatusUpdatedSuccessfully);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines whether the requested scope is one of the two supported application module scopes.
        /// </summary>
        /// <param name="moduleScope">The requested module scope.</param>
        /// <returns><see langword="true"/> when the scope is supported.</returns>
        private static bool IsSupportedModuleScope(short moduleScope)
        {
            return moduleScope == AppConstants.TenantModuleScope ||
                   moduleScope == AppConstants.HostModuleScope;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Maps a persisted Header Module to the CRUD response.
        /// </summary>
        /// <param name="module">The persisted Header Module.</param>
        /// <returns>The response model.</returns>
        private static GetParentModuleResponseDTO ToResponse(Module module)
        {
            return new GetParentModuleResponseDTO
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
        }

        #endregion
    }

    #endregion
}
