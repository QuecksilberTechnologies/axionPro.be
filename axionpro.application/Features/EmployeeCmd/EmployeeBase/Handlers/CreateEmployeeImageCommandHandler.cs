using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class CreateEmployeeImageCommand : IRequest<ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        public CreateEmployeeImageRequestDTO DTO { get; set; }

        public CreateEmployeeImageCommand(CreateEmployeeImageRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateEmployeeImageCommandHandler : IRequestHandler<CreateEmployeeImageCommand, ApiResponse<List<GetEmployeeImageReponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateEmployeeImageCommand> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public CreateEmployeeImageCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateEmployeeImageCommand> logger,
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

        public async Task<ApiResponse<List<GetEmployeeImageReponseDTO>>> Handle(CreateEmployeeImageCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            string? savedFullPath = null;  // 📂 File full path track karne ke liye

            try
            {
                // 🔹 STEP 1: Token Validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Invalid or expired token.");

                // 🔹 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized or inactive user.");

                // 🔹 STEP 3: Decrypt Tenant + Employee
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("User invalid.");
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                long decryptedEmployeeId = _idEncoderService.DecodeId_long(EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId), finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId_long(EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                long decryptedActualEmployeeId = _idEncoderService.DecodeId_long(actualEmpId, finalKey);



                if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(decryptedEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         decryptedEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }


                // 🔹 STEP 4: File Upload
                string? FilePath = null;
                string? FileName = null;
                bool HasImageUploaded = false;
                // 🔹 Tenant info from decoded values
                long tenantId = decryptedTenantId;
                int ImageType = 0;

                string docFileName = EncryptionSanitizer.CleanEncodedInput(actualEmpId.Trim().ToLower());

                if (docFileName != null)
                {
                    // 🔹 File upload check
                    if (request.DTO.ImageFile != null && request.DTO.ImageFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await request.DTO.ImageFile.CopyToAsync(ms);
                            var fileBytes = ms.ToArray();

                            // 🔹 File naming convention (same pattern as asset)
                            string fileName = $"ProfileImage-{decryptedActualEmployeeId + "_" + docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                            string fullFolderPath = _fileStorageService.GetEmployeeFolderPath(tenantId, decryptedActualEmployeeId, "profile");

                            // 🔹 Store actual name for reference in DB
                            FileName = fileName;

                            // 🔹 Save file physically
                            savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, fullFolderPath);

                            // 🔹 If saved successfully, set relative path
                            if (!string.IsNullOrEmpty(savedFullPath))
                            {
                                FilePath = _fileStorageService.GetRelativePath(savedFullPath);

                                HasImageUploaded = true;
                                ImageType = 1;


                            }


                        }
                    }

                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data

                // 3️⃣ DTO Configuration
                var entity = _mapper.Map<EmployeeImage>(request.DTO);

                entity.EmployeeId = decryptedActualEmployeeId;
                entity.AddedById = decryptedEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = request.DTO.IsActive;
                entity.FileType = HasImageUploaded ? ImageType : 0;

                if (HasImageUploaded)
                {
                    entity.FilePath = FilePath;
                    entity.FileName = FileName;
                }

                entity.HasImageUploaded = HasImageUploaded;

                


                //   HttpRequestOptionsKey

                // 4️⃣ Repository Operation
                var responseDTO = await _unitOfWork.Employees.AddImageAsync(entity);
                 
                // 5️⃣ Encrypt Result Data
                    var encryptedList = ProjectionHelper.ToGetProfileImageInfoResponseDTOs(responseDTO.Items, _idEncoderService, tenantKey);

                // 6️⃣ Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 7️⃣ Final API Response
                return new ApiResponse<List<GetEmployeeImageReponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    CompletionPercentage = responseDTO.CompletionPercentage,
                    Data = encryptedList,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while adding base employee info");
                return ApiResponse<List<GetEmployeeImageReponseDTO>>.Fail("Failed to add base employee info.", new List<string> { ex.Message });
            }
        }
    }
}
