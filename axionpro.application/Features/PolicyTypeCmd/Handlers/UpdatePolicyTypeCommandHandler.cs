using AutoMapper;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{
    public class UpdatePolicyTypeCommand : IRequest<ApiResponse<bool>>
    {
        public UpdatePolicyTypeDTO DTO { get; set; }

        public UpdatePolicyTypeCommand(UpdatePolicyTypeDTO dTO)
        {
            this.DTO = dTO;
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
            // 🧩 Variables for optional document handling
            string? docPath = null;
            string? docName = null;
            bool hasFileUploaded = false;

            try
            {
                // ❌ Safety check
                if (request.DTO == null)
                    return ApiResponse<bool>.Fail("Invalid request.");

                // 🔐 STEP 1: Common validation (Tenant, User, Role etc.)
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // 🔑 STEP 2: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("UpdatePolicyType"))
                    return ApiResponse<bool>.Fail("Permission denied.");

                // 🔄 STEP 3: Start DB transaction
                await _unitOfWork.BeginTransactionAsync();

                // 🔍 STEP 4: Fetch existing PolicyType
                var policyType =
                    await _unitOfWork.PolicyTypeRepository
                        .GetPolicyTypeByIdAsync(request.DTO.Id);

                if (policyType == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Policy type not found.");
                }

                // ✏️ STEP 5: Update PolicyType fields
                policyType.PolicyName = request.DTO.PolicyName.Trim();
                policyType.Description = string.IsNullOrWhiteSpace(request.DTO.Description)
                    ? null
                    : request.DTO.Description.Trim();
                policyType.IsActive = request.DTO.IsActive;
                policyType.UpdateById = validation.UserEmployeeId;
                policyType.UpdateDateTime = DateTime.UtcNow;

                bool policyUpdated =
                    await _unitOfWork.PolicyTypeRepository
                        .UpdatePolicyTypeAsync(policyType);

                if (!policyUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Policy type update failed.");
                }

                // 📂 STEP 6: OPTIONAL file upload
                if (request.DTO.FormFile != null && request.DTO.FormFile.Length > 0)
                {
                    string safeName =
                        EncryptionSanitizer.CleanEncodedInput(
                            policyType.PolicyName.Replace(" ", "").ToLower());

                    using var ms = new MemoryStream();
                    await request.DTO.FormFile.CopyToAsync(ms, cancellationToken);

                    string fileName =
                        $"Company-Policy-{validation.TenantId}_{safeName}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                    string folderPath =
                        _fileStorageService.GetTenantFolderPath(
                            validation.TenantId,
                            "policies");

                    var savedPath =
                        await _fileStorageService.SaveFileAsync(
                            ms.ToArray(),
                            fileName,
                            folderPath);

                    if (!string.IsNullOrWhiteSpace(savedPath))
                    {
                        docName = fileName;
                        docPath = _fileStorageService.GetRelativePath(savedPath);
                        hasFileUploaded = true;
                    }
                }

                // 🧾 STEP 7: DOCUMENT UPSERT (Insert or Update)
                if (hasFileUploaded)
                {
                    // 🔍 Check if document already exists for this PolicyType
                    var existingDoc =
                        await _unitOfWork.CompanyPolicyDocumentRepository.GetByIdAsync(policyType.Id, validation.TenantId, request.DTO.IsActive);

                    if (existingDoc == null)
                    {
                        // ➕ INSERT (document was never uploaded before)
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
                        // ✏️ UPDATE (replace old document)
                        existingDoc.FileName = docName!;
                        existingDoc.FilePath = docPath!;
                        existingDoc.UpdatedById = validation.UserEmployeeId;
                        existingDoc.UpdatedDateTime = DateTime.UtcNow;

                        await _unitOfWork.CompanyPolicyDocumentRepository
                            .UpdateAsync(existingDoc);
                    }
                }

                // ✅ STEP 8: Commit transaction
                await _unitOfWork.CommitAsync();

                return ApiResponse<bool>
                    .Success(true, "Policy type updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdatePolicyType failed");

                await _unitOfWork.RollbackTransactionAsync();

                return ApiResponse<bool>
                    .Fail("Unexpected error occurred.");
            }
        }


    }
}



