using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

public class UpdatePolicyTypeCommand : IRequest<ApiResponse<bool>>
{
    public UpdatePolicyTypeRequestDTO DTO { get; set; }

    public UpdatePolicyTypeCommand(UpdatePolicyTypeRequestDTO dto)
    {
        DTO = dto;
    }
}

public class UpdatePolicyTypeCommandHandler
    : IRequestHandler<UpdatePolicyTypeCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePolicyTypeCommandHandler> _logger;
    private readonly IPermissionService _permissionService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;

    public UpdatePolicyTypeCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdatePolicyTypeCommandHandler> logger,
        IPermissionService permissionService,
        IFileStorageService fileStorageService,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _permissionService = permissionService;
        _fileStorageService = fileStorageService;
        _commonRequestService = commonRequestService;
    }

    public async Task<ApiResponse<bool>> Handle(
      UpdatePolicyTypeCommand request,
      CancellationToken cancellationToken)
    {
        string? uploadedFileKey = null;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("🔹 UpdatePolicyType started");

            // ===============================
            // 1️⃣ NULL SAFETY
            // ===============================
            if (request?.DTO == null || request.DTO.Id <= 0)
                throw new ValidationErrorException("Invalid request.");

            // ===============================
            // 2️⃣ AUTH VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 3️⃣ PERMISSION CHECK
            // ===============================
            //var hasAccess = await _permissionService.HasAccessAsync(
            //    validation.RoleId,
            //    Modules.PolicyType,
            //    Operations.Update);

            //if (!hasAccess)
            //    throw new UnauthorizedAccessException("Access denied.");

            request.DTO.Prop ??= new();
            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 4️⃣ FETCH ENTITY
            // ===============================
            var policyType = await _unitOfWork.PolicyTypeRepository
                .GetPolicyTypeByIdAsync(request.DTO.Id, true);

            if (policyType == null)
                throw new ApiException("Policy type not found.", 404);

            // ===============================
            // 5️⃣ UPDATE FIELDS
            // ===============================
            if (!string.IsNullOrWhiteSpace(request.DTO.PolicyName))
                policyType.PolicyName = request.DTO.PolicyName.Trim();

            if (!string.IsNullOrWhiteSpace(request.DTO.Description))
                policyType.Description = request.DTO.Description.Trim();

            policyType.IsActive = request.DTO.IsActive;
            policyType.UpdateById = validation.UserEmployeeId;
            policyType.UpdateDateTime = DateTime.UtcNow;

            var updated = await _unitOfWork.PolicyTypeRepository
                .UpdatePolicyTypeAsync(policyType);

            if (!updated)
                throw new ApiException("Policy type update failed.", 500);

            // ===============================
            // 6️⃣ FILE UPLOAD
            // ===============================
            if (request.DTO.FormFile != null && request.DTO.FormFile.Length > 0)
            {
                string safeName = EncryptionSanitizer
                    .CleanEncodedInput(policyType.PolicyName ?? "policy")
                    .ToLower()
                    .Replace(" ", "_");

                string fileName =
                    $"company-policy-{safeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                string folderPath =
                    $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.PoliciesFolder}";

                uploadedFileKey = await _fileStorageService.UploadFileAsync(
                    request.DTO.FormFile,
                    folderPath,
                    fileName);

                if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                {
                    var existingDoc =
                        await _unitOfWork.CompanyPolicyDocumentRepository
                            .GetByIdAsync(policyType.Id, validation.TenantId, request.DTO.IsActive);

                    if (existingDoc == null)
                    {
                        await _unitOfWork.CompanyPolicyDocumentRepository.AddAsync(
                            new CompanyPolicyDocument
                            {
                                TenantId = validation.TenantId,
                                PolicyTypeId = policyType.Id,
                                DocumentTitle = policyType.PolicyName,
                                FileName = fileName,
                                FilePath = uploadedFileKey,
                                IsActive = true,
                                IsSoftDeleted = false,
                                AddedById = validation.UserEmployeeId,
                                AddedDateTime = DateTime.UtcNow
                            });
                    }
                    else
                    {
                        existingDoc.FileName = fileName;
                        existingDoc.FilePath = uploadedFileKey;
                        existingDoc.UpdatedById = validation.UserEmployeeId;
                        existingDoc.IsActive = request.DTO.IsActive;
                        existingDoc.UpdatedDateTime = DateTime.UtcNow;

                        await _unitOfWork.CompanyPolicyDocumentRepository
                            .UpdateAsync(existingDoc);
                    }
                }
            }

            // ===============================
            // 7️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("✅ PolicyType updated successfully");

            return ApiResponse<bool>
                .Success(true, "Policy type updated successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "❌ UpdatePolicyType failed");

            // 🔥 FILE CLEANUP
            if (!string.IsNullOrEmpty(uploadedFileKey))
            {
                try
                {
                    await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                }
                catch (Exception fileEx)
                {
                    _logger.LogError(fileEx, "Failed to delete uploaded file after rollback.");
                }
            }

            throw; // ✅ CRITICAL
        }
    }
}

 



