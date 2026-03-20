using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.Handlers
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
                // =====================================================
                // 🔐 STEP 1: COMMON VALIDATION
                // =====================================================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<EmployeeProfileSummaryInfo>
                        .Fail(validation.ErrorMessage);

                // =====================================================
                // 🔓 STEP 2: DECODE EMPLOYEE ID
                // =====================================================
                long employeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    return ApiResponse<EmployeeProfileSummaryInfo>
                        .Fail("Invalid EmployeeId.");

                // =====================================================
                // 🔑 STEP 3: PERMISSION CHECK (SOFT)
                // =====================================================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("ViewEmployeeSummary"))
                {
                    // optional strict stop
                    // return ApiResponse<EmployeeProfileSummaryInfo>.Fail("Permission denied.");
                }

                // =====================================================
                // 📦 STEP 4: FETCH SUMMARY
                // =====================================================
                var summary =
                    await _unitOfWork.Employees.EmployeeProfileSummaryAsync(
                        employeeId,
                        request.DTO.IsActive);

                if (summary == null)
                {
                    _logger.LogInformation(
                        "No Employee Profile Summary found | EmployeeId: {EmployeeId}",
                        employeeId);

                    return ApiResponse<EmployeeProfileSummaryInfo>
                        .Fail("No employee profile summary found.");
                }

                // =====================================================
                // 🔁 STEP 5: ENCODE & CLEAN RESPONSE
                // =====================================================
                var response =
                    ProjectionHelper.ToGetProfileSummaryResponseDTO(
                        summary,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey);

                // =====================================================
                // ✅ STEP 6: SUCCESS
                // =====================================================
                return ApiResponse<EmployeeProfileSummaryInfo>
                    .Success(
                        response,
                        "Employee profile summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching Employee Profile Summary | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                return ApiResponse<EmployeeProfileSummaryInfo>
                    .Fail(
                        "An unexpected error occurred.",
                        new List<string> { ex.Message });
            }
        }
    }
}
