using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.StatsFeatures.EmployeesCmd.Handlers
{


    public class GetEmployeeCountsQuery : IRequest<ApiResponse<EmployeeCountResponseStatsSp>>
    {
        public EmployeeCountRequestStatsSp DTO { get; }

        public GetEmployeeCountsQuery(EmployeeCountRequestStatsSp dTO)
        {
            DTO = dTO;
        }
    }
    public class GetEmployeesCountQueryHandler: IRequestHandler<GetEmployeeCountsQuery, ApiResponse<EmployeeCountResponseStatsSp>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeesCountQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeesCountQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeesCountQueryHandler> logger,
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

        public async Task<ApiResponse<EmployeeCountResponseStatsSp>> Handle(
    GetEmployeeCountsQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetEmployeeCounts started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 4️⃣ FETCH SP DATA
                // ===============================
                var result = await _unitOfWork
                    .StoreProcedureRepository
                    .GetEmployeeCountsAsync(validation.TenantId);

                if (result == null)
                {
                    _logger.LogInformation("⚠️ No employee stats found for TenantId {TenantId}", validation.TenantId);

                    return ApiResponse<EmployeeCountResponseStatsSp>
                        .Success(null, "No data found.");
                }

                _logger.LogInformation("✅ Employee stats retrieved successfully");

                // ===============================
                // 5️⃣ SUCCESS
                // ===============================
                return ApiResponse<EmployeeCountResponseStatsSp>
                    .Success(result, "Employee statistics retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetEmployeeCounts failed");

                throw; // ✅ CRITICAL
            }
        }
    }








}
