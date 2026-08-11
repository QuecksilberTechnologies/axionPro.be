// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines and handles tenant Parent/Header Module updates.
// ============================================================================

using axionpro.application.Common.Models.Security;
using axionpro.application.Constants;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Command

    /// <summary>
    /// Represents the request to update the editable values of a Parent/Header Module.
    /// </summary>
    public class UpdateParentModuleCommand : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleCommand"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="requestDTO">The client-editable Header Module values.</param>
        public UpdateParentModuleCommand(int id, UpdateParentModuleRequestDTO? requestDTO)
        {
            Id = id;
            RequestDTO = requestDTO;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the client-editable Header Module values.</summary>
        public UpdateParentModuleRequestDTO? RequestDTO { get; }
    }

    #endregion

    /// <summary>
    /// Handles updates while preserving Parent/Header Module tenant ownership, scope, and hierarchy.
    /// </summary>
    public class UpdateParentModuleCommandHandler : IRequestHandler<UpdateParentModuleCommand, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateParentModuleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="commonRequestService">Validates the authenticated tenant and actor context.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public UpdateParentModuleCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<UpdateParentModuleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Updates only client-editable Header Module fields for the authenticated tenant.
        /// </summary>
        /// <param name="request">The update request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The updated Header Module response.</returns>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            UpdateParentModuleCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.Id <= 0 || request.RequestDTO == null)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("A valid Parent Module identifier and data are required.");
            }

            var context = await _commonRequestService.ValidateRequestAsync();
            if (!context.Success)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail(context.ErrorMessage);
            }

            var dto = request.RequestDTO;
            if (string.IsNullOrWhiteSpace(dto.ModuleCode) || string.IsNullOrWhiteSpace(dto.ModuleName))
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("ModuleCode and ModuleName are required.");
            }

            try
            {
                var moduleScope = (short)AppConstants.TenantModuleScope;
                var entity = await _unitOfWork.ModuleRepository.GetParentModuleForUpdateAsync(
                    request.Id,
                    context.TenantId,
                    moduleScope,
                    cancellationToken);

                if (entity == null)
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail("Parent Module was not found.");
                }

                var moduleCode = dto.ModuleCode.Trim();
                var duplicateExists = await _unitOfWork.ModuleRepository.ExistsParentModuleCodeAsync(
                    moduleCode,
                    context.TenantId,
                    moduleScope,
                    entity.Id,
                    cancellationToken);

                if (duplicateExists)
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail("A Parent Module with this ModuleCode already exists.");
                }

                if (entity.IsActive && !dto.IsActive && await _unitOfWork.ModuleRepository.HasChildrenAsync(
                    entity.Id,
                    context.TenantId,
                    moduleScope,
                    cancellationToken))
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail(
                        "Deactivate active child modules before deactivating this Parent Module.");
                }

                entity.ModuleCode = moduleCode;
                entity.ModuleName = dto.ModuleName.Trim();
                entity.DisplayName = dto.DisplayName?.Trim();
                entity.Urlpath = dto.URLPath?.Trim();
                entity.ParentModuleId = null;
                entity.IsLeafNode = false;
                entity.IsModuleDisplayInUi = dto.IsModuleDisplayInUI;
                entity.IsCommonMenu = dto.IsCommonMenu;
                entity.IsActive = dto.IsActive;
                entity.ImageIconWeb = dto.ImageIconWeb?.Trim();
                entity.ImageIconMobile = dto.ImageIconMobile?.Trim();
                entity.ItemPriority = dto.ItemPriority;
                entity.Remark = dto.Remark?.Trim();
                entity.UpdatedById = GetActorId(context);
                entity.UpdatedDateTime = DateTime.UtcNow;

                var updated = await _unitOfWork.ModuleRepository.UpdateParentModuleAsync(entity, cancellationToken);

                return ApiResponse<GetParentModuleResponseDTO>.Success(
                    ToResponse(updated),
                    "Parent Module updated successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update Parent Module {ModuleId} for tenant {TenantId}.", request.Id, context.TenantId);
                return ApiResponse<GetParentModuleResponseDTO>.Fail("Failed to update Parent Module.");
            }
        }

        #endregion

        #region Parent Module Validation

        /// <summary>
        /// Returns the authenticated employee identifier used for audit fields.
        /// </summary>
        /// <param name="context">The validated request context.</param>
        /// <returns>The authenticated employee identifier.</returns>
        private static long GetActorId(CommonDecodedResult context)
        {
            return context.UserEmployeeId > 0 ? context.UserEmployeeId : context.LoggedInEmployeeId;
        }

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
                IsModuleDisplayInUI = module.IsModuleDisplayInUi,
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
}
