using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.Interfaces;
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
       

        public CreateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateRoleCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
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
        }


        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);
                //string Id = EncryptionSanitizer.CleanEncodedInput(request.DTO.Id);
             //   request.DTO.Id = (_idEncoderService.DecodeString(Id, finalKey)).ToString();


                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetRoleResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                

                // ✅ Validate Role Name
                string? roleName = request.DTO.RoleName?.Trim();
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Role name should not be empty or whitespace.",
                        Data = null
                    };
                }

                // 🧩 STEP 5: Repository call
                var responseDTO = await _unitOfWork.RoleRepository.CreateAsync(request.DTO, decryptedTenantId, decryptedEmployeeId);


                // 🧩 STEP 6: Validate response
                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogWarning("Role creation failed or no role returned. TenantId: {TenantId}", decryptedTenantId);

                    return new ApiResponse<List<GetRoleResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No role was created. Please check input details or try again.",
                        Data = null
                    };
                }
             //   var encryptedList = ProjectionHelper.ToGetRoleResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);


                // 🧩 STEP 7: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // ✅ Return success response
                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.PageSize} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role(s) for TenantId");

                return new ApiResponse<List<GetRoleResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to create role(s) due to an internal error.",
                    Data = null
                };
            }
        }
    }
}
