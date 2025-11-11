using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class CreateEducationInfoCommand : IRequest<ApiResponse<List<GetEducationResponseDTO>>>
    {
        public CreateEducationRequestDTO DTO { get; }

        public CreateEducationInfoCommand(CreateEducationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateEducationInfoCommandHandler : IRequestHandler<CreateEducationInfoCommand, ApiResponse<List<GetEducationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateEducationInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public CreateEducationInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateEducationInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
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
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(CreateEducationInfoCommand request, CancellationToken cancellationToken)
        {
            string? savedFullPath = null;  // 📂 File full path track karne ke liye

            try
            {
                // 🔹 STEP 1: Token Validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Invalid or expired token.");

                // 🔹 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Unauthorized or inactive user.");

                // 🔹 STEP 3: Decrypt Tenant + Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("User invalid.");

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                long decryptedEmployeeId = _idEncoderService.DecodeId(EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId), finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);

                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Tenant or employee info missing.");

                // 🔹 STEP 4: File Upload
                string? docPath = null;
                string? docName = null;

                if (request.DTO.EducationDocument != null && request.DTO.EducationDocument.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await request.DTO.EducationDocument.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();

                        string fileName = $"EDU-{decryptedEmployeeId}-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
                        string folderPath = "education";
                        string filePath = _fileStorageService.GenerateFilePath(decryptedTenantId, folderPath, fileName);
                        docName = filePath;
                        savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, filePath);

                        if (!string.IsNullOrEmpty(savedFullPath))
                            docPath = _fileStorageService.GetRelativePath(savedFullPath);
                    }
                }


                // 🔹 STEP 5: Map DTO to Entity
                var educationEntity = _mapper.Map<EmployeeEducation>(request.DTO);
                 
                 docName = _idEncoderService.DecodeString(EncryptionSanitizer.CleanEncodedInput(docName), finalKey);

                educationEntity.EmployeeId = decryptedEmployeeId;
                educationEntity.EducationDocPath = docPath;
                educationEntity.AddedById = decryptedEmployeeId;
                educationEntity.AddedDateTime = DateTime.UtcNow;
                educationEntity.IsActive = true;
                educationEntity.IsInfoVerified = false;
                educationEntity.IsEditAllowed = true;
                educationEntity.DocName = docName;
                educationEntity.DocType = 2;


                // 🔹 STEP 6: Database Insert + File Validation
                var responseDTO = await _unitOfWork.EmployeeEducationRepository.CreateAsync(educationEntity);

                if (responseDTO == null || responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    // ❌ File save ho gaya lekin data save nahi hua → file delete karo
                    if (!string.IsNullOrEmpty(savedFullPath))
                    {
                        try
                        {
                            File.Delete(savedFullPath);
                            _logger.LogWarning("🗑️ File deleted due to DB failure: {Path}", savedFullPath);
                        }
                        catch (Exception delEx)
                        {
                            _logger.LogError(delEx, "❌ Failed to delete file after DB failure.");
                        }
                    }

                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Education info insert failed. File reverted.");
                }

                // 🔹 STEP 7: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 🔹 STEP 8: Projection + Encryption
                var encryptedList = ProjectionHelper.ToGetEducationResponseDTOs(responseDTO.Items, _encryptionService, tenantKey, request.DTO.EmployeeId);

                return ApiResponse<List<GetEducationResponseDTO>>.Success(
                    encryptedList,
                    $"{responseDTO.TotalCount} record(s) created successfully."
                );
            }
            catch (Exception ex)
            {
                // ❌ Exception aane par rollback aur file cleanup
                await _unitOfWork.RollbackTransactionAsync();

                if (!string.IsNullOrEmpty(savedFullPath) && File.Exists(savedFullPath))
                {
                    try
                    {
                        File.Delete(savedFullPath);
                        _logger.LogWarning("🗑️ File deleted due to exception rollback: {Path}", savedFullPath);
                    }
                    catch (Exception delEx)
                    {
                        _logger.LogError(delEx, "❌ Failed to delete file during rollback.");
                    }
                }

                _logger.LogError(ex, "❌ Error while creating education info");
                return ApiResponse<List<GetEducationResponseDTO>>.Fail("Internal server error while creating education info.");
            }
        }
    }
}
