using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Token;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IHashed;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserLoginAndcDsasxhboardCmd.Handlers
{
    public class GetNewLoginPasswordURLCommand : IRequest<ApiResponse<GetNewPasswordLinkResponseDTO>>
    {
        public SetNewPasswordLinkRequestDTO DTO { get; set; }

        public GetNewLoginPasswordURLCommand(SetNewPasswordLinkRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetNewLoginPasswordURLCommandHandler
        : IRequestHandler<GetNewLoginPasswordURLCommand, ApiResponse<GetNewPasswordLinkResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetNewLoginPasswordURLCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPasswordService _passwordService;

        public GetNewLoginPasswordURLCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetNewLoginPasswordURLCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IPasswordService passwordService,
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
            _passwordService = passwordService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<GetNewPasswordLinkResponseDTO>> Handle(
            GetNewLoginPasswordURLCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.DTO;

                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<GetNewPasswordLinkResponseDTO>
                        .Fail(validation.ErrorMessage);
                long empId = await _unitOfWork.StoreProcedureRepository.ValidateActiveUserLoginOnlyAsync(request.DTO.UserLoginId);
                _logger.LogInformation("Validation result for LoginId {LoginId}: EmployeeId = {empId}", request.DTO.UserLoginId, empId);

                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for UserLoginId: {LoginId}", request.DTO.UserLoginId);
                    // await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<GetNewPasswordLinkResponseDTO>.Fail("User is not authenticated or authorized to perform this action.");
                }

                // 2️⃣ Get employee
                GetMinimalEmployeeResponseDTO emp =
                    await _unitOfWork.Employees.GetSingleRecordAsync(empId, true);

                if (emp == null)
                    return ApiResponse<GetNewPasswordLinkResponseDTO>.Fail("Employee not found.");

                // 3️⃣ Encode IDs
                string encryptedEmployeeId =
                    _idEncoderService.EncodeId_long(emp.Id, null);

                string encryptedTenantId =
                    _idEncoderService.EncodeId_long(emp.TenantId, null);

                // 4️⃣ Token Info
                var tokenInfo = new GetTokenInfoDTO
                {
                    EmployeeId = encryptedEmployeeId,
                    TenantId = encryptedTenantId,
                    Email = dto.UserLoginId,
                    FullName = emp.FirstName ?? "",
                    TokenPurpose = _idEncoderService.EncodeId_int(ConstantValues.SetPassword, ""),
                    IssuedAt = DateTime.UtcNow,
                    Expiry = DateTime.UtcNow.AddMinutes(30),
                    IsFirstLogin = false,
                    ClientType = "Web"
                };

                // 5️⃣ Generate token
                var token = _tokenService.GenerateToken(tokenInfo);

                // 6️⃣ Build URL
                var baseUrl = _config["FrontEndWebURL:BaseUrl"] ?? string.Empty;

                var resetUrl = $"{baseUrl}/auth/set-password?token={token.Result}";
                               
                var response = new GetNewPasswordLinkResponseDTO
                {
                    UrlLink = resetUrl
                };

                return ApiResponse<GetNewPasswordLinkResponseDTO>
                    .Success(response, "Password link generated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating password reset URL");

                return ApiResponse<GetNewPasswordLinkResponseDTO>
                    .Fail("Failed to generate password reset link.");
            }
        }
    }
}