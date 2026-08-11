// ============================================================================
// Author      : Deepesh Gupta
// Company     : Quecksilber Technologies
// Role        : CEO
// Purpose     : Defines and handles retrieval of one tenant Parent/Header Module.
// ============================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands
{
    #region Query

    /// <summary>
    /// Represents a read-only request for one Parent/Header Module.
    /// </summary>
    public class GetParentModuleByIdQuery : IRequest<ApiResponse<GetParentModuleResponseDTO>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleByIdQuery"/> class.
        /// </summary>
        /// <param name="id">The Header Module identifier.</param>
        public GetParentModuleByIdQuery(int id)
        {
            Id = id;
        }

        /// <summary>Gets the Header Module identifier.</summary>
        public int Id { get; }
    }

    #endregion

    /// <summary>
    /// Handles one Parent/Header Module lookup with tenant and scope isolation.
    /// </summary>
    public class GetParentModuleByIdQueryHandler : IRequestHandler<GetParentModuleByIdQuery, ApiResponse<GetParentModuleResponseDTO>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetParentModuleByIdQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetParentModuleByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">Provides the existing Module repository.</param>
        /// <param name="commonRequestService">Validates the authenticated tenant context.</param>
        /// <param name="logger">Records unexpected processing failures.</param>
        public GetParentModuleByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetParentModuleByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region MediatR Handler

        /// <summary>
        /// Retrieves one Header Module only when it belongs to the authenticated tenant and tenant module scope.
        /// </summary>
        /// <param name="request">The read-only request.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The matching Header Module response.</returns>
        public async Task<ApiResponse<GetParentModuleResponseDTO>> Handle(
            GetParentModuleByIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.Id <= 0)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail("A valid Parent Module identifier is required.");
            }

            var context = await _commonRequestService.ValidateRequestAsync();
            if (!context.Success)
            {
                return ApiResponse<GetParentModuleResponseDTO>.Fail(context.ErrorMessage);
            }

            try
            {
                var module = await _unitOfWork.ModuleRepository.GetParentModuleByIdAsync(
                    request.Id,
                    context.TenantId,
                    (short)AppConstants.TenantModuleScope,
                    cancellationToken);

                if (module == null)
                {
                    return ApiResponse<GetParentModuleResponseDTO>.Fail("Parent Module was not found.");
                }

                return ApiResponse<GetParentModuleResponseDTO>.Success(module, "Parent Module retrieved successfully.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to retrieve Parent Module {ModuleId} for tenant {TenantId}.", request.Id, context.TenantId);
                return ApiResponse<GetParentModuleResponseDTO>.Fail("Failed to retrieve Parent Module.");
            }
        }

        #endregion
    }
}
