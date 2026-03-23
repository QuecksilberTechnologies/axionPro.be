using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Education;
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
        private readonly ICommonRequestService _commonRequestService;


        public UpdateEducationInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UpdateEducationInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, IFileStorageService fileStorageService, ICommonRequestService commonRequestService)
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
            _commonRequestService = commonRequestService;
        }


        public async Task<ApiResponse<bool>> Handle(
       UpdateEducationInfoCommand request,
       CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("UpdateEducation started");

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

                request.DTO.Prop ??= new ExtraPropRequestDTO();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION (YOUR FIXED PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update education.");

                // ===============================
                // 4️⃣ FETCH EXISTING
                // ===============================
                var existing =
                    await _unitOfWork.EmployeeEducationRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (existing == null)
                    throw new ApiException("Education record not found.", 404);

                // ===============================
                // 5️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var dto = request.DTO;

                // ===============================
                // 6️⃣ APPLY UPDATES
                // ===============================
                if (!string.IsNullOrWhiteSpace(dto.Degree))
                    existing.Degree = dto.Degree.Trim();

                if (!string.IsNullOrWhiteSpace(dto.InstituteName))
                    existing.InstituteName = dto.InstituteName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.ScoreValue))
                    existing.ScoreValue = dto.ScoreValue.Trim();

                if (!string.IsNullOrWhiteSpace(dto.ScoreType) &&
                    int.TryParse(dto.ScoreType.Trim(), out int parsedScoreType))
                    existing.ScoreType = parsedScoreType;

                if (!string.IsNullOrWhiteSpace(dto.GradeDivision))
                    existing.GradeDivision = dto.GradeDivision.Trim();

                existing.EducationGap = dto.IsEducationGapBeforeDegree;
                existing.GapYears = dto.GapYears;
                existing.ReasonOfEducationGap = dto.ReasonOfEducationGap?.Trim();

                existing.StartDate = dto.StartDate ?? existing.StartDate;
                existing.EndDate = dto.EndDate ?? existing.EndDate;

                // ===============================
                // 7️⃣ FILE UPLOAD
                // ===============================
                if (dto.EducationDocument != null &&
                    dto.EducationDocument.Length > 0)
                {
                    try
                    {
                        string degreeName =
                            dto.Degree?.Trim().ToLower().Replace(" ", "_") ?? "doc";

                        string newFileName =
                            $"{ConstantValues.EducationFolder}-{existing.EmployeeId}-{degreeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{existing.EmployeeId}/" +
                            $"{ConstantValues.EducationFolder}";

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            dto.EducationDocument,
                            folderPath,
                            newFileName);

                        existing.FilePath = uploadedFileKey;
                        existing.FileName = newFileName;
                        existing.HasEducationDocUploded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Education file upload failed");
                        throw new ApiException("File upload failed.", 500);
                    }
                }

                // ===============================
                // 🧾 AUDIT
                // ===============================
                existing.UpdatedById = validation.UserEmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 8️⃣ SAVE
                // ===============================
                await _unitOfWork.EmployeeEducationRepository
                    .UpdateEmployeeFieldAsync(existing);

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("UpdateEducation success");

                return ApiResponse<bool>.Success(true, "Education updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error updating education");

                // 🧹 FILE CLEANUP
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "File cleanup failed");
                    }
                }

                throw; // 🚨 MUST
            }
        }

    }


}
