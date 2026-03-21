using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class CreateEmployeeImageCommand : IRequest<ApiResponse<GetEmployeeImageReponseDTO>>
    {
        public CreateEmployeeImageRequestDTO DTO { get; set; }

        public CreateEmployeeImageCommand(CreateEmployeeImageRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateEmployeeImageCommandHandler
      : IRequestHandler<CreateEmployeeImageCommand, ApiResponse<GetEmployeeImageReponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateEmployeeImageCommandHandler> _logger;
        private readonly IFileServiceAWS _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _configuration;

        public CreateEmployeeImageCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateEmployeeImageCommandHandler> logger,
            IFileServiceAWS fileStorageService,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<GetEmployeeImageReponseDTO>> Handle(
            CreateEmployeeImageCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // =========================================
                // 1️⃣ COMMON VALIDATION
                // =========================================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<GetEmployeeImageReponseDTO>
                        .Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // =========================================
                // 2️⃣ PERMISSION CHECK (OPTIONAL)
                // =========================================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("AddEmployeeImage"))
                //     return ApiResponse<GetEmployeeImageReponseDTO>.Fail("Permission denied.");

                // =========================================
                // 3️⃣ FILE UPLOAD (SAFE)
                // =========================================
                string? filePath = null;
                string? fileName = null;
                bool hasImageUploaded = false;

                if (request.DTO.ImageFile != null &&   request.DTO.ImageFile.Length > 0 &&     request.DTO.IsActive)
                {
                    try
                    {
                        fileName =
                            $"profile-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}";

                        // ✅ S3 key path (NO directory creation)
                        string folderPath =
                            $"tenants-{validation.TenantId}/employees/{request.DTO.Prop.EmployeeId}/profile";

                        // ✅ Upload to S3
                        var fileKey =  await _fileStorageService.UploadFileAsync(
                                request.DTO.ImageFile,
                                folderPath,
                                fileName);

                        if (!string.IsNullOrWhiteSpace(fileKey))
                        {
                            // ✅ Direct S3 key save karo (NO relative path)
                            filePath = fileKey;

                            hasImageUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Employee image upload failed");
                    }
                }

                // =========================================
                // 4️⃣ MAP → ENTITY
                // =========================================
                var entity = _mapper.Map<EmployeeImage>(request.DTO);

                entity.EmployeeId = request.DTO.Prop.EmployeeId;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = request.DTO.IsActive;
                entity.HasImageUploaded = hasImageUploaded;
                entity.FileName = hasImageUploaded ? fileName : null;
                entity.FilePath = hasImageUploaded ? filePath : null;
                entity.FileType = hasImageUploaded ? 1 : 0;
                entity.IsPrimary = true;

                // =========================================
                // 5️⃣ SAVE (REPO RETURNS DTO)
                // =========================================
                var savedImage =
                    await _unitOfWork.Employees.AddImageAsync(entity);

                if (savedImage == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<GetEmployeeImageReponseDTO>
                        .Fail("Employee image creation failed.");
                }

                // =========================================
                // 6️⃣ BUILD FULL IMAGE URL
                // =========================================
                string baseUrl =
                    _configuration["FileSettings:BaseUrl"] ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(savedImage.FilePath))
                    savedImage.FilePath = $"{baseUrl}{savedImage.FilePath}";

                // =========================================
                // 7️⃣ COMMIT
                // =========================================
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Success(savedImage, "Employee image added successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "❌ CreateEmployeeImage failed");

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Fail("Unexpected error occurred while adding employee image.");
            }
        }
    }
}