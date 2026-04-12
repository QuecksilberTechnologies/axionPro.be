using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdatePolicyTypeCommand : IRequest<ApiResponse<GetPolicyTypeResponseDTO>>
{
    public UpdatePolicyTypeRequestDTO DTO { get; set; }

    public UpdatePolicyTypeCommand(UpdatePolicyTypeRequestDTO dto)
    {
        DTO = dto;
    }
}

public class UpdatePolicyTypeCommandHandler
    : IRequestHandler<UpdatePolicyTypeCommand, ApiResponse<GetPolicyTypeResponseDTO>>
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

    public async Task<ApiResponse<GetPolicyTypeResponseDTO>> Handle(
    UpdatePolicyTypeCommand request,
    CancellationToken cancellationToken)
    {
        string? uploadedFileKey = null;
        string fileName = string.Empty;
        bool hasPolicyDocUploaded = false;

        try
        {
            _logger.LogInformation("🔹 UpdatePolicyType started");

            // ===============================
            // ✅ AUTH VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // ✅ REQUEST VALIDATION
            // ===============================
            if (request?.DTO == null || request.DTO.Id <= 0)
                throw new ValidationErrorException("Invalid request.");

            if (request.DTO.EmployeeTypeIds == null || !request.DTO.EmployeeTypeIds.Any())
                throw new ValidationErrorException("At least one EmployeeType is required.");

            // ===============================
            // 🔥 START TRANSACTION
            // ===============================
            await _unitOfWork.BeginTransactionAsync();

            // ===============================
            // 🔥 FILE UPLOAD
            // ===============================
            if (request.DTO.FormFile != null && request.DTO.FormFile.Length > 0)
            {
                string safeName = EncryptionSanitizer
                    .CleanEncodedInput(request.DTO.PolicyName ?? "policy")
                    .ToLower()
                    .Replace(" ", "_");

                fileName = $"{safeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                string folderPath =
                    $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.PoliciesFolder}";

                uploadedFileKey = await _fileStorageService.UploadFileAsync(
                    request.DTO.FormFile,
                    folderPath,
                    fileName);

                hasPolicyDocUploaded = !string.IsNullOrWhiteSpace(uploadedFileKey);
            }

            // ===============================
            // 🔥 FETCH POLICY
            // ===============================
            var policyType = await _unitOfWork.PolicyTypeRepository
                .GetPolicyTypeByIdAsync(request.DTO.Id, true);

            if (policyType == null)
                throw new ApiException("Policy type not found.", 404);

            // ===============================
            // 🔥 UPDATE ENTITY
            // ===============================
            policyType.PolicyName = request.DTO.PolicyName.Trim();
            policyType.Description = request.DTO.Description?.Trim();
            policyType.IsActive = request.DTO.IsActive;
            policyType.IsStructured = request.DTO.IsStructured;
            policyType.PolicyTypeEnumVal = request.DTO.PolicyTypeEnumVal;
            policyType.HasPolicyDocUploaded = hasPolicyDocUploaded;

            policyType.UpdateById = validation.UserEmployeeId;
            policyType.UpdateDateTime = DateTime.UtcNow;

            await _unitOfWork.PolicyTypeRepository.UpdatePolicyTypeAsync(policyType);
           
            // ===============================
            // 🔥 REPLACE EMPLOYEE TYPE MAPPING
            // ===============================
            var existingMappings = await _unitOfWork
                .UnStructuredEmployeePolicyTypeMappingRepository
                .GetByEmployeeTypeByPolicyTypeIdAsync(policyType.Id, validation.TenantId);

            foreach (var item in existingMappings)
            {
                item.IsSoftDeleted = true;
                item.SoftDeletedById = validation.UserEmployeeId;
                item.SoftDeletedDateTime = DateTime.UtcNow;
            }

            if (existingMappings.Any())
            {
                await _unitOfWork
                    .UnStructuredEmployeePolicyTypeMappingRepository
                    .UpdateRangeAsync(existingMappings); // 🔥 FIXED
            }

            var newMappings = request.DTO.EmployeeTypeIds
                .Distinct()
                .Select(empTypeId => new UnStructuredPolicyTypeMappingWithEmployeeType
                {
                    TenantId = validation.TenantId,
                    PolicyTypeId = policyType.Id,
                    EmployeeTypeId = empTypeId,
                    IsActive = true,
                    StartDate = DateTime.UtcNow,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsSoftDeleted = false
                }).ToList();

            if (newMappings.Any())
            {
                await _unitOfWork
                    .UnStructuredEmployeePolicyTypeMappingRepository
                    .AddRangeAsync(newMappings);
            }

            // ===============================
            // 🔥 DOCUMENT UPSERT
            // ===============================
            if (hasPolicyDocUploaded && uploadedFileKey != null)
            {
                var existingDoc = await _unitOfWork.PolicyTypeDocumentRepository
                    .GetByPolicyTypeIdAsync(policyType.Id, validation.TenantId); // 🔥 FIXED

                if (existingDoc != null)
                {
                    if (!string.IsNullOrEmpty(existingDoc.FilePath))
                        await _fileStorageService.DeleteFileAsync(existingDoc.FilePath);

                    existingDoc.FileName = fileName;
                    existingDoc.FilePath = uploadedFileKey;
                    existingDoc.UpdatedById = validation.UserEmployeeId;
                    existingDoc.UpdatedDateTime = DateTime.UtcNow;

                    await _unitOfWork.PolicyTypeDocumentRepository.UpdateAsync(existingDoc);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    await _unitOfWork.PolicyTypeDocumentRepository.AddAsync(
                        new PolicyTypeDocument
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
            }
            
            // ===============================
            // ✅ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            // ===============================
            // 🔥 RESPONSE
            // ===============================
            var response = new GetPolicyTypeResponseDTO
            {
                Id = policyType.Id,
                PolicyName = policyType.PolicyName ?? string.Empty,
                Description = policyType.Description,
                IsActive = policyType.IsActive ?? false,
                IsStructured = policyType.IsStructured,
                PolicyTypeEnumVal = policyType.PolicyTypeEnumVal,
                EmployeeTypeIds = request.DTO.EmployeeTypeIds.ToList(),
                DocDetails = new List<GetPolicyTypeDocumentResponseDTO>()
            };

            if (hasPolicyDocUploaded && uploadedFileKey != null)
            {
                response.DocDetails.Add(new GetPolicyTypeDocumentResponseDTO
                {
                    PolicyTypeId = policyType.Id,
                    DocumentTitle = policyType.PolicyName,
                    FileName = fileName,
                    FilePath = _fileStorageService.GetFileUrl(uploadedFileKey),
                    IsActive = true
                });
            }

            return ApiResponse<GetPolicyTypeResponseDTO>
                .Success(response, "Policy type updated successfully.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();

            if (!string.IsNullOrEmpty(uploadedFileKey))
            {
                try { await _fileStorageService.DeleteFileAsync(uploadedFileKey); }
                catch { }
            }

            _logger.LogError(ex, "❌ UpdatePolicyType failed");
            throw;
        }
    }
}




