using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOS.Employee.Sensitive;
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

namespace axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers
{
    public class UpdateIdentityInfoCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateIdentityReqestDTO DTO { get; set; }

        public UpdateIdentityInfoCommand(UpdateIdentityReqestDTO dto)
        {
            DTO = dto;
        }

    }
    public class UpdateIdentityInfoCommandHandler : IRequestHandler<UpdateIdentityInfoCommand, ApiResponse<bool>>
    {
        private readonly IEmployeeIdentityRepository _empIdentityRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateIdentityInfoCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;


        public UpdateIdentityInfoCommandHandler(
            IEmployeeIdentityRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateIdentityInfoCommandHandler> logger,
            IMapper mapper,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IEncryptionService encryptionService,
            ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, IFileStorageService fileStorageService)
        {
            _empIdentityRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateIdentityInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // -------------------------------------------------
                // 1️⃣ COMMON VALIDATION
                // -------------------------------------------------
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(request.DTO.EmployeeId,   validation.Claims.TenantEncriptionKey, _idEncoderService      );

                // -------------------------------------------------
                // 2️⃣ PERMISSION CHECK
                // -------------------------------------------------
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("UpdateBankInfo"))
                {
                    // return ApiResponse<bool>.Fail("You do not have permission to update bank info.");
                }

                // -------------------------------------------------
                // 3️⃣ FETCH EXISTING Identity RECORD
                // -------------------------------------------------
                var emp = await _unitOfWork.EmployeeIdentityRepository.GetSingleRecordAsync(request.DTO.Id, true);

                if (emp == null)
                    return ApiResponse<bool>.Fail("Employee bank record not found.");

                var dto = request.DTO;

                // -------------------------------------------------
                // 4️⃣ PARTIAL FIELD UPDATES
                // -------------------------------------------------

              
                // 🔹 STRING FIELDS
                if (!string.IsNullOrWhiteSpace(dto.AadhaarNumber))
                    emp.AadhaarNumber = dto.AadhaarNumber.Trim();

                if (!string.IsNullOrWhiteSpace(dto.PanNumber))
                    emp.PanNumber = dto.PanNumber.Trim();

                if (!string.IsNullOrWhiteSpace(dto.PassportNumber))
                    emp.PassportNumber = dto.PassportNumber.Trim();

                if (!string.IsNullOrWhiteSpace(dto.DrivingLicenseNumber))
                    emp.DrivingLicenseNumber = dto.DrivingLicenseNumber.Trim();

                if (!string.IsNullOrWhiteSpace(dto.VoterId))
                    emp.VoterId = dto.VoterId.Trim();

                if (!string.IsNullOrWhiteSpace(dto.BloodGroup))
                    emp.BloodGroup = dto.BloodGroup.Trim();

                if (!string.IsNullOrWhiteSpace(dto.Nationality))
                    emp.Nationality = dto.Nationality.Trim();

                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName))
                    emp.EmergencyContactName = dto.EmergencyContactName.Trim();

                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactRelation))
                    emp.EmergencyContactRelation = dto.EmergencyContactRelation.Trim();

                if (!string.IsNullOrWhiteSpace(dto.EmergencyContactNumber))
                    emp.EmergencyContactNumber = dto.EmergencyContactNumber.Trim();

                // 🔹 BOOL / NULLABLE BOOL
                if (dto.MaritalStatus.HasValue)
                    emp.MaritalStatus = dto.MaritalStatus.Value;

                if (dto.HasEPFAccount != emp.HasEPFAccount)
                    emp.HasEPFAccount = dto.HasEPFAccount;

                if (!string.IsNullOrWhiteSpace(dto.UANNumber))
                    emp.UANNumber = dto.UANNumber.Trim();


        //public IFormFile? AadhaarDocFile { get; set; }
        //public IFormFile? PanDocFile { get; set; }
        //public IFormFile? PassportDocFile { get; set; }
                // ------------------------ FILE HANDLING (OPTIONAL) ------------------------
                if (request.DTO.AadhaarDocFile is { Length: > 0 })
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(emp.AadhaarDocPath))
                        {
                            string oldPath = emp.AadhaarDocPath; // This may be URL or relative path

                            bool fileDeleted = false;

                            // Case 1: Physical local/server file path
                            string physicalFullPath = _fileStorageService.GetRelativePath(oldPath);

                            if (!string.IsNullOrWhiteSpace(physicalFullPath) && File.Exists(physicalFullPath))
                            {
                                File.Delete(physicalFullPath);
                                fileDeleted = true;
                                _logger.LogInformation("📌 Local/Server education document deleted: {File}", physicalFullPath);
                            }

                            // Case 2: Remote CDN/HTTP/Cloud File
                            if (!fileDeleted && Uri.TryCreate(oldPath, UriKind.Absolute, out Uri? uri)
                                && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp))
                            {
                                using var client = new HttpClient();
                                var response = await client.DeleteAsync(uri);

                                if (response.IsSuccessStatusCode)
                                {
                                    fileDeleted = true;
                                    _logger.LogInformation("🌍 Remote education document deleted: {File}", uri);
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ Remote file delete attempt failed for: {File}", uri);
                                }
                            }

                            if (!fileDeleted)
                                _logger.LogWarning("⚠️ Delete attempted but file not found: {File}", oldPath);
                        }

                        // Now upload new file
                        using var ms = new MemoryStream();
                        await request.DTO.AadhaarDocFile.CopyToAsync(ms);

                        string newFileName = $"Sensi-{emp.EmployeeId}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                        string folderPath = _fileStorageService.GetEmployeeFolderPath(
                            request.DTO.Prop.TenantId,
                            emp.EmployeeId,
                            "identities"
                        );

                        string savedPath = await _fileStorageService.SaveFileAsync(ms.ToArray(), newFileName, folderPath);

                        emp.AadhaarDocPath = _fileStorageService.GetRelativePath(savedPath);
                        emp.AadhaarDocName = newFileName;
                        emp.HasAadhaarIdUploaded = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error replacing identity document for employee {Emp}", emp.EmployeeId);
                        return ApiResponse<bool>.Fail("File upload failed, please try again.");
                    }

                }

                emp.UpdatedById = request.DTO.Prop.EmployeeId;
                emp.UpdatedDateTime = DateTime.UtcNow;

                // ✅ Step 5: Fetch Employee Identity record
                var isUpdated = await _empIdentityRepository.UpdateIdentity(emp);

                if (!isUpdated)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Employee identity not updated.");

                }
                           

                return ApiResponse<bool>.Success(true, $"Data updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating identity info.");
                return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }
    }

}
