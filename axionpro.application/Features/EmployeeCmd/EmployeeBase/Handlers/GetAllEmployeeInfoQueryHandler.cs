// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles GetAllEmployeeInfoQueryHandler requests using authenticated tenant context.
// ================================================================



using axionpro.application.Constants;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{

    #region Query

    /// <summary>
    /// Represents the GetAllEmployeeInfoQuery application component.
    /// </summary>
    public class GetAllEmployeeInfoQuery : IRequest<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        public GetAllEmployeeInfoRequestDTO DTO { get; }

        public GetAllEmployeeInfoQuery(GetAllEmployeeInfoRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    /// <summary>
    /// Handles authenticated tenant requests for this feature.
    /// </summary>
        #endregion

    #region Handler

public class GetAllEmployeeInfoQueryHandler : IRequestHandler<GetAllEmployeeInfoQuery, ApiResponse<List<GetAllEmployeeInfoResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllEmployeeInfoQueryHandler> _logger;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;

        public GetAllEmployeeInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllEmployeeInfoQueryHandler> logger,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
        }
        public async Task<ApiResponse<List<GetAllEmployeeInfoResponseDTO>>> Handle(
    GetAllEmployeeInfoQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetAllEmployeeInfo started");

                // ===============================
                // Validate the request.
                // ===============================
                #region Tenant Request Validation
                var validation =
                    await _commonRequestService.ValidateTenantUserRequestAsync();
                #endregion

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // Enforce required request data.
                // ===============================
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request");

                long? employeeId = null;
                if (!string.IsNullOrWhiteSpace(request.DTO.EmployeeId))
                {
                    employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                    if (employeeId <= 0)
                        throw new ValidationErrorException("Invalid EmployeeId.");
                }
                // Retrieve the requested employee records.
                // ===============================
                var responseDTO = await _unitOfWork.Employees.GetAllInfo(
                    validation.TenantId,
                    employeeId,
                    request.DTO);

                if (responseDTO == null)
                    throw new ApiException("Employee data not found", 404);

                // ===============================
                // Preserve the existing empty-result behavior.
                // ===============================
                var items = responseDTO?.Data ?? new List<GetAllEmployeeInfoResponseDTO>();

                // EmployeeTenantPermissionBehavior has already enforced the Module and stored-procedure
                // permission check. The validated context below supplies the trusted viewer identity used
                // only to shape the response; no client-provided role or EmployeeId controls visibility.
                var resultList = items.Any()
                    ? ProjectionHelper.ToGetAllEmployeeInfoResponseDTOs(responseDTO, _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _fileStorageService,
                        validation.LoggedInEmployeeId,
                        validation.RoleTypeId)
                    : new List<GetAllEmployeeInfoResponseDTO>();

                // Expose the configured assignment limit on every existing Employee row without another role query.
                foreach (var employee in resultList)
                {
                    employee.MaxRoleAssigned = AppConstants.MaxEmployeeRoleAssigned;
                }

                _logger.LogInformation("GetAllEmployeeInfo success");

                // ===============================
                // Build the standardized response.
                // ===============================
                return ApiResponse<List<GetAllEmployeeInfoResponseDTO>> .SuccessPaginatedPercentage(
                        Data: resultList,
                        Message: items.Any()
                            ? "Employee info retrieved successfully."
                            : "No employee info found.",
                        PageNumber: responseDTO?.PageNumber ?? 1,
                        PageSize: responseDTO?.PageSize ?? 0,
                        TotalRecords: responseDTO?.TotalCount ?? 0,
                        TotalPages: responseDTO?.TotalPages ?? 0,
                        CompletionPercentage: responseDTO?.CompletionPercentage ?? 0,
                        HasUploadedAll: responseDTO?.HasUploadedAll ?? false
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching employee info | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; //  MUST
            }
        }

    }
    #endregion
}


