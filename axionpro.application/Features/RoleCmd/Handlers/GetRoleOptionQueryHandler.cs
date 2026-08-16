// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles authenticated requests for tenant role option projections.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOS.Role;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request for role options in the authenticated tenant context.
    /// </summary>
    public class GetRoleOptionQuery : IRequest<ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        public GetRoleOptionRequestDTO OptionDTO { get; set; } = default!;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleOptionQuery"/> class.
        /// </summary>
        /// <param name="optionDTO">The role option query criteria.</param>
        public GetRoleOptionQuery(GetRoleOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles authenticated tenant requests for role option projections.
    /// </summary>
    public class GetRoleOptionQueryHandler : IRequestHandler<GetRoleOptionQuery, ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetRoleOptionQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GetRoleOptionQueryHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The persistence unit of work.</param>
        /// <param name="commonRequestService">The authenticated request validator.</param>
        /// <param name="logger">The diagnostic logger.</param>
        public GetRoleOptionQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetRoleOptionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves tenant role options and constructs the successful API response.
        /// </summary>
        public async Task<ApiResponse<List<GetRoleOptionResponseDTO>>> Handle(
            GetRoleOptionQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.OptionDTO == null)
            {
                throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
            }

            request.OptionDTO.Prop ??= new();
            request.OptionDTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.OptionDTO.Prop.TenantId = validation.TenantId;

            var roles = await _unitOfWork.RoleRepository.GetOptionAsync(request.OptionDTO);

            _logger.LogInformation(
                "Retrieved {Count} role options for tenant {TenantId}.",
                roles.Count,
                validation.TenantId);

            // Build the application response in the handler layer.
            return ApiResponse<List<GetRoleOptionResponseDTO>>.Success(
                roles,
                AppConstants.SuccessMessages.RoleOptionsRetrieved);
        }

        #endregion
    }

    #endregion
}
