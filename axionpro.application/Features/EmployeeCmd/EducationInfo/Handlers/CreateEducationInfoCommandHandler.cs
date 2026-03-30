using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Exceptions;
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

        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(
      CreateEducationInfoCommand request,
      CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("CreateEducation started");

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

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (YOUR FIXED PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to add education.");

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 5️⃣ FILE UPLOAD
                // ===============================
                string? docPath = null;
                string? fileName = null;
                bool hasEducationUploaded = false;

                if (request.DTO.EducationDocument != null &&
                    request.DTO.EducationDocument.Length > 0)
                {
                    try
                    {
                        string degreeName =
                            request.DTO.Degree?.Trim().ToLower().Replace(" ", "_") ?? "doc";

                        fileName =
                            $"{ConstantValues.EducationFolder}-{request.DTO.Prop.EmployeeId}-{degreeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/" +
                            $"{ConstantValues.EducationFolder}";

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.EducationDocument,
                            folderPath,
                            fileName);

                        if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                        {
                            docPath = uploadedFileKey;
                            hasEducationUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Education file upload failed");
                        throw new ApiException("File upload failed.", 500);
                    }
                }

                // ===============================
                // 6️⃣ MAP ENTITY
                // ===============================
                var educationEntity = _mapper.Map<EmployeeEducation>(request.DTO);

                educationEntity.EmployeeId = request.DTO.Prop.EmployeeId;
                educationEntity.AddedById = request.DTO.Prop.UserEmployeeId;
                educationEntity.AddedDateTime = DateTime.UtcNow;

                educationEntity.IsActive = true;
                educationEntity.IsInfoVerified = false;
                educationEntity.IsEditAllowed = true;

                educationEntity.FileType = hasEducationUploaded ? 2 : 0;
                educationEntity.FilePath = docPath;
                educationEntity.FileName = fileName;
                educationEntity.HasEducationDocUploded = hasEducationUploaded;

                // ===============================
                // 7️⃣ SAVE
                // ===============================
                var responseDTO =
                    await _unitOfWork.EmployeeEducationRepository
                        .CreateAsync(educationEntity);

                if (responseDTO == null)
                    throw new ApiException("Failed to create education record.", 500);

                // ===============================
                // 8️⃣ PROJECTION
                // ===============================
                var encryptedList =
                    ProjectionHelper.ToGetEducationResponseDTOs(
                        responseDTO,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _configuration , _fileStorageService);

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CreateEducation success");

                return ApiResponse<List<GetEducationResponseDTO>>
                    .Success(encryptedList, "Education info created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error creating education");

                // 🧹 FILE CLEANUP (S3)
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
