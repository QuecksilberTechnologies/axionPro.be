using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Interfaces;
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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers
{
    public class CreatePersonalInfoCommand : IRequest<ApiResponse<GetIdentityResponseDTO>>
    {
        public CreateIdentityRequestDTO DTO { get; set; }

        public CreatePersonalInfoCommand(CreateIdentityRequestDTO dto)
        {
            DTO = dto;
        }
    }

    //public class CreateIdentityInfoCommandHandler : IRequestHandler<CreatePersonalInfoCommand, ApiResponse<GetIdentityResponseDTO>>
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IMapper _mapper;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly ILogger<CreateIdentityInfoCommandHandler> _logger;
    //    private readonly ITokenService _tokenService;
    //    private readonly IPermissionService _permissionService;
    //    private readonly IConfiguration _config;
    //    private readonly IEncryptionService _encryptionService;
    //    private readonly IIdEncoderService _idEncoderService;
    //    private readonly IFileStorageService _fileStorageService;
    //    private readonly ICommonRequestService _commonRequestService;


    //    public CreateIdentityInfoCommandHandler(
    //        IUnitOfWork unitOfWork,
    //        IMapper mapper,
    //        IHttpContextAccessor httpContextAccessor,
    //        ILogger<CreateIdentityInfoCommandHandler> logger,
    //        ITokenService tokenService,
    //        IPermissionService permissionService,
    //        IConfiguration config,
    //        IEncryptionService encryptionService,
    //        IIdEncoderService idEncoderService,
    //        IFileStorageService fileStorageService, ICommonRequestService commonRequestService
    //    )
    //    {
    //        _unitOfWork = unitOfWork;
    //        _mapper = mapper;
    //        _httpContextAccessor = httpContextAccessor;
    //        _logger = logger;
    //        _tokenService = tokenService;
    //        _permissionService = permissionService;
    //        _config = config;
    //        _encryptionService = encryptionService;
    //        _idEncoderService = idEncoderService;
    //        _fileStorageService = fileStorageService;
    //        _commonRequestService = commonRequestService;
    //    }

    //    //public async Task<ApiResponse<GetIdentityResponseDTO>> Handle(CreatePersonalInfoCommand request, CancellationToken cancellationToken)
    //    //{
    //    //    string? savedFullPath = null;

    //    //    await _unitOfWork.BeginTransactionAsync();

    //    //    try
    //    //    {


    //    //        // 1️⃣ Common validation
    //    //        var validation = await _commonRequestService
    //    //            .ValidateRequestAsync(request.DTO.UserEmployeeId);

    //    //        if (!validation.Success)
    //    //            return ApiResponse<GetIdentityResponseDTO>
    //    //                .Fail(validation.ErrorMessage);

    //    //        // Assign decoded values coming from CommonRequestService
    //    //        request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
    //    //        request.DTO.Prop.TenantId = validation.TenantId;
    //    //        request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey,
    //    //            _idEncoderService);

    //    //        // ✅ Create  using repository
    //    //        var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
    //    //        if (!permissions.Contains("PersonalInfo"))
    //    //        {
    //    //            //await _unitOfWork.RollbackTransactionAsync();
    //    //            //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
    //    //        }
    //    //        // 🧩 STEP 4: Call Repository to get data 

    //    //        bool hasPanIdUploaded = false;
    //    //        bool hasAadharIdUploaded = false;
    //    //        bool hasPassportIdUploaded = false;

    //    //        // 🧩 STEP 2: File Upload (Aadhaar, PAN, Passport)
    //    //        string? aadharDocName = null;
    //    //        string? panDocName = null;
    //    //        string? passportDocName = null;


    //    //        string? passportDocPath = null;
    //    //        string? panDocPath = null;
    //    //        string? aadharDocPath = null;


    //    //        long tenantId = request.DTO.Prop.TenantId;


    //    //        var isHas = await _unitOfWork.EmployeeIdentityRepository.IsEmployeePersonalDetailExistsAsync(request.DTO.Prop.EmployeeId, null);
    //    //        if (isHas)
    //    //        {
    //    //            await _unitOfWork.RollbackTransactionAsync();
    //    //            return ApiResponse<GetIdentityResponseDTO>.Fail("Identity info already exists for this employee.");
    //    //        }

    //    //        // 🪪 Helper function for masking and safe file naming
    //    //        string MaskSensitiveNumber(string number)
    //    //        {
    //    //            if (string.IsNullOrWhiteSpace(number)) return "unknown";
    //    //            number = number.Trim();
    //    //            return number.Length > 4 ? $"XXX-{number[^4..]}" : $"XXX-{number}";
    //    //        }

    //    //        // 🔹 Aadhaar Upload
    //    //        if (request.DTO.AadhaarDocFile != null && request.DTO.AadhaarDocFile.Length > 0)
    //    //        {
    //    //            string maskedAadhar = MaskSensitiveNumber(request.DTO.AadhaarNumber ?? "aadhaar");

    //    //            using var ms = new MemoryStream();
    //    //            await request.DTO.AadhaarDocFile.CopyToAsync(ms);
    //    //            var fileBytes = ms.ToArray();

    //    //            string fileName = $"Aadhaar-{request.DTO.Prop.EmployeeId}_{maskedAadhar}-{DateTime.UtcNow:yyMMddHHmmss}.png";
    //    //            string folderPath = _fileStorageService.GetEmployeeFolderPath(tenantId, request.DTO.Prop.EmployeeId, "identity");

    //    //            aadharDocName = fileName;
    //    //            savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, folderPath);

    //    //            if (!string.IsNullOrEmpty(savedFullPath))
    //    //            {
    //    //                aadharDocPath = _fileStorageService.GetRelativePath(savedFullPath);
    //    //                hasAadharIdUploaded = true;
    //    //            }
    //    //        }

    //    //        // 🔹 PAN Upload
    //    //        if (request.DTO.PanDocFile != null && request.DTO.PanDocFile.Length > 0)
    //    //        {
    //    //            string maskedPan = MaskSensitiveNumber(request.DTO.PanNumber ?? "pan");

    //    //            using var ms = new MemoryStream();
    //    //            await request.DTO.PanDocFile.CopyToAsync(ms);
    //    //            var fileBytes = ms.ToArray();

    //    //            string fileName = $"PAN-{request.DTO.Prop.EmployeeId}_{maskedPan}-{DateTime.UtcNow:yyMMddHHmmss}.png";
    //    //            string folderPath = _fileStorageService.GetEmployeeFolderPath(tenantId, request.DTO.Prop.EmployeeId, "identity");

    //    //            panDocName = fileName;
    //    //            savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, folderPath);

    //    //            if (!string.IsNullOrEmpty(savedFullPath))
    //    //            {
    //    //                panDocPath = _fileStorageService.GetRelativePath(savedFullPath);
    //    //                hasPanIdUploaded = true;
    //    //            }
    //    //        }

    //    //        // 🔹 Passport Upload
    //    //        if (request.DTO.PassportDocFile != null && request.DTO.PassportDocFile.Length > 0)
    //    //        {
    //    //            string maskedPassport = MaskSensitiveNumber(request.DTO.PassportNumber ?? "passport");

    //    //            using var ms = new MemoryStream();
    //    //            await request.DTO.PassportDocFile.CopyToAsync(ms);
    //    //            var fileBytes = ms.ToArray();

    //    //            string fileName = $"Passport-{request.DTO.Prop.EmployeeId}_{maskedPassport}-{DateTime.UtcNow:yyMMddHHmmss}.png";
    //    //            string folderPath = _fileStorageService.GetEmployeeFolderPath(tenantId, request.DTO.Prop.EmployeeId, "identity");

    //    //            passportDocName = fileName;
    //    //            savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, folderPath);

    //    //            if (!string.IsNullOrEmpty(savedFullPath))
    //    //            {
    //    //                passportDocPath = _fileStorageService.GetRelativePath(savedFullPath);
    //    //                hasPassportIdUploaded = true;
    //    //            }
    //    //        }

    //    //        // 🧩 STEP 3: Mapping DTO → Entity
    //    //        var identityEntity = _mapper.Map<EmployeePersonalDetail>(request.DTO);

    //    //        // ✅ Manual assignments (not part of DTO)
    //    //        identityEntity.EmployeeId = request.DTO.Prop.EmployeeId;
    //    //        identityEntity.AddedById = request.DTO.Prop.UserEmployeeId;
    //    //        identityEntity.AddedDateTime = DateTime.UtcNow;
    //    //        identityEntity.IsActive = true;
    //    //        identityEntity.IsEditAllowed = true;
    //    //        identityEntity.IsInfoVerified = false;



    //    //        // 🪪 Document paths assignment (only if uploaded)
    //    //        if (hasAadharIdUploaded)
    //    //        {
    //    //            identityEntity.AadhaarDocPath = aadharDocPath;
    //    //            identityEntity.AadhaarDocName = aadharDocName;
    //    //        }
    //    //        identityEntity.HasAadhaarIdUploaded = hasAadharIdUploaded;
    //    //        if (hasPanIdUploaded)
    //    //        {
    //    //            identityEntity.PanDocPath = panDocPath;
    //    //            identityEntity.PanDocName = panDocName;
    //    //        }
    //    //        identityEntity.HasPanIdUploaded = hasPanIdUploaded;
    //    //        if (hasPassportIdUploaded)
    //    //        {
    //    //            identityEntity.PassportDocPath = passportDocPath;
    //    //            identityEntity.PassportDocName = passportDocName;
    //    //        }
    //    //        identityEntity.HasPassportIdUploaded = hasPassportIdUploaded;


    //    //        // 🧩 STEP 4: Save to Database
    //    //        var savedResponse = await _unitOfWork.EmployeeIdentityRepository.CreateAsync(identityEntity);

    //    //        var result = ProjectionHelper.ToGetIdentityResponseDTO(savedResponse, _idEncoderService, validation.Claims.TenantEncriptionKey);

    //    //        await _unitOfWork.CommitTransactionAsync();

    //    //        return new ApiResponse<GetIdentityResponseDTO>
    //    //        {
    //    //            IsSucceeded = true,
    //    //            Message = "Identity info added successfully.",
    //    //            Data = result
    //    //        };
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        await _unitOfWork.RollbackTransactionAsync();
    //    //        _logger.LogError(ex, "❌ Error while adding identity info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
    //    //        return ApiResponse<GetIdentityResponseDTO>.Fail($"Failed to add identity info: {ex.Message}");
    //    //    }
    //    //}

    //}

}
