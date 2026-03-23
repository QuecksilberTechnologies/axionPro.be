using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _configuration;

        public CreateEmployeeImageCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateEmployeeImageCommandHandler> logger,
            IFileStorageService fileStorageService,
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
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("CreateEmployeeImage started");

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

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (YOUR PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to add employee image.");

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 5️⃣ FILE UPLOAD
                // ===============================
                string? filePath = null;
                string? fileName = null;
                bool hasImageUploaded = false;

                if (request.DTO.ImageFile != null &&
                    request.DTO.ImageFile.Length > 0 &&
                    request.DTO.IsActive)
                {
                    try
                    {
                        fileName =
                            $"profile-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}";

                        string folderPath =
                            $"tenants/tenant-{validation.TenantId}/employees/{request.DTO.Prop.EmployeeId}/profile";

                        uploadedFileKey =
                            await _fileStorageService.UploadFileAsync(
                                request.DTO.ImageFile,
                                folderPath,
                                fileName);

                        if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                        {
                            filePath = uploadedFileKey;
                            hasImageUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Employee image upload failed");
                        throw new ApiException("Image upload failed.", 500);
                    }
                }

                // ===============================
                // 6️⃣ MAP ENTITY
                // ===============================
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

                // ===============================
                // 7️⃣ SAVE
                // ===============================
                var savedImage =
                    await _unitOfWork.Employees.AddImageAsync(entity);

                if (savedImage == null)
                    throw new ApiException("Employee image creation failed.", 500);

                // ===============================
                // 8️⃣ BUILD FULL URL
                // ===============================
                string baseUrl =
                    _configuration["FileSettings:BaseUrl"] ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(savedImage.FilePath))
                    savedImage.FilePath = $"{baseUrl}{savedImage.FilePath}";

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CreateEmployeeImage success");

                return ApiResponse<GetEmployeeImageReponseDTO>
                    .Success(savedImage, "Employee image added successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "CreateEmployeeImage failed");

                // 🧹 FILE CLEANUP (CRITICAL 🚨)
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "Failed to cleanup uploaded image");
                    }
                }

                throw; // 🚨 MUST
            }
        }
    }
}