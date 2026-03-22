using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class UpdateDependentCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateDependentRequestDTO DTO { get; set; }

        public UpdateDependentCommand(UpdateDependentRequestDTO dto)
        {
            DTO = dto;
        }

    }

    public class UpdateDependentInfoCommandHandler
     : IRequestHandler<UpdateDependentCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateDependentInfoCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _configuration;

        public UpdateDependentInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UpdateDependentInfoCommandHandler> logger,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService,
            ICommonRequestService commonRequestService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;

        }

        public async Task<ApiResponse<bool>> Handle(
            UpdateDependentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                long loggedInEmployeeId = validation.UserEmployeeId;

                // 🔎 STEP 2: Validate Dependent Id
                if (request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid dependent id.");

                // 🔑 STEP 3: Permission Check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("UpdateDependentInfo"))
                {
                    // optional strict block
                    // return ApiResponse<bool>.Fail("Permission denied.");
                }

                // 📦 STEP 4: Fetch existing dependent
                var dependent =
                    await _unitOfWork.EmployeeDependentRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (dependent == null)
                    return ApiResponse<bool>.Fail("Dependent record not found.");

                // =====================================================
                // 📂 DEPENDENT PROOF CHECK (CLEAN & SAFE)
                // =====================================================
              
               
                // =====================================================
                // 🔄 PARTIAL UPDATE (NULL SAFE)
                // =====================================================

                // Dependent Name (string)
                if (request.DTO.DependentName != null)
                {
                    dependent.DependentName =
                        string.IsNullOrWhiteSpace(request.DTO.DependentName)
                            ? dependent.DependentName
                            : request.DTO.DependentName.Trim();
                }

                // Relation (INT enum validation)
                if (request.DTO.Relation.HasValue)
                {
                    int relationValue = request.DTO.Relation.Value;

                    if (!Enum.IsDefined(typeof(RelationDependant), relationValue))
                        return ApiResponse<bool>.Fail("Invalid dependent relation value.");

                    dependent.Relation = relationValue;
                }

                // =====================================================
                // 📂 DEPENDENT PROOF FILE UPLOAD (AUDIT SAFE)
                // =====================================================
                // =====================================================
                // 📂 DEPENDENT PROOF FILE UPLOAD (S3 - NO DELETE)
                // =====================================================
                if (request.DTO.ProofFile != null &&
                    request.DTO.ProofFile.Length > 0)
                {
                    try
                    {
                        // 🔹 RELATION NAME SAFE
                        string relationName =
                            Enum.IsDefined(typeof(RelationDependant), dependent.Relation)
                                ? ((RelationDependant)dependent.Relation).ToString().ToLower()
                                : "dependent";

                        relationName = relationName.Replace(" ", "_");

                        // 🔹 FILE NAME
                        string fileName =
                            $"proof-{dependent.EmployeeId}-{relationName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        // 🔹 FOLDER PATH (STANDARD RULE)
                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{dependent.EmployeeId}/dependent";

                        // 🔹 UPLOAD (DIRECT S3)
                        var fileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.ProofFile,
                            folderPath,
                            fileName);

                        // 🔹 UPDATE ENTITY (OLD FILE PRESERVED)
                        dependent.FilePath = fileKey;
                        dependent.FileName = fileName;
                        dependent.FileType = 2; // pdf
                        dependent.HasProofUploaded = true;

                        _logger.LogInformation("Dependent proof uploaded. Old file preserved.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Error uploading dependent proof | DependentId: {Id}",
                            dependent.Id);

                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.Fail("File upload failed.");
                    }
                }

                // Date of Birth
                if (request.DTO.DateOfBirth.HasValue)
                    dependent.DateOfBirth = request.DTO.DateOfBirth.Value;

                // Is Covered
                if (request.DTO.IsCoveredInPolicy.HasValue)
                    dependent.IsCoveredInPolicy = request.DTO.IsCoveredInPolicy.Value;

                // Is Married
                if (request.DTO.IsMarried.HasValue)
                    dependent.IsMarried = request.DTO.IsMarried.Value;

                // Remark
                if (request.DTO.Remark != null)
                {
                    dependent.Remark =
                        string.IsNullOrWhiteSpace(request.DTO.Remark)
                            ? dependent.Remark
                            : request.DTO.Remark.Trim();
                }

                // Description
                if (request.DTO.Description != null)
                {
                    dependent.Description =
                        string.IsNullOrWhiteSpace(request.DTO.Description)
                            ? dependent.Description
                            : request.DTO.Description.Trim();
                }

                // 🧾 AUDIT
                dependent.UpdatedById = loggedInEmployeeId;
                dependent.UpdatedDateTime = DateTime.UtcNow;
               


                // 💾 SAVE
                bool updated =
                    await _unitOfWork.EmployeeDependentRepository
                        .UpdateAsync(dependent);

                if (!updated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to update dependent info.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Dependent updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "❌ Error updating dependent info");

                return ApiResponse<bool>.Fail(
                    "Unexpected error occurred.",
                    new List<string> { ex.Message });
            }

        }
    }

}

