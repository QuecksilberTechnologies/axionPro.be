using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Logging;

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
        // 🔹 Client pattern variables (same as CREATE)
        string? docPath = null;
        string? docName = null;
        bool hasFileUploaded = false;

        try
        {
            // --------------------------------------------------
            // 1️⃣ Safety check
            // --------------------------------------------------
            if (request.DTO == null)
                return ApiResponse<bool>.Fail("Invalid request.");

            // --------------------------------------------------
            // 2️⃣ Common validation (Tenant / User)
            // --------------------------------------------------
            var validation = await _commonRequestService.ValidateRequestAsync();
            if (!validation.Success)
                return ApiResponse<bool>.Fail(validation.ErrorMessage);

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // --------------------------------------------------
            // 3️⃣ Permission check
            // --------------------------------------------------
            var permissions =
                await _permissionService.GetPermissionsAsync(validation.RoleId);

            if (!permissions.Contains("UpdatePolicyType"))
            {

              //  return ApiResponse<bool>.Fail("You do not have permission to update policy types.");
            }
            // --------------------------------------------------
            // 4️⃣ Begin transaction
            // --------------------------------------------------
            await _unitOfWork.BeginTransactionAsync();

            // --------------------------------------------------
            // 5️⃣ Fetch existing PolicyType
            // --------------------------------------------------
            var policyType =
                await _unitOfWork.PolicyTypeRepository
                    .GetPolicyTypeByIdAsync(request.DTO.Id, true);

            if (policyType == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.Fail("Policy type not found.");
            }

            // --------------------------------------------------
            // 6️⃣ Update PolicyType fields
            // --------------------------------------------------


            // ✅ Update PolicyName ONLY if provided
            if (!string.IsNullOrWhiteSpace(request.DTO.PolicyName))
            {
                policyType.PolicyName = request.DTO.PolicyName.Trim();
            }

            // ✅ Update Description ONLY if provided
            if (!string.IsNullOrWhiteSpace(request.DTO.Description))
            {
                policyType.Description = request.DTO.Description.Trim();
            }

            // ✅ IsActive is explicit (checkbox / toggle)
            policyType.IsActive = request.DTO.IsActive;

            policyType.UpdateById = validation.UserEmployeeId;
            policyType.UpdateDateTime = DateTime.UtcNow;

 

            var policyUpdated =
                await _unitOfWork.PolicyTypeRepository.UpdatePolicyTypeAsync(policyType);

            if (!policyUpdated)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.Fail("Policy type update failed.");
            }

            // --------------------------------------------------
            // 7️⃣ OPTIONAL FILE UPLOAD (same as CREATE)
            // --------------------------------------------------
            // --------------------------------------------------
            // 7️⃣ OPTIONAL FILE UPLOAD (S3 CLEAN)
            // --------------------------------------------------
            if (request.DTO.FormFile != null &&
                request.DTO.FormFile.Length > 0)
            {
                try
                {
                    string safePolicyName =
                        EncryptionSanitizer.CleanEncodedInput(
                            (policyType.PolicyName ?? "policy"))
                        .ToLower()
                        .Replace(" ", "_");

                    // 🔹 FILE NAME
                    string fileName =
                        $"company-policy-{safePolicyName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                    // 🔹 FOLDER PATH (STANDARD)
                    string folderPath = $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.PoliciesFolder}";


                    // 🔹 UPLOAD TO S3
                    var fileKey = await _fileStorageService.UploadFileAsync(
                        request.DTO.FormFile,
                        folderPath,
                        fileName);

                    if (!string.IsNullOrWhiteSpace(fileKey))
                    {
                        docName = fileName;
                        docPath = fileKey;   // ✅ IMPORTANT
                        hasFileUploaded = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Policy update file upload failed");

                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("File upload failed.");
                }
            }
            // --------------------------------------------------
            // 8️⃣ DOCUMENT UPSERT (CREATE PATTERN FOLLOWED)
            // --------------------------------------------------
            if (hasFileUploaded)
            {
                var existingDoc =
                    await _unitOfWork.CompanyPolicyDocumentRepository.GetByIdAsync(policyType.Id,validation.TenantId, request.DTO.IsActive);

                if (existingDoc == null)
                {
                    // ➕ INSERT (document never existed)
                    var newDoc = new CompanyPolicyDocument
                    {                        
                        TenantId = validation.TenantId,
                        PolicyTypeId = policyType.Id,
                        DocumentTitle = policyType.PolicyName,
                        FileName = docName!,
                        FilePath = docPath!,
                        IsActive = true,
                        IsSoftDeleted = false,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    };

                    await _unitOfWork.CompanyPolicyDocumentRepository
                        .AddAsync(newDoc);
                }
                else
                {
                    // ✏️ UPDATE existing document

                    existingDoc.FileName = docName!;
                    existingDoc.FilePath = docPath!;
                    existingDoc.UpdatedById = validation.UserEmployeeId;
                    existingDoc.IsActive = request.DTO.IsActive;
                    existingDoc.UpdatedDateTime = DateTime.UtcNow;

                    await _unitOfWork.CompanyPolicyDocumentRepository
                        .UpdateAsync(existingDoc);
                }
            }

            // --------------------------------------------------
            // 9️⃣ Commit transaction
            // --------------------------------------------------
            await _unitOfWork.CommitTransactionAsync();


            return ApiResponse<bool>
                .Success(true, "Policy type updated successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating PolicyType");

            await _unitOfWork.RollbackTransactionAsync();

            return ApiResponse<bool>
                .Fail("An unexpected error occurred while updating policy type.");
        }
    }
}

 



