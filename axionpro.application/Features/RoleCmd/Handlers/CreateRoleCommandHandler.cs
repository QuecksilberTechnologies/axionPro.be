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
    public class CreateRoleCommand : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public CreateRoleRequestDTO DTO { get; set; }

        public CreateRoleCommand(CreateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<List<GetRoleResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateRoleCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly  IIdEncoderService _idEncoderService;
        ICommonRequestService _commonRequestService;



        public CreateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateRoleCommandHandler> logger,
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


        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 CreateRole started");

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
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ VALIDATE ROLE NAME
                // ===============================
                string? roleName = request.DTO.RoleName?.Trim();

                if (string.IsNullOrWhiteSpace(roleName))
                    throw new ValidationErrorException("Role name cannot be empty.");

                // ===============================
                // 5️⃣ CREATE ROLE
                // ===============================
                var result = await _unitOfWork.RoleRepository.CreateAsync(request.DTO);

                if (result == null || result.Data == null)
                    throw new ApiException("Role creation failed.", 500);

                var data = result.Data ?? new List<GetRoleResponseDTO>();

                _logger.LogInformation("✅ Created {Count} role(s)", data.Count);

                // ===============================
                // 6️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetRoleResponseDTO>>
                    .SuccessPaginated(
                        data: data,
                        pageNumber: result.PageNumber,
                        pageSize: result.PageSize,
                        totalRecords: result.TotalCount,
                        totalPages: result.TotalPages,
                        message: "Role(s) created successfully."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateRole failed");

                throw; // ✅ CRITICAL
            }
        }

    }
}
