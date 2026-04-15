using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
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

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class GetBankInfoQuery : IRequest<ApiResponse<List<GetBankResponseDTO>>>
    {
        public GetBankReqestDTO DTO { get; set; }

        public GetBankInfoQuery(GetBankReqestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetBankInfoQueryHandler : IRequestHandler<GetBankInfoQuery, ApiResponse<List<GetBankResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetBankInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService ;


        public GetBankInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBankInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
             IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService,
             IFileStorageService fileStorageService)
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
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(
      GetBankInfoQuery request,
      CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetBankInfo started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO?.UserEmployeeId);

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
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to view bank info.");

                // ===============================
                // 4️⃣ FETCH DATA
                // ===============================
                var bankEntities =
                    await _unitOfWork.EmployeeBankRepository
                        .GetInfoAsync(request.DTO);




                // ===============================
                // 5️⃣ EMPTY LIST = SUCCESS
                // ===============================
                if (bankEntities == null || bankEntities.Data == null || !bankEntities.Data.Any())
                {
                    return ApiResponse<List<GetBankResponseDTO>>
                        .SuccessPaginatedPercentage(
                            Data: new List<GetBankResponseDTO>(),
                            Message: "No bank info found.",
                            PageNumber: bankEntities?.PageNumber ?? 1,
                            PageSize: bankEntities?.PageSize ?? 0,
                            TotalRecords: bankEntities?.TotalCount ?? 0,
                            TotalPages: bankEntities?.TotalPages ?? 0,
                            HasUploadedAll: bankEntities?.HasUploadedAll ?? false,
                            CompletionPercentage: bankEntities?.CompletionPercentage ?? 0
                        );
                }

                // ===============================
                // 6️⃣ PROJECTION
                // ===============================
                var result = ProjectionHelper.ToGetBankResponseDTOs(
                    bankEntities,
                    _idEncoderService,
                    validation.Claims.TenantEncriptionKey,
                    _config, _fileStorageService
                );

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                _logger.LogInformation("GetBankInfo success");

                return ApiResponse<List<GetBankResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: result,
                        Message: "Bank info retrieved successfully.",
                        PageNumber: bankEntities.PageNumber,
                        PageSize: bankEntities.PageSize,
                        TotalRecords: bankEntities.TotalCount,
                        TotalPages: bankEntities.TotalPages,
                        HasUploadedAll: bankEntities.HasUploadedAll,
                        CompletionPercentage: bankEntities.CompletionPercentage
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching bank info | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; // 🚨 MUST
            }
        }
    }


}
 