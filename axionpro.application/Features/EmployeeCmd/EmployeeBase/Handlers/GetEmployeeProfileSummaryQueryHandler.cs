using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    // =====================================================
    // QUERY
    // =====================================================
    public class GetEmployeeProfileSummaryQuery
        : IRequest<ApiResponse<EmployeeProfileSummaryInfo>>
    {
        public GetEmployeeSummaryRequestDTO DTO { get; }

        public GetEmployeeProfileSummaryQuery(GetEmployeeSummaryRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // =====================================================
    // HANDLER
    // =====================================================
    public class GetEmployeeProfileSummaryQueryHandler
        : IRequestHandler<GetEmployeeProfileSummaryQuery, ApiResponse<EmployeeProfileSummaryInfo>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeeProfileSummaryQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeeProfileSummaryQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeeProfileSummaryQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<EmployeeProfileSummaryInfo>> Handle(
      GetEmployeeProfileSummaryQuery request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetEmployeeProfileSummary started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                // ===============================
                // 3️⃣ DECODE EMPLOYEE ID
                // ===============================
                var employeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 4️⃣ PERMISSION (YOUR PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to view employee summary.");

                // ===============================
                // 5️⃣ FETCH SUMMARY
                // ===============================
                var summary =
                    await _unitOfWork.Employees.EmployeeProfileSummaryAsync(
                        employeeId,
                        request.DTO.IsActive);

                if (summary == null)
                {
                    _logger.LogInformation(
                        "No Employee Profile Summary found | EmployeeId: {EmployeeId}",
                        employeeId);

                    throw new ApiException("Employee profile summary not found.", 404);
                }

                // ===============================
                // 6️⃣ PROJECTION
                // ===============================
                var response =
                    ProjectionHelper.ToGetProfileSummaryResponseDTO(
                        summary,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey);

                _logger.LogInformation("GetEmployeeProfileSummary success");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<EmployeeProfileSummaryInfo>
                    .Success(
                        response,
                        "Employee profile summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching employee profile summary | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; // 🚨 MUST
            }
        }
    }
}
