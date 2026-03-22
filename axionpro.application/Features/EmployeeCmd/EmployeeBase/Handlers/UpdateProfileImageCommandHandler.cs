using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class UpdateProfileImageCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeImageRequestDTO DTO { get; set; }
        public UpdateProfileImageCommand(UpdateEmployeeImageRequestDTO dto) => DTO = dto;
    }

    public class UpdateIdentityInfoCommandHandler
        : IRequestHandler<UpdateProfileImageCommand, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateIdentityInfoCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;


        public UpdateIdentityInfoCommandHandler(
            IBaseEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateIdentityInfoCommandHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IEncryptionService encryptionService,
            IFileStorageService fileStorageService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
            _fileStorageService = fileStorageService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 2️⃣ Get existing image info
                var employeeImageInfo = await _employeeRepository.IsImageExist(request.DTO.Id, true);
                if (employeeImageInfo == null)
                    return ApiResponse<bool>.Fail("Employee image record not found.");

                // 3️⃣ S3 folder path (KEY PREFIX)
                string folderPath = $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.EmployeeFolder}/{employeeImageInfo.EmployeeId}/{ConstantValues.ProfileFolder}";


                // ==================================================================
                // 4️⃣ If IsActive = FALSE → DELETE FROM S3 + RESET DB
                // ==================================================================
                if (!request.DTO.IsActive)
                {
                    if (employeeImageInfo.HasImageUploaded &&
                        !string.IsNullOrWhiteSpace(employeeImageInfo.FilePath))
                    {
                        try
                        {
                            await _fileStorageService.DeleteFileAsync(employeeImageInfo.FilePath);

                            _logger.LogInformation("Profile image deleted from S3: {Key}", employeeImageInfo.FilePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting file from S3.");
                        }
                    }

                    // Reset DB
                    employeeImageInfo.HasImageUploaded = false;
                    employeeImageInfo.FileName = null;
                    employeeImageInfo.FilePath = null;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;

                    bool resetStatus = await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                    if (!resetStatus)
                        return ApiResponse<bool>.Fail("Failed to deactivate profile image.");

                    await _unitOfWork.CommitTransactionAsync();
                    return ApiResponse<bool>.Success(true, "Profile image disabled & deleted.");
                }

                // ==================================================================
                // 5️⃣ If IsActive = TRUE → REPLACE IMAGE
                // ==================================================================
                if (request.DTO.ProfileImage != null && request.DTO.ProfileImage.Length > 0)
                {
                    // 🔹 Delete old file (if exists)
                    if (employeeImageInfo.HasImageUploaded && !string.IsNullOrWhiteSpace(employeeImageInfo.FilePath))
                    {
                        try
                        {
                            await _fileStorageService.DeleteFileAsync(employeeImageInfo.FilePath);

                            _logger.LogInformation("Old profile image deleted from S3: {Key}", employeeImageInfo.FilePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting old image from S3.");
                        }
                    }

                    // 🔹 Generate new filename
                    string newFileName = $"{ConstantValues.ProfileFolder}-{employeeImageInfo.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}";

                    // 🔹 Upload to S3
                    var fileKey = await _fileStorageService.UploadFileAsync( request.DTO.ProfileImage,folderPath,newFileName);

                    // 🔹 Update DB
                    employeeImageInfo.FileName = newFileName;
                    employeeImageInfo.FilePath = fileKey; // ✅ IMPORTANT
                    employeeImageInfo.HasImageUploaded = true;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;
                }

                // ==================================================================
                // 6️⃣ SAVE
                // ==================================================================
                bool updateStatus = await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                if (!updateStatus)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to update profile image.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Profile image updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in Profile Image Update");
                return ApiResponse<bool>.Fail("Unexpected error occurred.", new() { ex.Message });
            }
        }

    }


}