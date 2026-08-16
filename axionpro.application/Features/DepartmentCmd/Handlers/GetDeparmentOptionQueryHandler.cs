// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves active department options using trusted tenant context.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to retrieve active department options.
    /// </summary>
    public class GetDepartmentOptionQuery : IRequest<ApiResponse<List<GetDepartmentOptionResponse>>>
    {
        public GetOptionRequestDTO OptionDTO { get; }

        /// <summary>
        /// Initializes the option query.
        /// </summary>
        public GetDepartmentOptionQuery(GetOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles active department-option queries for the authenticated tenant.
    /// </summary>
    public class GetDepartmentOptionQueryHandler : IRequestHandler<GetDepartmentOptionQuery, ApiResponse<List<GetDepartmentOptionResponse>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetDepartmentOptionQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public GetDepartmentOptionQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetDepartmentOptionQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves active options without using the request DTO as a trusted context container.
        /// </summary>
        public async Task<ApiResponse<List<GetDepartmentOptionResponse>>> Handle(
            GetDepartmentOptionQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            var departments = await _unitOfWork.DepartmentRepository.GetOptionAsync(
                validation.TenantId,
                cancellationToken);

            var options = departments
                .Where(option => option != null)
                .Cast<GetDepartmentOptionResponse>()
                .ToList() ?? new List<GetDepartmentOptionResponse>();

            _logger.LogInformation(
                "Retrieved {Count} department options for TenantId: {TenantId}",
                options.Count,
                validation.TenantId);

            return new ApiResponse<List<GetDepartmentOptionResponse>>
            {
                IsSucceeded = true,
                Message = options.Count == 0
                    ? "No departments found for this tenant."
                    : "Department options fetched successfully.",
                Data = options
            };
        }

        #endregion
    }

    #endregion
}
