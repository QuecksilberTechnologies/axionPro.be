using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Employee;
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

                // 2️⃣ Permission Check (Optional)
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                // if (!permissions.Contains("UpdateEmployeeImage")) return ApiResponse<bool>.Fail("Permission denied.");

                // 3️⃣ Check existing employee image
                var employeeImageInfo = await _employeeRepository.IsImageExist(request.DTO.Id, true);
                if (employeeImageInfo == null)
                    return ApiResponse<bool>.Fail("Employee image record not found.");

                // 4️⃣ Prepare file name (if provided, else existing)
                string? fileName = !string.IsNullOrWhiteSpace(request.DTO.FileName)
                    ? request.DTO.FileName.Trim().Replace(" ", "_").ToLower()
                    : employeeImageInfo.FileName?.Trim().Replace(" ", "_").ToLower();


                // ==================================================================
                // 5️⃣ If IsActive = FALSE → reset image info (NO file upload/deletion)
                // ==================================================================
                if (!request.DTO.IsActive)
                {
                    employeeImageInfo.HasImageUploaded = false;
                    employeeImageInfo.FileName = null;
                    employeeImageInfo.FilePath = null;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;

                    bool resetStatus = await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                    if (!resetStatus)
                        return ApiResponse<bool>.Fail("Failed to deactivate profile image.");

                    await _unitOfWork.CommitTransactionAsync();
                    return ApiResponse<bool>.Success(true, "Profile image deactivated successfully.");
                }


                // ==================================================================
                // 6️⃣ If IsActive = TRUE → image upload + replace old file
                // ==================================================================
                if (request.DTO.ProfileImage != null && request.DTO.ProfileImage.Length > 0)
                {
                    string folderPath = _fileStorageService.GetEmployeeFolderPath(
                        request.DTO.Prop.TenantId,
                        employeeImageInfo.EmployeeId,
                        "profile");

                    // Delete old file
                    if (employeeImageInfo.HasImageUploaded && !string.IsNullOrWhiteSpace(employeeImageInfo.FileName))
                    {
                        try
                        {
                            string oldPath = _fileStorageService.GenerateFullFilePath(folderPath, employeeImageInfo.FileName);

                            if (File.Exists(oldPath))
                            {
                                File.Delete(oldPath);
                                _logger.LogInformation("Old profile image deleted: {Path}", oldPath);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deleting old image.");
                        }
                    }

                    // Create new filename
                    string newFileName = $"pic-{employeeImageInfo.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                    // Save new file
                    using var ms = new MemoryStream();
                    await request.DTO.ProfileImage.CopyToAsync(ms);
                    string savedPath = await _fileStorageService.SaveFileAsync(ms.ToArray(), newFileName, folderPath);

                    // Update DB entity
                    employeeImageInfo.FileName = newFileName;
                    employeeImageInfo.FilePath = _fileStorageService.GetRelativePath(savedPath);
                    employeeImageInfo.HasImageUploaded = true;
                    employeeImageInfo.UpdateById = validation.UserEmployeeId;
                    employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;
                }


                // 7️⃣ SAVE TO DB
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