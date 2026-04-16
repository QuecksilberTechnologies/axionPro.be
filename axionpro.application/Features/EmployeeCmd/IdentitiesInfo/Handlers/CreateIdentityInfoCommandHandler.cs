using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Exceptions;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers
{
    public class CreateIdentityInfoCommand : IRequest<ApiResponse<bool>>
    {
        public CreateEmployeeIdentityRequestDTO DTO { get; set; }

        public CreateIdentityInfoCommand(CreateEmployeeIdentityRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateEmployeeIdentityCommandHandler  : IRequestHandler<CreateIdentityInfoCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<CreateIdentityInfoCommand> _logger;
        public CreateEmployeeIdentityCommandHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService, ILogger<CreateIdentityInfoCommand> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
            _logger = logger;   
        }

        public async Task<ApiResponse<bool>> Handle(
    CreateIdentityInfoCommand request,
    CancellationToken cancellationToken)
        {
            var uploadedFiles = new List<string>();

            try
            {
                _logger.LogInformation("CreateEmployeeIdentity started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                if (request?.DTO?.Identities == null || !request.DTO.Identities.Any())
                    throw new ValidationErrorException("No identity data received.");

                var first = request.DTO.Identities.First();

                var validation =
                    await _commonRequestService.ValidateRequestAsync(first.UserEmployeeId);

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION (CRITICAL 🚨)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to add identity.");

                // ===============================
                // 3️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var entities = new List<EmployeeIdentity>();

                foreach (var identity in request.DTO.Identities)
                {
                    var employeeId =
                        RequestCommonHelper.DecodeOnlyEmployeeId(
                            identity.EmployeeId,
                            validation.Claims.TenantEncriptionKey,
                            _idEncoderService);

                    if (employeeId <= 0)
                        throw new ValidationErrorException("Invalid EmployeeId.");

                    string? documentPath = null;
                    string? documentName = null;

                    // ===============================
                    // 4️⃣ FILE UPLOAD
                    // ===============================
                    if (identity.IdentityDocumentFile != null &&
                        identity.IdentityDocumentFile.Length > 0)
                    {
                        try
                        {
                            string maskedValue =
                                identity.IdentityValue?.Length > 4
                                    ? identity.IdentityValue[^4..]
                                    : "XXXX";

                            string docCode =
                                identity.DocumnetCode?.Trim().ToLower().Replace(" ", "_") ?? "id";

                            documentName =
                                $"id-{docCode}-{employeeId}-{maskedValue}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                            string folderPath =
                                $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                                $"{ConstantValues.EmployeeFolder}/{employeeId}/identity";

                            var fileKey =
                                await _fileStorageService.UploadFileAsync(
                                    identity.IdentityDocumentFile,
                                    folderPath,
                                    documentName);

                            if (!string.IsNullOrWhiteSpace(fileKey))
                            {
                                documentPath = fileKey;
                                uploadedFiles.Add(fileKey); // 🔥 TRACK FILE
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Identity file upload failed");
                            throw new ApiException("Identity file upload failed.", 500);
                        }
                    }

                    // ===============================
                    // 5️⃣ BUILD ENTITY
                    // ===============================
                    entities.Add(new EmployeeIdentity
                    {
                        EmployeeId = employeeId,
                        IdentityCategoryDocumentId = identity.IdentityCategoryDocumentId,
                        IdentityValue = identity.IdentityValue,

                        DocumentFileName = documentName,
                        DocumentFilePath = documentPath,

                        EffectiveFrom = identity.EffectiveFrom ?? DateOnly.FromDateTime(DateTime.UtcNow),
                        EffectiveTo = identity.EffectiveTo ?? DateOnly.FromDateTime(DateTime.UtcNow),

                        HasIdentityUploaded = documentPath != null,
                        IsEditAllowed = true,
                        IsActive = true,

                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    });
                }

                // ===============================
                // 6️⃣ SAVE
                // ===============================
                var isSuccess =
                    await _unitOfWork.EmployeeIdentityRepository.CreateAsync(entities);

                if (!isSuccess)
                    throw new ApiException("Identity save failed.", 500);

                // ===============================
                // 7️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CreateEmployeeIdentity success");

                return ApiResponse<bool>.Success(
                    true,
                    "Identity details saved successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "CreateEmployeeIdentity failed");

                // 🔥 FILE CLEANUP (CRITICAL)
                foreach (var file in uploadedFiles)
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(file);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "Failed to cleanup identity file");
                    }
                }

                throw; // 🚨 MUST
            }
        }


    }



}
