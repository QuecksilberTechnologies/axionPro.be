// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles GetRoleQueryHandler requests using authenticated tenant context.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to retrieve paged roles for the authenticated tenant.
    /// </summary>
    public class GetRoleQuery : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public GetRoleRequestDTO DTO { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleQuery"/> class.
        /// </summary>
        public GetRoleQuery(GetRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles paged role queries using the authenticated tenant context.
    /// </summary>
    public class GetRoleQueryHandler : IRequestHandler<GetRoleQuery, ApiResponse<List<GetRoleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRoleQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleQueryHandler"/> class.
        /// </summary>
        public GetRoleQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetRoleQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves roles for the trusted tenant and builds the paged application response.
        /// </summary>
        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.DTO == null)
            {
                throw new ArgumentNullException(nameof(request.DTO));
            }

            var roles = await _unitOfWork.RoleRepository.GetAsync(
                validation.TenantId,
                request.DTO);

            _logger.LogInformation(
                "Retrieved {Count} roles for tenant {TenantId}.",
                roles.TotalCount,
                validation.TenantId);

            return ApiResponse<List<GetRoleResponseDTO>>.SuccessPaginated(
                roles.Data ?? new List<GetRoleResponseDTO>(),
                roles.PageNumber,
                roles.PageSize,
                roles.TotalCount,
                roles.TotalPages,
                AppConstants.SuccessMessages.RolesRetrieved);
        }

        #endregion
    }

    #endregion
}
