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

        public async Task<ApiResponse<GetEmployeeCodePatternResponseDTO>> Handle(
     GetEmployeeCodePatternQuery request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetEmployeeCodePattern started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

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

                request.DTO.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var pattern = await _unitOfWork
                    .TenantEmployeeCodePatternRepository
                    .GetTenantEmployeeCodePatternAsync(
                        request.DTO.TenantId,
                        request.DTO.IsActive);

                if (pattern == null)
                {
                    _logger.LogInformation(
                        "⚠️ No employee code pattern found for TenantId {TenantId}",
                        validation.TenantId);

                    return ApiResponse<GetEmployeeCodePatternResponseDTO>
                        .Success(null, "No pattern found.");
                }

                _logger.LogInformation("✅ Employee code pattern retrieved");

                // ===============================
                // 5️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetEmployeeCodePatternResponseDTO>
                    .Success(pattern, "Pattern fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ GetEmployeeCodePattern failed");

                throw; // ✅ CRITICAL
            }
        }

    }
}

