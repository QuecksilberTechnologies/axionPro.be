using axionpro.application.DTOS.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers
{
    // ======================= QUERY ============================
    public class GetEmployeeCodePatternQuery
        : IRequest<ApiResponse<GetEmployeeCodePatternResponseDTO>>
    {
        public EmployeeCodePatternRequestDTO DTO { get; }

        public GetEmployeeCodePatternQuery(EmployeeCodePatternRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================= HANDLER ============================
    public class GetEmployeeCodePatternQueryHandler
        : IRequestHandler<GetEmployeeCodePatternQuery, ApiResponse<GetEmployeeCodePatternResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetEmployeeCodePatternQueryHandler> _logger;

        public GetEmployeeCodePatternQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetEmployeeCodePatternQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        public async Task<ApiResponse<GetEmployeeCodePatternResponseDTO>> Handle( GetEmployeeCodePatternQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "GetEmployeeCodePattern started.");

                // ===============================
                // 1️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                {
                    throw new ValidationErrorException(
                        "Invalid request data.");
                }

                // ===============================
                // 2️⃣ TENANT OR HOST VALIDATION
                // ===============================
                var tenantValidation =
                    await _commonRequestService.ValidateTenantUserRequestAsync();

                long targetTenantId;

                if (tenantValidation.Success)
                {
                    // Tenant users can access only their own tenant.
                    targetTenantId = tenantValidation.TenantId;

                    // ===============================
                    // 2️⃣ PERMISSION CHECK (RBAC)
                    // ===============================
                    //var hasAccess = await _permissionService.HasAccessAsync(
                    //    validation.RoleId,
                    //    Modules.Employee,
                    //    Operations.View);

                    //if (!hasAccess)
                    //    throw new UnauthorizedAccessException("Access denied.");
                }
                else
                {
                    targetTenantId= request.DTO.TenantId; // Use the provided TenantId for Host users.
                    var hostValidation =
                        await _commonRequestService.ValidateHostUserRequestAsync();

                    if (hostValidation<0)
                    {
                        throw new UnauthorizedAccessException(
                            "A valid Tenant or Host access token is required.");
                    }

                    if (request.DTO.TenantId <= 0)
                    {
                        throw new ValidationErrorException(
                            "TenantId is required for a Host user request.");
                    }

                    targetTenantId = request.DTO.TenantId;
                }

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var pattern = await _unitOfWork
                    .TenantEmployeeCodePatternRepository
                    .GetTenantEmployeeCodePatternAsync(
                        targetTenantId,
                        request.DTO.IsActive);

                if (pattern == null)
                {
                    _logger.LogInformation(
                        "No employee code pattern found for TenantId {TenantId}.",
                        targetTenantId);

                    return ApiResponse<GetEmployeeCodePatternResponseDTO>
                        .Success(null, "No pattern found.");
                }

                _logger.LogInformation(
                    "Employee code pattern retrieved for TenantId {TenantId}.",
                    targetTenantId);

                return ApiResponse<GetEmployeeCodePatternResponseDTO>
                    .Success(
                        pattern,
                        "Pattern fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "GetEmployeeCodePattern failed.");

                throw;
            }
        }

    }
}

