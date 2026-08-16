// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Retrieves tenant-scoped department projections using trusted context.
// ================================================================

using axionpro.application.DTOs.Department;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{
    #region Query

    /// <summary>
    /// Represents a request to retrieve departments.
    /// </summary>
    public class GetDepartmentQuery : IRequest<ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        public GetDepartmentRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the query with client-supplied filters.
        /// </summary>
        public GetDepartmentQuery(GetDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles department listing requests for the authenticated tenant.
    /// </summary>
    public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetDepartmentQueryHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public GetDepartmentQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetDepartmentQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Retrieves paged departments without mutating the request DTO with server context.
        /// </summary>
        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(
            GetDepartmentQuery request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            var response = await _unitOfWork.DepartmentRepository.GetAsync(
                request.DTO,
                validation.TenantId,
                cancellationToken);

            var departments = response.Data ?? new List<GetDepartmentResponseDTO>();
            _logger.LogInformation(
                "Retrieved {Count} departments for TenantId: {TenantId}",
                departments.Count,
                validation.TenantId);

            return new ApiResponse<List<GetDepartmentResponseDTO>>
            {
                IsSucceeded = true,
                Message = departments.Count == 0
                    ? "No data found."
                    : $"{response.TotalCount} record(s) retrieved successfully.",
                PageNumber = response.PageNumber,
                PageSize = response.PageSize,
                TotalRecords = response.TotalCount,
                TotalPages = response.TotalPages,
                Data = departments
            };
        }

        #endregion
    }

    #endregion
}
