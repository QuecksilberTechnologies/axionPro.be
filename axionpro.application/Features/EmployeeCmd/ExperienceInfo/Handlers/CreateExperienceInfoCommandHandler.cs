using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
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
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers
{
    public class CreateExperienceInfoCommand : IRequest<ApiResponse<List<GetExperienceResponseDTO>>>
    {
        public CreateExperienceRequestDTO DTO { get; set; }

        public CreateExperienceInfoCommand(CreateExperienceRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    //public class CreateExperienceInfoCommandHandler : IRequestHandler<CreateExperienceInfoCommand, ApiResponse<List<GetExperienceResponseDTO>>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IMapper _mapper;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly ILogger<CreateExperienceInfoCommandHandler> _logger;
    //    private readonly ITokenService _tokenService;
    //    private readonly IPermissionService _permissionService;
    //    private readonly IConfiguration _config;
    //    private readonly IEncryptionService _encryptionService;
    //    private readonly IIdEncoderService _idEncoderService;
    //    private readonly IFileStorageService _fileStorageService;

    //    public CreateExperienceInfoCommandHandler(
    //        IUnitOfWork unitOfWork,
    //        IMapper mapper,
    //        IHttpContextAccessor httpContextAccessor,
    //        ILogger<CreateExperienceInfoCommandHandler> logger,
    //        ITokenService tokenService,
    //        IPermissionService permissionService,
    //        IConfiguration config,
    //        IEncryptionService encryptionService,
    //        IIdEncoderService idEncoderService,
    //        IFileStorageService fileStorageService)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _mapper = mapper;
    //        _httpContextAccessor = httpContextAccessor;
    //        _logger = logger;
    //        _tokenService = tokenService;
    //        _permissionService = permissionService;
    //        _config = config;
    //        _encryptionService = encryptionService;
    //        _idEncoderService = idEncoderService;
    //        _fileStorageService = fileStorageService;
    //    }
    //    public async Task<ApiResponse<List<GetExperienceResponseDTO>>> Handle(CreateExperienceInfoCommand request, CancellationToken cancellationToken)
    //    {
    //        string? savedFullPath = null;  // 📂 File full path track karne ke liye

    //        try
    //        {
    //            // 🔹 STEP 1: Token Validation
    //            var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
    //                .ToString()?.Replace("Bearer ", "");
    //            if (string.IsNullOrEmpty(bearerToken))
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized: Token not found.");

    //            var secretKey = TokenKeyHelper.GetJwtSecret(_config);
    //            var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
    //            if (tokenClaims == null || tokenClaims.IsExpired)
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Invalid or expired token.");

    //            // 🔹 STEP 2: Validate Active User
    //            long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
    //            if (loggedInEmpId < 1)
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized or inactive user.");

    //            // 🔹 STEP 3: Decrypt Tenant + Employee
    //            string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
    //            if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("User invalid.");
    //            string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
    //            long decryptedEmployeeId = _idEncoderService.DecodeId(EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId), finalKey);
    //            long decryptedTenantId = _idEncoderService.DecodeId(EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);
    //            string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
    //            long decryptedActualEmployeeId = _idEncoderService.DecodeId(actualEmpId, finalKey);



    //            if (decryptedTenantId <= 0 || decryptedEmployeeId <= 0)
    //            {
    //                _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Tenant or employee information missing.");
    //            }

    //            if (!(decryptedEmployeeId == loggedInEmpId))
    //            {
    //                _logger.LogWarning(
    //                    "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
    //                     decryptedEmployeeId, loggedInEmpId
    //                );

    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
    //            }


    //            // 🔹 STEP 4: File Upload
    //            string? docPath = null;
    //            string? docName = null;

    //            // 🔹 Tenant info from decoded values
    //            long? tenantId = decryptedTenantId;

    //            string docFileName = EncryptionSanitizer.CleanEncodedInput(request.DTO.BankStatementDocName.Trim().ToLower());
    //            if (docFileName != null)
    //            {
    //                // 🔹 File upload check
    //                if (request.DTO.ExperienceCertificatePDF != null && request.DTO.ExperienceCertificatePDF.Length > 0)
    //                {
    //                    using (var ms = new MemoryStream())
    //                    {
    //                        await request.DTO.ExperienceCertificatePDF.CopyToAsync(ms);
    //                        var fileBytes = ms.ToArray();

    //                        // 🔹 File naming convention (same pattern as asset)
    //                        string fileName = $"EDU-{decryptedActualEmployeeId + "_" + docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";
    //                        string folderPath = "education"; // Folder name same for all education docs

    //                        // 🔹 Generate full file path (tenant + folder + filename)
    //                        string filePath = _fileStorageService.GenerateFilePath(tenantId, folderPath, fileName);

    //                        // 🔹 Store actual name for reference in DB
    //                        docName = fileName;

    //                        // 🔹 Save file physically
    //                        savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, filePath);

    //                        // 🔹 If saved successfully, set relative path
    //                        if (!string.IsNullOrEmpty(savedFullPath))
    //                            docPath = _fileStorageService.GetRelativePath(savedFullPath);
    //                    }
    //                }

    //            }

    //            else
    //                return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Digree-Name missing.");
    //            EmployeeExperience  employeeExperience = new EmployeeExperience();

    //            PagedResponseDTO<GetExperienceResponseDTO> responseDTO = await _unitOfWork.EmployeeExpereinceRepository.CreateAsync(employeeExperience);


    //            // 4️⃣ Encrypt Ids in result
    //            var encryptedList = ProjectionHelper.ToGetExperienceResponseDTOs(responseDTO.Items, _encryptionService, tenantKey, request.DTO.EmployeeId);

    //            // 5️⃣ Commit transaction
    //            await _unitOfWork.CommitTransactionAsync();

    //            // 6️⃣ Return API response
    //            return new ApiResponse<List<GetExperienceResponseDTO>>
    //            {
    //                IsSucceeded = true,
    //                Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
    //                PageNumber = responseDTO.PageNumber,
    //                PageSize = responseDTO.PageSize,
    //                TotalRecords = responseDTO.TotalCount,
    //                TotalPages = responseDTO.TotalPages,
    //                Data = encryptedList
    //            };
    //        }
    //        catch (Exception ex)
    //        {
    //            await _unitOfWork.RollbackTransactionAsync();
    //            _logger.LogError(ex, "Error occurred while adding experience info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
    //            return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Failed to add experience info.", new List<string> { ex.Message });
    //        }
    //    }
    //}

}
