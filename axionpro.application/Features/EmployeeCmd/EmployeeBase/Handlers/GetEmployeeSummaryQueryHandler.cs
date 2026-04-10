using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.Handlers
{
    public class GetEmployeeSummaryQuery : IRequest<ApiResponse<SummaryEmployeeInfo>>
    {
        public GetEmployeeSummaryRequestDTO DTO { get; }

        public GetEmployeeSummaryQuery(GetEmployeeSummaryRequestDTO dTO)
        {
            DTO = dTO;
        }
        
    }
    public class GetEmployeeSummaryQueryHandler
       : IRequestHandler<GetEmployeeSummaryQuery, ApiResponse<SummaryEmployeeInfo>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeeSummaryQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;



        public GetEmployeeSummaryQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeeSummaryQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            IConfiguration config, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _config = config;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<SummaryEmployeeInfo>> Handle(
    GetEmployeeSummaryQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetEmployeeSummary started");

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
                    await _unitOfWork.Employees
                        .BuildEmployeeSummaryAsync(
                            employeeId,
                            request.DTO.IsActive);

                if (summary == null)
                {
                    _logger.LogInformation(
                        "No Employee Summary found | EmployeeId: {EmployeeId}",
                        employeeId);

                    throw new ApiException("Employee summary not found.", 404);
                }

                // ===============================
                // 6️⃣ PROJECTION
                // ===============================
                var result =
                    ProjectionHelper.ToGetSummaryResponseDTO(
                        summary,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _config, _fileStorageService);

                _logger.LogInformation("GetEmployeeSummary success");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<SummaryEmployeeInfo>
                    .Success(
                        result,
                        "Employee summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching employee summary | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; // 🚨 MUST
            }
        }
    }

}
