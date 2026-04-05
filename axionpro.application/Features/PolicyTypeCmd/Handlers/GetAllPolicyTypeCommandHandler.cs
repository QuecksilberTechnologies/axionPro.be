using axionpro.application.DTOs.PolicyType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetAllPolicyTypeCommand
        : IRequest<ApiResponse<List<GetAllPolicyTypeResponseDTO>>>
    {
        public GetAllPolicyTypeRequestDTO DTO { get; }

        public GetAllPolicyTypeCommand(GetAllPolicyTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================================================
    // 🔹 HANDLER
    // ======================================================
    public class GetAllPolicyTypeCommandHandler
        : IRequestHandler<
            GetAllPolicyTypeCommand,
            ApiResponse<List<GetAllPolicyTypeResponseDTO>>>
    {
        private readonly IPolicyTypeRepository _policyTypeRepository;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreatePolicyTypeCommandHandler> _logger;
        IPermissionService permissionService;
        public GetAllPolicyTypeCommandHandler(
            IPolicyTypeRepository policyTypeRepository,
            ICommonRequestService commonRequestService, ILogger<CreatePolicyTypeCommandHandler> logger, IPermissionService permissionService)
        {
            _policyTypeRepository = policyTypeRepository;
            _commonRequestService = commonRequestService;
            _logger = logger;            
            this.permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetAllPolicyTypeResponseDTO>>> Handle(
    GetAllPolicyTypeCommand request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetAllPolicyType started");

                // ===============================
                // 1️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                // ===============================
                // 2️⃣ AUTH VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.PolicyType,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var result =
                    await _policyTypeRepository.GetAllPolicyTypesAsync(
                        validation.TenantId,
                        request.DTO.IsActive,request.DTO.PolicyTypeEnumVal
                    );

                var data = result?.Data ?? new List<GetAllPolicyTypeResponseDTO>();

                _logger.LogInformation("✅ Retrieved {Count} policy types", data.Count);

                // ===============================
                // 5️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetAllPolicyTypeResponseDTO>>
                    .Success(data, "All policies fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllPolicyType");

                throw; // ✅ CRITICAL
            }
        }
    }
}
