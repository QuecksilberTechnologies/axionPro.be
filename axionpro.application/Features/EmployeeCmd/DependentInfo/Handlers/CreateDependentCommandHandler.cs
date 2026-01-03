using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Command;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION (SAME AS CONTACT)
                var validation =
                    await _commonRequestService.ValidateRequestAsync(
                        request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetDependentResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService
                    );

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddDependentInfo"))
                {
                    // optional hard-stop
                    // return ApiResponse<List<GetDependentResponseDTO>>
                    //     .Fail("You do not have permission to add dependent info.");
                }

                // 🧩 STEP 4: Validation
                if (string.IsNullOrWhiteSpace(request.DTO.Relation))
                    return ApiResponse<List<GetDependentResponseDTO>>
                        .Fail("Dependent relation cannot be empty.");

                if (!Regex.IsMatch(request.DTO.Relation, @"^[a-zA-Z\s]+$"))
                    return ApiResponse<List<GetDependentResponseDTO>>
                        .Fail("Dependent relation must contain only letters.");

                // 📎 STEP 5: File Upload (if any)
                string? docPath = null;
                string? docName = null;
                bool hasProofUploaded = false;

                if (request.DTO.ProofFile != null &&
                    request.DTO.ProofFile.Length > 0)
                {
                    var safeName =
                        EncryptionSanitizer.CleanEncodedInput(
                            request.DTO.Relation.Trim().Replace(" ", "").ToLower());

                    using var ms = new MemoryStream();
                    await request.DTO.ProofFile.CopyToAsync(ms, cancellationToken);

                    string fileName =
                        $"proof-{request.DTO.Prop.EmployeeId}_{safeName}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                    string folderPath =
                        _fileStorageService.GetEmployeeFolderPath(
                            request.DTO.Prop.TenantId,
                            request.DTO.Prop.EmployeeId,
                            "dependent");

                    var savedPath =
                        await _fileStorageService.SaveFileAsync(
                            ms.ToArray(), fileName, folderPath);

                    if (!string.IsNullOrEmpty(savedPath))
                    {
                        docPath = _fileStorageService.GetRelativePath(savedPath);
                        docName = fileName;
                        hasProofUploaded = true;
                    }
                }

                // 🧱 STEP 6: Map Entity
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

                // 💾 STEP 7: Save
                var responseDTO =
                    await _unitOfWork.EmployeeDependentRepository
                        .CreateAsync(entity);

                // 🔐 STEP 8: Encrypt IDs
                var encryptedList =
                    ProjectionHelper.ToGetDependentResponseDTOs(
                        responseDTO.Items,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey, _configuration
                    );

                await _unitOfWork.CommitTransactionAsync();

                // 📦 STEP 9: Response
                return new ApiResponse<List<GetDependentResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = encryptedList
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error while adding dependent info for EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                return ApiResponse<List<GetDependentResponseDTO>>
                    .Fail("Failed to add dependent info.",
                          new List<string> { ex.Message });
            }
        }
    }


}


