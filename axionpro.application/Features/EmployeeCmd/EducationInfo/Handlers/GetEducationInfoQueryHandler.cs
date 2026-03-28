using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class GetEducationInfoQuery : IRequest<ApiResponse<List<GetEducationResponseDTO>>>
    {
        public GetEducationRequestDTO DTO { get; set; }

        public GetEducationInfoQuery(GetEducationRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetEducationInfoQueryHandler : IRequestHandler<GetEducationInfoQuery, ApiResponse<List<GetEducationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetEducationInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IConfiguration _configuration;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;


        public GetEducationInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetEducationInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
          IEncryptionService encryptionService, IIdEncoderService idEncoderService, IConfiguration configuration, ICommonRequestService commonRequestService, IFileStorageService fileStorageService)
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
            this._configuration = configuration;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
        }


        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(
    GetEducationInfoQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetEducationInfo started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (YOUR FIXED PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to view education.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var entity =
                    await _unitOfWork.EmployeeEducationRepository
                        .GetInfo(request.DTO);

                // ===============================
                // 5️⃣ OPTIMIZED EMPTY HANDLING
                // ===============================
                var items = entity?.Items ?? new List<GetEducationResponseDTO>();

                var responseDTO = items.Any()
                    ? ProjectionHelper.ToGetEducationResponseDTOs(
                        entity,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _configuration, _fileStorageService)
                    : new List<GetEducationResponseDTO>();

                _logger.LogInformation("GetEducationInfo success");

                // ===============================
                // 6️⃣ SINGLE RESPONSE
                // ===============================
                return ApiResponse<List<GetEducationResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: responseDTO,
                        Message: items.Any()
                            ? "Education info retrieved successfully."
                            : "No education info found.",
                        PageNumber: entity?.PageNumber ?? 1,
                        PageSize: entity?.PageSize ?? 0,
                        TotalRecords: entity?.TotalCount ?? 0,
                        TotalPages: entity?.TotalPages ?? 0,
                        CompletionPercentage: entity?.CompletionPercentage ?? 0,
                        HasUploadedAll: entity?.HasUploadedAll ?? false
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching education | EmployeeId: {EmployeeId}",
                    request.DTO?.UserEmployeeId);

                throw; // 🚨 MUST
            }
        }

    }
}
