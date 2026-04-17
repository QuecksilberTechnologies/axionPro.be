using AutoMapper;
using axionpro.application.DTOS.AssetDTO.asset;
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
    public class GetAllCategoryCommand : IRequest<ApiResponse<List<GetCategoryResponseDTO>>>
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
        : IRequestHandler<GetAllCategoryCommand, ApiResponse<List<GetCategoryResponseDTO>>>
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
        public async Task<ApiResponse<List<GetCategoryResponseDTO>>> Handle(
  GetAllCategoryCommand request,
  CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Asset Categories");

                // ✅ VALIDATION
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ✅ FETCH DATA
                var pagedResult = await _unitOfWork.AssetCategoryRepository
                    .GetAllAsync(request.DTO);

                // ✅ EMPTY CASE
                if (pagedResult == null || pagedResult.Data == null || !pagedResult.Data.Any())
                {
                    _logger.LogWarning(
                        "No Asset Categories found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return ApiResponse<List<GetCategoryResponseDTO>>.SuccessPaginatedPercentage(
                        Data: new List<GetCategoryResponseDTO>(),
                        PageNumber: request.DTO.PageNumber,
                        PageSize: request.DTO.PageSize,
                        TotalRecords: 0,
                        TotalPages: 0,
                        Message: "No Asset Categories found.",
                        HasUploadedAll: null,
                        CompletionPercentage: null
                    );
                }

                // ✅ MAP CORRECTLY (IMPORTANT 🔥)
                var responseDTOs = _mapper.Map<List<GetCategoryResponseDTO>>(pagedResult.Data);

                _logger.LogInformation(
                    "Successfully retrieved {Count} Asset Categories for TenantId: {TenantId}",
                    responseDTOs.Count,
                    request.DTO.Prop.TenantId);

                // ✅ FINAL RESPONSE (YOUR PATTERN)
                return ApiResponse<List<GetCategoryResponseDTO>>.SuccessPaginatedPercentage(
                    Data: responseDTOs,
                    PageNumber: pagedResult.PageNumber,
                    PageSize: pagedResult.PageSize,
                    TotalRecords: pagedResult.TotalCount,
                    TotalPages: pagedResult.TotalPages,
                    Message: "Asset Categories fetched successfully.",
                    HasUploadedAll: pagedResult.HasUploadedAll,
                    CompletionPercentage: pagedResult.CompletionPercentage
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching Asset Categories for TenantId: {TenantId}",
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
