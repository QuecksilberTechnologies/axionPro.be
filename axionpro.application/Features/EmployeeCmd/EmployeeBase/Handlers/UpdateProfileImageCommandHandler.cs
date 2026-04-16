using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
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

        public async Task<ApiResponse<bool>> Handle(
       UpdateProfileImageCommand request,
       CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("UpdateProfileImage started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ PERMISSION
                //// ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update profile image.");

                // ===============================
                // 3️⃣ FETCH EXISTING
                // ===============================
                var employeeImageInfo =
                    await _employeeRepository.IsImageExist(request.DTO.Id, true);

                if (employeeImageInfo == null)
                    throw new ApiException("Employee image record not found.", 404);

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                string folderPath =
                    $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                    $"{ConstantValues.EmployeeFolder}/{employeeImageInfo.EmployeeId}/" +
                    $"{ConstantValues.ProfileFolder}";

                // ======================================================
                // 🔴 CASE 1: DEACTIVATE (DELETE IMAGE)
                // ======================================================
                if (!request.DTO.IsActive)
                {
                    if (employeeImageInfo.HasImageUploaded&&
                        !string.IsNullOrWhiteSpace(employeeImageInfo.FilePath))
                    {
                        await _fileStorageService.DeleteFileAsync(employeeImageInfo.FilePath);
                    }

                    employeeImageInfo.HasImageUploaded = false;
                    employeeImageInfo.FileName = null;
                    employeeImageInfo.FilePath = null;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;

                    var status =
                        await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                    if (!status)
                        throw new ApiException("Failed to deactivate profile image.", 500);

                    await _unitOfWork.CommitTransactionAsync();

                    return ApiResponse<bool>.Success(true, "Profile image disabled.");
                }

                // ======================================================
                // 🟢 CASE 2: UPDATE / REPLACE IMAGE
                // ======================================================
                if (request.DTO.ProfileImage != null &&
                    request.DTO.ProfileImage.Length > 0)
                {
                    // Delete old
                    if (employeeImageInfo.HasImageUploaded &&
                        !string.IsNullOrWhiteSpace(employeeImageInfo.FilePath))
                    {
                        await _fileStorageService.DeleteFileAsync(employeeImageInfo.FilePath);
                    }

                    string newFileName =
                        $"{ConstantValues.ProfileFolder}-{employeeImageInfo.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}";

                    uploadedFileKey =
                        await _fileStorageService.UploadFileAsync(
                            request.DTO.ProfileImage,
                            folderPath,
                            newFileName);

                    employeeImageInfo.FileName = newFileName;
                    employeeImageInfo.FilePath = uploadedFileKey;
                    employeeImageInfo.HasImageUploaded = true;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;
                }

                // ===============================
                // 5️⃣ SAVE
                // ===============================
                var updateStatus =
                    await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                if (!updateStatus)
                    throw new ApiException("Failed to update profile image.", 500);

                // ===============================
                // 6️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("UpdateProfileImage success");

                return ApiResponse<bool>.Success(true, "Profile image updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "UpdateProfileImage failed");

                // 🧹 FILE CLEANUP (CRITICAL 🚨)
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "Failed to cleanup uploaded file");
                    }
                }

                throw; // 🚨 MUST
            }
        }
    }


}