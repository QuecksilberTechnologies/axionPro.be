using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;

using axionpro.domain.Entity; using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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

        public async Task<ApiResponse<GetPolicyTypeResponseDTO>> Handle(CreatePolicyTypeCommand request,    CancellationToken cancellationToken)
        {
            // 🔹 CLIENT PATTERN VARIABLES
            string? docPath = null;
            string? docName = null;
            bool hasPolicyDocUploaded = false;

            try
            {
                if (request.DTO == null)
                    return ApiResponse<GetPolicyTypeResponseDTO>
                        .Fail("Invalid request. PolicyType data is required.");

                 await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: Common validation
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<GetPolicyTypeResponseDTO>
                        .Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 🔑 STEP 2: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddPolicyType"))
                {
                    //return ApiResponse<GetPolicyTypeResponseDTO>
                    //    .Fail("You do not have permission to add policy types.");
                }
                GetCompanyPolicyDocumentResponseDTO companyPolicyDocument = new GetCompanyPolicyDocumentResponseDTO();

                // 🔹 STEP 3: CREATE POLICY TYPE (ALWAYS)
                var policyType = new PolicyType
                {
                    TenantId = request.DTO.Prop.TenantId,
                    PolicyName = request.DTO.PolicyName.Trim(),
                    Description = string.IsNullOrWhiteSpace(request.DTO.Description)
                        ? null
                        : request.DTO.Description.Trim(),
                    IsActive = request.DTO.IsActive,
                    IsStructured = request.DTO.IsStructured,
                    IsSoftDelete = false,
                    AddedById = request.DTO.Prop.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                var createdPolicyType = await _unitOfWork.PolicyTypeRepository.CreatePolicyTypeAsync(policyType);

                if (createdPolicyType == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<GetPolicyTypeResponseDTO>
                        .Fail("Policy type creation failed.");
                }

                // 🔹 STEP 4: FILE UPLOAD (CLIENT PATTERN – OPTIONAL)
                // 🔹 STEP 4: FILE UPLOAD (S3 CLEAN)
                if (request.DTO.FormFile != null &&
                    request.DTO.FormFile.Length > 0)
                {
                    try
                    {
                        string safePolicyName =
                            EncryptionSanitizer.CleanEncodedInput(
                                request.DTO.PolicyName ?? "policy")
                            .ToLower()
                            .Replace(" ", "_");

                        // 🔹 FILE NAME
                        docName =
                            $"company-policy-{safePolicyName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        // 🔹 FOLDER PATH (STANDARD)
                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.PoliciesFolder}";

                        // 🔹 UPLOAD
                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.FormFile,
                            folderPath,
                            docName);

                        if (!string.IsNullOrWhiteSpace(fileKey))
                        {
                            docPath = fileKey;
                            hasPolicyDocUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Policy document upload failed");

                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<GetPolicyTypeResponseDTO>
                            .Fail("File upload failed.");
                    }
                }
                // 🔹 STEP 5: INSERT COMPANY POLICY DOCUMENT (ONLY IF FILE UPLOADED)
                if (hasPolicyDocUploaded)
                {
                    

                    var companyPolicyDoc = new CompanyPolicyDocument
                    {
                        TenantId = validation.TenantId,
                        PolicyTypeId = createdPolicyType.Id, // 🔥 CRITICAL
                        DocumentTitle = request.DTO.PolicyName.Trim(),
                        FileName = docName!,
                        FilePath = docPath!,                                         
                        IsActive = request.DTO.IsActive,
                        IsSoftDeleted = false,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow
                    };

                    companyPolicyDocument= await _unitOfWork.CompanyPolicyDocumentRepository.AddAsync(companyPolicyDoc);
                }
                if (companyPolicyDocument != null)
                {
                    string baseUrl = _config["FileSettings:BaseUrl"] ?? string.Empty;

                    // 🔹 PolicyType info
                    createdPolicyType.DocDetails.PolicyTypeId = createdPolicyType.Id;

                    // 🔹 Document info
                    createdPolicyType.DocDetails.Id = companyPolicyDocument.Id;
                    createdPolicyType.DocDetails.DocumentTitle = companyPolicyDocument.DocumentTitle;
                    createdPolicyType.DocDetails.FileName = companyPolicyDocument.FileName;

                    // 🔹 Build absolute URL safely
                    if (!string.IsNullOrWhiteSpace(companyPolicyDocument.FileName))
                    {
                        createdPolicyType.DocDetails.URL = $"{baseUrl.TrimEnd('/')}/{companyPolicyDocument.FileName.TrimStart('/')}";
                    }
                    else
                    {
                        createdPolicyType.DocDetails.URL = null;
                    }
                }


                // 🔹 STEP 6: COMMIT (ONE TRANSACTION)
                   await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<GetPolicyTypeResponseDTO>
                    .Success(createdPolicyType, "Policy type created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating PolicyType with document");

                await _unitOfWork.RollbackTransactionAsync();

                return ApiResponse<GetPolicyTypeResponseDTO>
                    .Fail("An unexpected error occurred while creating policy type.");
            }
        }
    }


}
