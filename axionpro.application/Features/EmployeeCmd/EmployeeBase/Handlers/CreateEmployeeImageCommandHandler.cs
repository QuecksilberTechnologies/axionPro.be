using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
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
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;

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

                if (request.DTO.ImageFile != null &&  request.DTO.ImageFile.Length > 0 &&  request.DTO.IsActive)
                {
                    try
                    {
                        string cleanName =
                            EncryptionSanitizer.CleanEncodedInput(
                                request.DTO.Prop.EmployeeId.ToString());

                        fileName =
                            $"profile-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.png";

                        string folderPath =
                            _fileStorageService.GetEmployeeFolderPath(
                                validation.TenantId,
                                request.DTO.Prop.EmployeeId,
                                "profile");

                        using var ms = new MemoryStream();
                        await request.DTO.ImageFile.CopyToAsync(ms);

                        var savedFullPath =
                            await _fileStorageService.SaveFileAsync(
                                ms.ToArray(),
                                fileName,
                                folderPath);

                        if (!string.IsNullOrWhiteSpace(savedFullPath))
                        {
                            filePath =
                                _fileStorageService.GetRelativePath(savedFullPath);
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