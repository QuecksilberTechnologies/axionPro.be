using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEmail;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers
{
    public class GetAllCategoryCommand : IRequest<ApiResponse<PagedResponseDTO<GetCategoryResponseDTO>>>
    {
        public GetCategoryReqestDTO DTO { get; set; }

        public GetAllCategoryCommand(GetCategoryReqestDTO dto)
        {
            DTO = dto;
        }
    }
    /// <summary>
    /// Handles fetching all Asset Categories for a given tenant.
    /// </summary>
    public class GetAllCategoryCommandHandler
        : IRequestHandler<GetAllCategoryCommand, ApiResponse<PagedResponseDTO<GetCategoryResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllCategoryCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;


        public GetAllCategoryCommandHandler(IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetAllCategoryCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService ,
            IHttpContextAccessor _httpContextAccessor, IEmailService emailService
)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _configuration = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _emailService = emailService;
        }

        /// <summary>
        /// Handles the GetAllCategoryCommand request to retrieve all categories.
        /// </summary>
        public async Task<ApiResponse<PagedResponseDTO<GetCategoryResponseDTO>>> Handle( GetAllCategoryCommand request,
     CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Categories");

                // ===============================
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw (middleware handle karega)
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetCategory",   // 🔹 Module
                //    "View"             // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to view asset categories.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var categoryEntities = await _unitOfWork.AssetCategoryRepository
                    .GetAllAsync(request.DTO);

                // ===============================
                // 5️⃣ HANDLE EMPTY DATA (IMPORTANT)
                // ===============================
                if (categoryEntities == null || categoryEntities.Data.Count == 0)
                {
                    _logger.LogWarning(
                        "No Asset Categories found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    // ✅ Empty list = success (best practice)
                    return ApiResponse<PagedResponseDTO<GetCategoryResponseDTO>>
                        .Success(new PagedResponseDTO<GetCategoryResponseDTO>
                        {
                            Data = new List<GetCategoryResponseDTO>(),
                            TotalCount = 0
                        }, "No Asset Categories found.");
                }

                // ===============================
                // 6️⃣ MAP ENTITY → DTO
                // ===============================
                var responseDTOs = _mapper.Map<List<GetCategoryResponseDTO>>(categoryEntities);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Categories for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                return ApiResponse<PagedResponseDTO<GetCategoryResponseDTO>>
                    .Success(new PagedResponseDTO<GetCategoryResponseDTO>
                    {
                        Data = responseDTOs,
                        TotalCount = responseDTOs.Count,                       
                        PageNumber = request.DTO.PageNumber,
                        PageSize = request.DTO.PageSize
                    }, "Asset Categories fetched successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Categories for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
