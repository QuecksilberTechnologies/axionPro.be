using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
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
            IIdEncoderService idEncoderService)
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
        }

        public async Task<ApiResponse<bool>> Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
               try
                {
                    var dto = request.DTO ?? throw new ArgumentNullException(nameof(request.DTO));

                    // -----------------------------------------------
                    // 1) TOKEN VALIDATION
                    // -----------------------------------------------
                    var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                        .ToString()?.Replace("Bearer ", "");

                    var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                    var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                    long loggedInEmpId = await _unitOfWork.CommonRepository
                        .ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);


                    // -----------------------------------------------
                    // 2) DECRYPT IDs
                    // -----------------------------------------------
                    string finalKey = EncryptionSanitizer.SuperSanitize(tokenClaims.TenantEncriptionKey);

                    request.DTO._UserEmployeeId = _idEncoderService.DecodeId(dto.UserEmployeeId, finalKey);

                    long decryptedTenantId = _idEncoderService.DecodeId(
                        EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);

                    request.DTO._Id = SafeParser.TryParseLong(dto.Id ?? dto.Id);
                    // -----------------------------------------------------
                    // 4️⃣ FETCH EXISTING IMAGE RECORD
                    // -----------------------------------------------------
                    var employeeImageInfo = await _employeeRepository.IsImageExist(dto._Id, true);
                    if (employeeImageInfo == null)
                        return ApiResponse<bool>.Fail("Employee image record not found.");


                    // -----------------------------------------------------
                    // 5️⃣ DETERMINE FINAL FILE NAME (Smart Logic)
                    // -----------------------------------------------------
                    string fileName = !string.IsNullOrWhiteSpace(dto.FileName) ? dto.FileName.Trim().Replace(" ", "_").ToLower() : employeeImageInfo.FileName?.Trim().Replace(" ", "_").ToLower();


                    //if (string.IsNullOrWhiteSpace(fileName))
                    //    return ApiResponse<bool>.Fail("Invalid image file name.");


                    // -----------------------------------------------------
                    // 6️⃣ FILE UPLOAD PROCESS (Delete → Replace)
                    // -----------------------------------------------------
                    if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
                    {
                        string folderPath = _fileStorageService.GetEmployeeFolderPath(decryptedTenantId, employeeImageInfo.EmployeeId, "profile");

                        // Delete previous file if exists
                        if (employeeImageInfo.HasImageUploaded && !string.IsNullOrWhiteSpace(employeeImageInfo.FileName))
                        {
                            try
                            {
                                string oldPath = _fileStorageService.GenerateFullFilePath(folderPath, employeeImageInfo.FileName);

                                if (File.Exists(oldPath))
                                {
                                    File.Delete(oldPath);
                                    _logger.LogInformation("Old profile image removed: {Path}", oldPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error deleting old image.");
                            }
                        }

                        // Create new file name
                        string newFileName = $"pic-{employeeImageInfo.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.png".Trim();

                        // Save new file
                        using var ms = new MemoryStream();
                        await dto.ProfileImage.CopyToAsync(ms);
                        var savedPath = await _fileStorageService.SaveFileAsync(ms.ToArray(), newFileName, folderPath);
                        // Update DB entity
                        employeeImageInfo.FileName = newFileName;
                        employeeImageInfo.FilePath = _fileStorageService.GetRelativePath(savedPath);
                        employeeImageInfo.HasImageUploaded = true;
                        employeeImageInfo.UpdateById = request.DTO._UserEmployeeId;
                        employeeImageInfo.UpdatedDateTime = DateTime.UtcNow;
                    }


                    // -----------------------------------------------------
                    // 7️⃣ SAVE IN DATABASE
                    // -----------------------------------------------------
                    bool updateStatus = await _unitOfWork.Employees.UpdateProfileImage(employeeImageInfo);

                    if (!updateStatus)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.Fail("Failed to update image.");
                    }

                    await _unitOfWork.CommitTransactionAsync();
                    return ApiResponse<bool>.Success(true, "Identity image updated successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in Identity Image Update");
                    return ApiResponse<bool>.Fail("Unexpected error occurred.", new() { ex.Message });
                }
            
        }
      

    }
}