using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers
{
    #region Query Definition
    public class GetIdentityInfoQuery : IRequest<ApiResponse<GetIdentityResponseDTO>>
    {
        public GetIdentityRequestDTO DTO { get; }

        public GetIdentityInfoQuery(GetIdentityRequestDTO dto)
        {
            DTO = dto ?? throw new ArgumentNullException(nameof(dto));
        }
    }
    #endregion

    #region Query Handler
    public class GetIdentityInfoQueryHandler : IRequestHandler<GetIdentityInfoQuery, ApiResponse<GetIdentityResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetIdentityInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;


        public GetIdentityInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetIdentityInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
          IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService
)
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

        public async Task<ApiResponse<GetIdentityResponseDTO>> Handle(GetIdentityInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // 1️⃣ Common validation
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<GetIdentityResponseDTO>
                        .Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("PersonalInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                // 4️⃣ Fetch Data from Repository
                // 4️⃣ Fetch from repository
                var entity = await _unitOfWork.EmployeeIdentityRepository.GetInfo(request.DTO);

                if (entity == null)
                {
                    _logger.LogInformation("No Identity Info found for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);
                    return ApiResponse<GetIdentityResponseDTO>.Fail("No identity info found.");
                }


                var result = ProjectionHelper.ToGetIdentityResponseDTO(entity, _idEncoderService, validation.Claims.TenantEncriptionKey);


                _logger.LogInformation("Successfully fetched identity info for EmployeeId: {EmployeeId}", request.DTO.EmployeeId);

                return ApiResponse<GetIdentityResponseDTO>.Success(result, "Identity info retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Identity Info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<GetIdentityResponseDTO>.Fail(
                    "An unexpected error occurred while fetching identity info.",
                    new List<string> { ex.Message }
                );
            }
        }
    }
    #endregion
}
