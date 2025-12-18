using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class UpdateEducationInfoCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateEducationRequestDTO DTO { get; set; }
        public UpdateEducationInfoCommand(UpdateEducationRequestDTO dto) => DTO = dto;
    }


    public class UpdateEducationInfoCommandHandler : IRequestHandler<UpdateEducationInfoCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UpdateEducationInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIdEncoderService _idEncoderService;

        public UpdateEducationInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateEducationInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, IFileStorageService fileStorageService)
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


        public async Task<ApiResponse<bool>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {

                if (request.DTO.Prop == null)
                {
                    request.DTO.Prop = new ExtraPropRequestDTO();
                }

                // ------------------------ TOKEN VALIDATION ------------------------
                var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrWhiteSpace(token))
                    return ApiResponse<bool>.Fail("Unauthorized — Token missing.");

                var tokenClaims = TokenClaimHelper.ExtractClaims(token, TokenKeyHelper.GetJwtSecret(_config));
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");


                // ------------------------ VALIDATE REQUEST ------------------------
                if (string.IsNullOrWhiteSpace(request.DTO.Id))
                    return ApiResponse<bool>.Fail("Invalid education identity.");

                if (string.IsNullOrWhiteSpace(request.DTO.UserEmployeeId))
                    return ApiResponse<bool>.Fail("Invalid user mapping.");

                 string finalKey = EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantEncriptionKey);
                // request.DTO.Prop.TenantId =  SafeParser.TryParseLong(tokenClaims.TenantId);
                 request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId_long(EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId), finalKey);

                request.DTO.Prop.TenantId = _idEncoderService.DecodeId_long(EncryptionSanitizer.CleanEncodedInput( tokenClaims.TenantId), finalKey);

                if (request.DTO.Prop.TenantId <= 0 || request.DTO.Prop.UserEmployeeId <= 0)
                    return ApiResponse<bool>.Fail("Invalid decoded identity.");

                request.DTO.Prop.RowId  = SafeParser.TryParseLong(request.DTO.Id);
                if (request.DTO.Prop.RowId <= 0)
                    return ApiResponse<bool>.Fail("Invalid record reference.");


                // ------------------------ AUTHORIZATION ------------------------
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId != request.DTO.Prop.UserEmployeeId)
                    return ApiResponse<bool>.Fail("Unauthorized request — Access denied.");


                // ------------------------ FETCH EXISTING RECORD ------------------------
                var existing = await _unitOfWork.EmployeeEducationRepository.GetSingleRecordAsync(request.DTO.Prop.RowId, true);

                if (existing == null)
                    return ApiResponse<bool>.UpdatedFail("Education record not found.");




                // ------------------------ APPLY FIELD UPDATES (NULL SAFE) ------------------------
                existing.Degree = string.IsNullOrWhiteSpace(request.DTO.Degree) ? existing.Degree : request.DTO.Degree.Trim();
                existing.InstituteName = string.IsNullOrWhiteSpace(request.DTO.InstituteName) ? existing.InstituteName : request.DTO.InstituteName.Trim();
                existing.Remark = request.DTO.Remark?.Trim();
                existing.ScoreValue = request.DTO.ScoreValue?.Trim();
             //   1 = GPA, 2 = Percentage, 3 = CGPA, etc.
                if (!string.IsNullOrWhiteSpace(request.DTO.ScoreType) && int.TryParse(request.DTO.ScoreType.Trim(), out int parsedScoreType))
                {
                    existing.ScoreType = parsedScoreType;
                }
                existing.GradeDivision = request.DTO.GradeDivision?.Trim();
                existing.EducationGap = request.DTO.IsEducationGapBeforeDegree;
                existing.GapYears = request.DTO.GapYears;
                existing.ReasonOfEducationGap = request.DTO.ReasonOfEducationGap?.Trim();
                existing.StartDate = request.DTO.StartDate ?? existing.StartDate;
                existing.EndDate = request.DTO.EndDate ?? existing.EndDate;

                // ------------------------ FILE HANDLING (OPTIONAL) ------------------------
                if (request.DTO.EducationDocument is { Length: > 0 })
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(existing.FilePath))
                        {
                            string oldPath = existing.FilePath; // This may be URL or relative path

                            bool fileDeleted = false;

                            // Case 1: Physical local/server file path
                            string physicalFullPath = _fileStorageService.GetRelativePath(oldPath);

                            if (!string.IsNullOrWhiteSpace(physicalFullPath) && File.Exists(physicalFullPath))
                            {
                                File.Delete(physicalFullPath);
                                fileDeleted = true;
                                _logger.LogInformation("📌 Local/Server education document deleted: {File}", physicalFullPath);
                            }

                            // Case 2: Remote CDN/HTTP/Cloud File
                            if (!fileDeleted && Uri.TryCreate(oldPath, UriKind.Absolute, out Uri? uri)
                                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
                            {
                                using var client = new HttpClient();
                                var response = await client.DeleteAsync(uri);

                                if (response.IsSuccessStatusCode)
                                {
                                    fileDeleted = true;
                                    _logger.LogInformation("🌍 Remote education document deleted: {File}", uri);
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ Remote file delete attempt failed for: {File}", uri);
                                }
                            }

                            if (!fileDeleted)
                                _logger.LogWarning("⚠️ Delete attempted but file not found: {File}", oldPath);
                        }

                        // Now upload new file
                        using var ms = new MemoryStream();
                        await request.DTO.EducationDocument.CopyToAsync(ms);

                        string newFileName = $"EDU-{existing.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                        string folderPath = _fileStorageService.GetEmployeeFolderPath(
                            request.DTO.Prop.TenantId,
                            existing.EmployeeId,
                            "education"
                        );

                        string savedPath = await _fileStorageService.SaveFileAsync(ms.ToArray(), newFileName, folderPath);

                        existing.FilePath = _fileStorageService.GetRelativePath(savedPath);
                        existing.FileName = newFileName;
                        existing.HasEducationDocUploded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error replacing education document for employee {Emp}", existing.EmployeeId);
                        return ApiResponse<bool>.Fail("File upload failed, please try again.");
                    }

                }


                // ------------------------ SAVE CHANGES ------------------------
                existing.UpdatedById = loggedInEmpId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                await _unitOfWork.EmployeeEducationRepository.UpdateEmployeeFieldAsync(existing);


                return ApiResponse<bool>.UpdatedSuccess("Education record updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error updating education");
                return ApiResponse<bool>.Fail("Something went wrong while updating education.");
            }
        }
    }
}
