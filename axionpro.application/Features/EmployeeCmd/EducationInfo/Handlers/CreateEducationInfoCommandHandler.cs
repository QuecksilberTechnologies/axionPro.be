using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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
        private readonly IConfiguration _configuration;
        private readonly ICommonRequestService _commonRequestService;


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
            IFileStorageService fileStorageService, IConfiguration configuration, ICommonRequestService commonRequestService)
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
            _configuration = configuration;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(CreateEducationInfoCommand request, CancellationToken cancellationToken)
        {
            string? savedFullPath = null;  // 📂 File full path track karne ke liye

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION (SAME AS CONTACT)
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetEducationResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService
                    );

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddDependentInfo"))
                {
                    // optional hard-stop
                    // return ApiResponse<List<GetDependentResponseDTO>>
                    //     .Fail("You do not have permission to add dependent info.");
                }



                // 🔹 STEP 4: File Upload
                // =================================================
                // FILE HANDLING (S3 CLEAN)
                // =================================================
                string? docPath = null;
                string? fileName = null;
                bool HasEducationUploaded = false;

                if (request.DTO.EducationDocument != null &&
                    request.DTO.EducationDocument.Length > 0)
                {
                    try
                    {
                        // 🔹 CLEAN DEGREE NAME
                        string degreeName = request.DTO.Degree?.Trim().ToLower().Replace(" ", "_") ?? "doc";

                        // 🔹 FILE NAME
                        fileName =
                            $"{ConstantValues.EducationFolder}-{request.DTO.Prop.EmployeeId}-{degreeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        // 🔹 FOLDER PATH (YOUR STANDARD RULE)
                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/" +
                            $"{ConstantValues.EducationFolder}";

                        // 🔹 UPLOAD
                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.EducationDocument,
                            folderPath,
                            fileName);

                        if (!string.IsNullOrWhiteSpace(fileKey))
                        {
                            docPath = fileKey;
                            HasEducationUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading education document");

                        return ApiResponse<List<GetEducationResponseDTO>>
                            .Fail("File upload failed.");
                    }
                }

                // 🔹 STEP 5: Map DTO to Entity
                var educationEntity = _mapper.Map<EmployeeEducation>(request.DTO);                  
                educationEntity.EmployeeId = request.DTO.Prop.EmployeeId;                
                educationEntity.AddedById = request.DTO.Prop.UserEmployeeId;
                educationEntity.AddedDateTime = DateTime.UtcNow;
                educationEntity.IsActive = true;
                educationEntity.IsInfoVerified = false;
                educationEntity.IsEditAllowed = true;               
                educationEntity.FileType = 0;
              
                if (HasEducationUploaded)
                {
                    educationEntity.FileType = 2;//pdf
                    educationEntity.FilePath = docPath;
                    educationEntity.FileName = fileName;
                   
                }
                educationEntity.HasEducationDocUploded = HasEducationUploaded;


                // 🔹 STEP 6: Database Insert + File Validation
                var responseDTO = await _unitOfWork.EmployeeEducationRepository.CreateAsync(educationEntity);

                 

                // 🔹 STEP 7: Commit Transaction
                await _unitOfWork.CommitTransactionAsync();
               
                       
                // 🔹 STEP 8: Projection + Encryption
                var encryptedList = ProjectionHelper.ToGetEducationResponseDTOs(responseDTO, _idEncoderService,
                        validation.Claims.TenantEncriptionKey, _configuration);

                return new ApiResponse<List<GetEducationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = encryptedList
                };
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
