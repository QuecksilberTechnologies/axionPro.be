using AutoMapper;
using axionpro.application.DTOs.Role;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class DeleteRoleQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRoleRequestDTO DTO { get; set; }

        public DeleteRoleQuery(DeleteRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeleteRoleQueryHandler : IRequestHandler<DeleteRoleQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteRoleQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteRoleQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteRoleQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
      DeleteRoleQuery request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 DeleteRole started");

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
                //    Modules.Role,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid RoleId.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                _logger.LogInformation(
                    "🗑️ Deleting RoleId: {RoleId}, TenantId: {TenantId}",
                    request.DTO.Id,
                    validation.TenantId);

                // ===============================
                // 4️⃣ DELETE
                // ===============================
                var deleted = await _unitOfWork.RoleRepository
                    .DeleteAsync(request.DTO, validation.UserEmployeeId, request.DTO.Id);

                if (!deleted)
                    throw new ApiException("Role not found or already deleted.", 404);

                _logger.LogInformation("✅ Role deleted successfully");

                // ===============================
                // 5️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Role deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ DeleteRole failed");

                throw; // ✅ CRITICAL
            }
        }
    }
}
