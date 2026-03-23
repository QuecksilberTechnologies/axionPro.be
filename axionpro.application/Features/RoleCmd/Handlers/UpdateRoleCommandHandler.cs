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
    public class UpdateRoleCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateRoleRequestDTO DTO { get; set; }

        public UpdateRoleCommand(UpdateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService; // IIdEncoderService idEncoderService
        ICommonRequestService _commonRequestService;

      
        public UpdateRoleCommandHandler(
               
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateRoleCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)

            {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
           
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
      UpdateRoleCommand request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 UpdateRole started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK (🔥 YOUR RULE)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Role,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid Role Id.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ FIELD VALIDATION
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.RoleName))
                    throw new ValidationErrorException("Role name cannot be empty.");

                request.DTO.RoleName = request.DTO.RoleName.Trim();

                // ===============================
                // 5️⃣ UPDATE
                // ===============================
                var updated = await _unitOfWork.RoleRepository.UpdateAsync(request.DTO);

                if (!updated)
                    throw new ApiException("Role not found or no changes detected.", 404);

                _logger.LogInformation(
                    "✅ Role updated successfully. RoleId {RoleId}, UpdatedBy {EmpId}",
                    request.DTO.Id,
                    validation.UserEmployeeId);

                // ===============================
                // 6️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>
                    .Success(true, "Role updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UpdateRole failed");

                throw; // ✅ CRITICAL
            }
        }
    }
}
