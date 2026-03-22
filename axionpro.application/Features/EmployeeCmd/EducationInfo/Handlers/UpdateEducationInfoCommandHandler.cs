using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Education;
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


        public async Task<ApiResponse<bool>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
        
        
        
        {
            try
            {

                if (request.DTO.Prop == null)
                {
                    request.DTO.Prop = new ExtraPropRequestDTO();
                }

                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION (SAME AS CONTACT)
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
 

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddDependentInfo"))
                {
                    // optional hard-stop
                    // return ApiResponse<List<GetDependentResponseDTO>>
                    //     .Fail("You do not have permission to add dependent info.");
                }


 

                // ------------------------ FETCH EXISTING RECORD ------------------------
                var existing = await _unitOfWork.EmployeeEducationRepository.GetSingleRecordAsync(request.DTO.Id, true);

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
                // =================================================
                // FILE HANDLING (S3 - NO DELETE, AUDIT SAFE)
                // =================================================
                if (request.DTO.EducationDocument != null &&
                    request.DTO.EducationDocument.Length > 0)
                {
                    try
                    {
                        // 🔹 OLD FILE KO TOUCH NAHI KARNA (IMPORTANT)
                        // S3 me rehne do (audit/history)

                        // 🔹 CLEAN DEGREE NAME
                        string degreeName = request.DTO.Degree?
                            .Trim()
                            .ToLower()
                            .Replace(" ", "_") ?? "doc";

                        // 🔹 FILE NAME
                        string newFileName =
                            $"{ConstantValues.EducationFolder}-{existing.EmployeeId}-{degreeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        // 🔹 FOLDER PATH (STANDARD RULE)
                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{existing.EmployeeId}/" +
                            $"{ConstantValues.EducationFolder}";

                        // 🔹 UPLOAD
                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.EducationDocument,
                            folderPath,
                            newFileName);

                        // 🔹 UPDATE DB (ONLY NEW FILE)
                        existing.FilePath = fileKey;
                        existing.FileName = newFileName;
                        existing.HasEducationDocUploded = true;

                        _logger.LogInformation("Education document uploaded. Old file preserved.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error uploading education document for employee {Emp}",
                            existing.EmployeeId);

                        return ApiResponse<bool>.Fail("File upload failed.");
                    }
                }
                // ------------------------ SAVE CHANGES ------------------------
                existing.UpdatedById = request.DTO.Prop.UserEmployeeId;
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
