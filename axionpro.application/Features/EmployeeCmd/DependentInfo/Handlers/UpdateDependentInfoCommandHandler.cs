using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Exceptions;
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
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("UpdateDependent started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                if (request.DTO.Id <= 0)
                    throw new ValidationErrorException("Invalid dependent id.");

                long loggedInEmployeeId = validation.UserEmployeeId;

                // ===============================
                // 3️⃣ PERMISSION (YOUR FIXED PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update dependent.");

                // ===============================
                // 4️⃣ FETCH EXISTING
                // ===============================
                var dependent =
                    await _unitOfWork.EmployeeDependentRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (dependent == null)
                    throw new ApiException("Dependent record not found.", 404);

                // ===============================
                // 5️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                var dto = request.DTO;

                // ===============================
                // 6️⃣ PARTIAL UPDATE
                // ===============================
                if (!string.IsNullOrWhiteSpace(dto.DependentName))
                    dependent.DependentName = dto.DependentName.Trim();

                if (dto.Relation.HasValue)
                {
                    if (!Enum.IsDefined(typeof(RelationDependant), dto.Relation.Value))
                        throw new ValidationErrorException("Invalid dependent relation.");

                    dependent.Relation = dto.Relation.Value;
                }

                if (dto.DateOfBirth.HasValue)
                    dependent.DateOfBirth = dto.DateOfBirth.Value;

                if (dto.IsCoveredInPolicy.HasValue)
                    dependent.IsCoveredInPolicy = dto.IsCoveredInPolicy.Value;

                if (dto.IsMarried.HasValue)
                    dependent.IsMarried = dto.IsMarried.Value;

                if (!string.IsNullOrWhiteSpace(dto.Remark))
                    dependent.Remark = dto.Remark.Trim();

                if (!string.IsNullOrWhiteSpace(dto.Description))
                    dependent.Description = dto.Description.Trim();

                // ===============================
                // 7️⃣ FILE UPLOAD (SAFE)
                // ===============================
                if (dto.ProofFile != null && dto.ProofFile.Length > 0)
                {
                    try
                    {
                        string relationName =
                            Enum.IsDefined(typeof(RelationDependant), dependent.Relation)
                                ? ((RelationDependant)dependent.Relation).ToString().ToLower()
                                : "dependent";

                        relationName = relationName.Replace(" ", "_");

                        string fileName =
                            $"proof-{dependent.EmployeeId}-{relationName}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{validation.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{dependent.EmployeeId}/dependent";

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            dto.ProofFile,
                            folderPath,
                            fileName);

                        dependent.FilePath = uploadedFileKey;
                        dependent.FileName = fileName;
                        dependent.FileType = 2;
                        dependent.HasProofUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Dependent file upload failed");
                        throw new ApiException("File upload failed.", 500);
                    }
                }

                // ===============================
                // 🧾 AUDIT
                // ===============================
                dependent.UpdatedById = loggedInEmployeeId;
                dependent.UpdatedDateTime = DateTime.UtcNow;

                // ===============================
                // 8️⃣ SAVE
                // ===============================
                var updated =
                    await _unitOfWork.EmployeeDependentRepository
                        .UpdateAsync(dependent);

                if (!updated)
                    throw new ApiException("Failed to update dependent.", 500);

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("UpdateDependent success");

                return ApiResponse<bool>.Success(true, "Dependent updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error updating dependent");

                // 🧹 FILE CLEANUP (CRITICAL)
                if (!string.IsNullOrEmpty(uploadedFileKey))
                {
                    try
                    {
                        await _fileStorageService.DeleteFileAsync(uploadedFileKey);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogError(cleanupEx, "File cleanup failed");
                    }
                }

                throw; // 🚨 MUST
            }
        }
    }

}

