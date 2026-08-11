// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines and handles non-destructive Parent/Header Module status changes.
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
    /// Represents the request to change a Parent/Header Module's active state.
    /// </summary>
    public class UpdateParentModuleStatusCommand : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommand"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        /// <param name="requestDTO">The requested target active state.</param>
        public UpdateParentModuleStatusCommand(int id, UpdateParentModuleStatusRequestDTO? requestDTO)
        {
            Id = id;
            RequestDTO = requestDTO;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }

        /// <summary>Gets the requested target active state.</summary>
        public UpdateParentModuleStatusRequestDTO? RequestDTO { get; }
    }

    #endregion

    /// <summary>
    /// Handles non-destructive active-state changes for tenant Parent/Header Modules.
    /// </summary>
    public class UpdateParentModuleStatusCommandHandler : IRequestHandler<UpdateParentModuleStatusCommand, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<UpdateParentModuleStatusCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateParentModuleStatusCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="commonRequestService">Validates the authenticated tenant and actor context.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public UpdateParentModuleStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<UpdateParentModuleStatusCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Changes the active state while preventing an active Header Module from hiding active direct children.
        /// </summary>
        /// <param name="request">The status change request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The Header Module response after a permitted status change.</returns>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            UpdateParentModuleStatusCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.Id <= 0 || request.RequestDTO == null)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("A valid Parent Module identifier and status are required.");
            }

            var context = await _commonRequestService.ValidateRequestAsync();
            if (!context.Success)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail(context.ErrorMessage);
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

                if (entity.IsActive && !request.RequestDTO.IsActive && await _unitOfWork.ModuleRepository.HasChildrenAsync(
                    entity.Id,
                    context.TenantId,
                    moduleScope,
                    cancellationToken))
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail(
                        "Deactivate active child modules before deactivating this Parent Module.");
                }

                entity.ParentModuleId = null;
                entity.IsLeafNode = false;
                entity.IsActive = request.RequestDTO.IsActive;
                entity.UpdatedById = GetActorId(context);
                entity.UpdatedDateTime = DateTime.UtcNow;

                var updated = await _unitOfWork.ModuleRepository.UpdateParentModuleAsync(entity, cancellationToken);

                return ApiResponse<GetParentModuleResponseDTO>.Success(
                    ToResponse(updated),
                    "Parent Module status updated successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to update Parent Module {ModuleId} status for tenant {TenantId}.", request.Id, context.TenantId);
                return ApiResponse<GetParentModuleResponseDTO>.Fail("Failed to update Parent Module status.");
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
