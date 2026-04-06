using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class CreateDependentCommand : IRequest<ApiResponse<List<GetDependentResponseDTO>>>
    {
        public CreateDependentRequestDTO DTO { get; set; }

        public CreateDependentCommand(CreateDependentRequestDTO dto)
        {
            DTO = dto;
        }

    }

    public class CreateDependentCommandHandler
    : IRequestHandler<CreateDependentCommand, ApiResponse<List<GetDependentResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDependentCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IConfiguration _configuration;


        public CreateDependentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateDependentCommandHandler> logger,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService,
            ICommonRequestService commonRequestService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
            _commonRequestService = commonRequestService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<List<GetDependentResponseDTO>>> Handle(
      CreateDependentCommand request,
      CancellationToken cancellationToken)
        {
            string? uploadedFileKey = null;

            try
            {
                _logger.LogInformation("CreateDependent started");

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

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to add dependent.");

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 5️⃣ FILE UPLOAD
                // ===============================
                string? docPath = null;
                string? docName = null;
                bool hasProofUploaded = false;

                if (request.DTO.ProofFile != null &&
                    request.DTO.ProofFile.Length > 0)
                {
                    try
                    {
                        string relation =
                            request.DTO.Relation.ToString() ?? "doc";

                        string fileName =
                            $"proof-{request.DTO.Prop.EmployeeId}-{relation}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                        string folderPath =
                            $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                            $"{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/{ConstantValues.DependentFolder}";

                        uploadedFileKey = await _fileStorageService.UploadFileAsync(
                            request.DTO.ProofFile,
                            folderPath,
                            fileName);

                        if (!string.IsNullOrWhiteSpace(uploadedFileKey))
                        {
                            docPath = uploadedFileKey;
                            docName = fileName;
                            hasProofUploaded = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Dependent proof upload failed");
                        throw new ApiException("File upload failed.", 500);
                    }
                }

                // ===============================
                // 6️⃣ MAP ENTITY
                // ===============================
                var entity = _mapper.Map<EmployeeDependent>(request.DTO);

                entity.EmployeeId = request.DTO.Prop.EmployeeId;
                entity.AddedById = request.DTO.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;

                entity.IsActive = true;
                entity.IsEditAllowed = true;
                entity.IsInfoVerified = false;

                entity.FileType = hasProofUploaded ? 2 : 0;
                entity.FilePath = docPath;
                entity.FileName = docName;
                entity.HasProofUploaded = hasProofUploaded;
                entity.IsCoveredInPolicy =false; // Default, can be updated later by HR/Admin
                // ===============================
                // 7️⃣ SAVE
                // ===============================
                var responseDTO =
                    await _unitOfWork.EmployeeDependentRepository
                        .CreateAsync(entity);

                if (responseDTO == null)
                    throw new ApiException("Failed to create dependent.", 500);

                // ===============================
                // 8️⃣ PROJECTION
                // ===============================
                var encryptedList =
                    ProjectionHelper.ToGetDependentResponseDTOs(
                        responseDTO.Items,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey,
                        _configuration, _fileStorageService
                    );

                // ===============================
                // 9️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CreateDependent success");

                return ApiResponse<List<GetDependentResponseDTO>>
                    .Success(encryptedList, "Dependent created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error creating dependent");

                // 🧹 FILE CLEANUP (CRITICAL 🚨)
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


