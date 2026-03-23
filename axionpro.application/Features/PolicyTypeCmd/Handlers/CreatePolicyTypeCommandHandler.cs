using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Exceptions;
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
using System.Reflection;

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

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🔹 CreatePolicyType started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.PolicyType,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ CREATE POLICY TYPE
                // ===============================
                var policyType = new PolicyType
                {
                    TenantId = validation.TenantId,
                    PolicyName = request.DTO.PolicyName.Trim(),
                    Description = string.IsNullOrWhiteSpace(request.DTO.Description)
                        ? null
                        : request.DTO.Description.Trim(),
                    IsActive = request.DTO.IsActive,
                    IsStructured = request.DTO.IsStructured,
                    IsSoftDelete = false,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow
                };

                var createdPolicyType =
                    await _unitOfWork.PolicyTypeRepository.CreatePolicyTypeAsync(policyType);

                if (createdPolicyType == null)
                    throw new ApiException("Policy type creation failed.", 500);

                // ===============================
                // 5️⃣ FILE UPLOAD
                // ===============================
                if (request.DTO.FormFile != null && request.DTO.FormFile.Length > 0)
                {
                    string safeName = EncryptionSanitizer
                        .CleanEncodedInput(request.DTO.PolicyName ?? "policy")
                        .ToLower()
                        .Replace(" ", "_");

                    string fileName = $"company-policy-{safeName}-{DateTime.UtcNow:yyyyMMddHHmmss}";
                    string folderPath =
                        $"{ConstantValues.TenantFolder}-{validation.TenantId}/{ConstantValues.PoliciesFolder}";

                    uploadedFileKey = await _fileStorageService.UploadFileAsync(
                        request.DTO.FormFile,
                        folderPath,
                        fileName);

                    if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                    {
                        var doc = new CompanyPolicyDocument
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

                        await _unitOfWork.CompanyPolicyDocumentRepository.AddAsync(doc);
                    }
                }

                // ===============================
                // 6️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ PolicyType created successfully");

                return ApiResponse<GetPolicyTypeResponseDTO>
                    .Success(createdPolicyType, "Policy type created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ CreatePolicyType failed");

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


}
