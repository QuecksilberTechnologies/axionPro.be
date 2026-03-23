using AutoMapper;
using axionpro.application.DTOS.Role;
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
    public class GetRoleOptionQuery : IRequest<ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        public GetRoleOptionRequestDTO OptionDTO { get; set; }

        public GetRoleOptionQuery(GetRoleOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    public class GetRoleOptionQueryHandler : IRequestHandler<GetRoleOptionQuery, ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetRoleOptionQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICommonRequestService _commonRequestService;


        public GetRoleOptionQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetRoleOptionQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            ICommonRequestService commonRequestService,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService)
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

        public async Task<ApiResponse<List<GetRoleOptionResponseDTO>>> Handle(
     GetRoleOptionQuery request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetRoleOption started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.OptionDTO.UserEmployeeId);

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
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.OptionDTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                request.OptionDTO.Prop ??= new();

                request.OptionDTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.OptionDTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var response = await _unitOfWork.RoleRepository
                    .GetOptionAsync(request.OptionDTO);

                var data = response?.Data ?? new List<GetRoleOptionResponseDTO>();

                _logger.LogInformation(
                    "✅ Retrieved {Count} role options for TenantId {TenantId}",
                    data.Count,
                    validation.TenantId);

                // ===============================
                // 4️⃣ SUCCESS (EMPTY ALLOWED ✅)
                // ===============================
                return ApiResponse<List<GetRoleOptionResponseDTO>>
                    .Success(data, "Role options fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetRoleOption failed");

                throw; // ✅ CRITICAL
            }
        }

    }

}
