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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    public class CreatePolicyTypeCommand : IRequest<ApiResponse<GetPolicyTypeResponseDTO>>
    {
        public CreatePolicyTypeRequestDTO DTO { get; set; }

        public CreatePolicyTypeCommand(CreatePolicyTypeRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
    public class CreatePolicyTypeCommandHandler
     : IRequestHandler<CreatePolicyTypeCommand, ApiResponse<GetPolicyTypeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePolicyTypeCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _config;

        public CreatePolicyTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreatePolicyTypeCommandHandler> logger,
            IPermissionService permissionService,
            IFileStorageService fileStorageService,
            ICommonRequestService commonRequestService,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;
               _config = configuration;
        }

        public async Task<ApiResponse<GetPolicyTypeResponseDTO>> Handle(
         CreatePolicyTypeCommand request,
         CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;
            bool hasPolicyDocUploaded = false; // ✅ FIX

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🔹 CreatePolicyType started");

                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                if (request.DTO.EmployeeTypeIds == null || !request.DTO.EmployeeTypeIds.Any())
                    throw new ValidationErrorException("At least one EmployeeType is required.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 🔥 FILE UPLOAD FIRST (IMPORTANT)
                // ===============================
                string fileName = string.Empty;

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

                    if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                    {
                        hasPolicyDocUploaded = true; // ✅ SET FLAG
                    }
                }

                // ===============================
                // 3️⃣ CREATE POLICY TYPE
                // ===============================
                var policyType = new PolicyType
                {
                    TenantId = validation.TenantId,
                    PolicyName = request.DTO.PolicyName.Trim(),
                    Description = string.IsNullOrWhiteSpace(request.DTO.Description)
                        ? null
                        : request.DTO.Description.Trim(),
                    IsActive = request.DTO.IsActive,
                    PolicyTypeEnumVal = request.DTO.PolicyTypeEnumVal,
                    IsStructured = request.DTO.IsStructured,
                    HasPolicyDocUploaded = hasPolicyDocUploaded, // ✅ CORRECT
                    IsSoftDelete = false,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                var createdPolicyType =
                    await _unitOfWork.PolicyTypeRepository.CreatePolicyTypeAsync(policyType);

                if (createdPolicyType == null)
                    throw new ApiException("Policy type creation failed.", 500);

                // ===============================
                // 🔥 BULK INSERT MAPPING
                // ===============================
                var mappings = request.DTO.EmployeeTypeIds
                    .Distinct()
                    .Select(empTypeId => new UnStructuredPolicyTypeMappingWithEmployeeType
                    {
                        TenantId = validation.TenantId,
                        EmployeeTypeId = empTypeId,
                        PolicyTypeId = createdPolicyType.Id,
                        IsActive = true,
                        StartDate = DateTime.UtcNow,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow,
                        IsSoftDeleted = false
                    })
                    .ToList();

                if (mappings.Any())
                {
                    await _unitOfWork.UnStructuredEmployeePolicyTypeMappingRepository
                        .AddRangeAsync(mappings);
                }

                // ===============================
                // 🔥 SAVE DOCUMENT ENTRY
                // ===============================
                if (hasPolicyDocUploaded && !string.IsNullOrWhiteSpace(uploadedFileKey))
                {
                    var doc = new PolicyTypeDocument
                    {
                        TenantId = validation.TenantId,
                        PolicyTypeId = createdPolicyType.Id,
                        DocumentTitle = request.DTO.PolicyName.Trim(),
                        FileName = fileName,
                        FilePath = uploadedFileKey,
                        IsActive = request.DTO.IsActive,
                        IsSoftDeleted = false,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    };

                    await _unitOfWork.PolicyTypeDocumentRepository.AddAsync(doc);
                }

                await _unitOfWork.CommitTransactionAsync();
                // 🔥 FIX: DocDetails
                if (hasPolicyDocUploaded && !string.IsNullOrWhiteSpace(uploadedFileKey))
                {
                    createdPolicyType.DocDetails = new List<GetPolicyTypeDocumentResponseDTO>
                        {
                            new GetPolicyTypeDocumentResponseDTO
                            {
                                PolicyTypeId = createdPolicyType.Id,
                                DocumentTitle = request.DTO.PolicyName,
                                FileName = fileName,            
                                FilePath = _fileStorageService.GetFileUrl(uploadedFileKey),
                                IsActive = true
                            }
                        };
                }
                else
                {
                    createdPolicyType.DocDetails = new List<GetPolicyTypeDocumentResponseDTO>();
                }

                // 🔥 FIX: EmployeeTypeIds
                createdPolicyType.EmployeeTypeIds = request.DTO.EmployeeTypeIds.ToList();

                return ApiResponse<GetPolicyTypeResponseDTO>
                    .Success(createdPolicyType, "Policy type created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ CreatePolicyType failed");

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

                throw;
            }
        }

    }


}
