using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
                if (request.DTO.ProofFile is { Length: > 0 })
                {
                    try
                    {
                        // 🔹 Build file name
                        string safeRelation =
                            Enum.IsDefined(typeof(RelationDependant), dependent.Relation)
                                ? ((RelationDependant)dependent.Relation).ToString()
                                : "dependent";

                        string fileName =
                            $"DependentProof-{dependent.EmployeeId}_{safeRelation}_{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                        // 🔹 Get employee dependent folder
                        string folderPath =
                            _fileStorageService.GetEmployeeFolderPath(
                                validation.TenantId,
                                dependent.EmployeeId,
                                "dependent");

                        // 🔹 Read file
                        using var ms = new MemoryStream();
                        await request.DTO.ProofFile.CopyToAsync(ms);

                        // 🔹 Save file
                        string savedFullPath =
                            await _fileStorageService.SaveFileAsync(
                                ms.ToArray(),
                                fileName,
                                folderPath);

                        // 🔹 Update entity (NO DELETE of OLD FILE)
                        dependent.FilePath = _fileStorageService.GetRelativePath(savedFullPath);
                        dependent.FileName = fileName;
                        dependent.FileType = 2; // pdf
                        dependent.HasProofUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "❌ Error uploading dependent proof | DependentId: {Id}",
                            dependent.Id);

                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.Fail(
                            "Failed to upload dependent proof file. Please try again.");
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

