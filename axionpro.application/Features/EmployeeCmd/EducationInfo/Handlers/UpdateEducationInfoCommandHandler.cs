// ******************************************************
//  FINAL OPTIMIZED VERSION — CLEAN + SAFE + COMMENTED
// ******************************************************

using AutoMapper;
using axionpro.application.Common.Attributes;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class UpdateEducationInfoCommand : IRequest<ApiResponse<bool>>
    {
        public GenericMultiFieldUpdateRequestDTO DTO { get; set; }

        public UpdateEducationInfoCommand(GenericMultiFieldUpdateRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class UpdateEducationInfoCommandHandler
        : IRequestHandler<UpdateEducationInfoCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateEducationInfoCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public UpdateEducationInfoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateEducationInfoCommandHandler> logger,
            IMapper mapper,
            IPermissionService permissionService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.DTO ?? throw new ArgumentNullException(nameof(request.DTO));

                // -----------------------------------------------
                // 1) TOKEN VALIDATION
                // -----------------------------------------------
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                long loggedInEmpId = await _unitOfWork.CommonRepository
                    .ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);

                // -----------------------------------------------
                // 2) BASIC CHECK (fields + files required)
                // -----------------------------------------------
                if ((dto.FieldsToUpdate == null || dto.FieldsToUpdate.Count == 0)
                    && (dto.FilesToUpdate == null || dto.FilesToUpdate.Count == 0))
                {
                    return ApiResponse<bool>.UpdatedFail("No fields provided to update.");
                }

                // -----------------------------------------------
                // 3) DECRYPT IDs
                // -----------------------------------------------
                string finalKey = EncryptionSanitizer.SuperSanitize(tokenClaims.TenantEncriptionKey);

                long decryptedUserEmpId = _idEncoderService.DecodeId(dto.UserEmployeeId, finalKey);
                long decryptedEmployeeId = _idEncoderService.DecodeId(dto.EmployeeId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(
                    EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);

                long decryptedRecordId = SafeParser.TryParseLong(dto.Id ?? dto.Id);

                // -----------------------------------------------
                // 4) FETCH EXISTING RECORD
                // -----------------------------------------------
                var existingRecord = await _unitOfWork.EmployeeEducationRepository
                    .GetSingleRecordAsync(decryptedRecordId, true);

                if (existingRecord == null)
                    return ApiResponse<bool>.UpdatedFail("Education record not found.");

                // -----------------------------------------------
                // 5) FIND DOC FILE NAME — FINAL OPTIMIZED LOGIC
                // -----------------------------------------------
                string? docFileName = string.Empty;

                // CASE 1: Degree updated in request
                if (dto.FieldsToUpdate != null && dto.FieldsToUpdate.Any())
                {
                    var degreeField = dto.FieldsToUpdate
                        .FirstOrDefault(x =>
                            x.FieldName?.Trim().Equals("Degree", StringComparison.OrdinalIgnoreCase) == true
                        );

                    if (degreeField != null)
                        docFileName = degreeField.FieldValue?
                                        .Trim()
                                        .Replace(" ", "_")
                                        .ToLower();
                }

                // CASE 2: Degree not provided in update request → fallback
                if (string.IsNullOrWhiteSpace(docFileName) && !string.IsNullOrWhiteSpace(existingRecord.Degree))
                {
                    docFileName = existingRecord.Degree
                        .Trim()
                        .Replace(" ", "_")
                        .ToLower();
                }



                // -----------------------------------------------
                // 6) FILE UPLOAD HANDLING
                // -----------------------------------------------
                if (dto.FilesToUpdate != null && dto.FilesToUpdate.Count > 0)
                {
                    foreach (var fileItem in dto.FilesToUpdate)
                    {
                        if (fileItem.FieldValue == null)
                            continue;

                        // If docFileName still empty → validation fail
                        if (string.IsNullOrWhiteSpace(docFileName))
                        {
                            return ApiResponse<bool>.Fail(
                                "Please provide 'Degree' before uploading a document."
                            );
                        }
                        string? fullFolderPath_delete = _fileStorageService.GetEmployeeFolderPath(decryptedTenantId, decryptedEmployeeId, "education");
                       // fullFolderPath_delete= EncryptionSanitizer.CleanEncodedInput(fullFolderPath_delete);
                        // ---- DELETE OLD FILE ----
                        if (!string.IsNullOrWhiteSpace(existingRecord.FilePath))
                        {
                            
                            try
                            {

                                //  string oldFullPath = _fileStorageService.GetRelativePath(existingRecord.FilePath);
                                string? fullFilePath = _fileStorageService.GenerateFullFilePath(fullFolderPath_delete, existingRecord.FileName);
                                
                                if (System.IO.File.Exists(fullFilePath))
                                {
                                    System.IO.File.Delete(fullFilePath);

                                    _logger.LogInformation(
                                        "Old education document deleted. Path: {Path}",
                                        fullFilePath
                                    );
                                }
                                else
                                {
                                    _logger.LogWarning(
                                        "Old file delete attempted but not found. Path: {Path}",
                                        fullFilePath
                                    );
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error deleting old document.");
                            }
                        }

                        // ---- SAVE NEW FILE ----
                        using var ms = new MemoryStream();
                        await fileItem.FieldValue.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();

                        string fileName =
                            $"EDU-{existingRecord.EmployeeId}_{docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";

                        string fullFolderPath = _fileStorageService.GetEmployeeFolderPath(
                            decryptedTenantId,
                            decryptedEmployeeId,
                            "education"
                        );

                        string savedPath = await _fileStorageService.SaveFileAsync(
                            fileBytes,
                            fileName,
                            fullFolderPath
                        );

                        if (!string.IsNullOrWhiteSpace(savedPath))
                        {
                            
                            existingRecord.FilePath = _fileStorageService.GetRelativePath(savedPath);
                            existingRecord.HasEducationDocUploded = true;
                            existingRecord.FileName = fileName;
                        }
                    }
                }

                // -----------------------------------------------
                // 7) READ-ONLY FIELD CHECK
                // -----------------------------------------------
                var readOnlyMap = typeof(EmployeeEducationEditableFieldsDTO)
                    .GetProperties()
                    .ToDictionary(
                        p => p.Name,
                        p => p.GetCustomAttribute<AccessControlAttribute>()?.ReadOnly ?? false
                    );

                var readOnlyFields = new List<string>();

                foreach (var item in dto.FieldsToUpdate)
                {
                    var prop = typeof(EmployeeEducation)
                        .GetProperty(item.FieldName,
                            BindingFlags.IgnoreCase |
                            BindingFlags.Public |
                            BindingFlags.Instance);

                    if (prop == null) continue;

                    if (readOnlyMap.TryGetValue(prop.Name, out bool isRO) && isRO)
                        readOnlyFields.Add(prop.Name);
                }

                if (readOnlyFields.Any())
                {
                    return ApiResponse<bool>.UpdatedFail(
                        $"These fields are read-only: {string.Join(", ", readOnlyFields)}"
                    );
                }

                // -----------------------------------------------
                // 8) APPLY FIELDS UPDATE (WITH AUTO-TRIM)
                // -----------------------------------------------
                foreach (var item in dto.FieldsToUpdate)
                {
                    var prop = typeof(EmployeeEducation)
                        .GetProperty(item.FieldName,
                            BindingFlags.IgnoreCase |
                            BindingFlags.Public |
                            BindingFlags.Instance);

                    if (prop == null)
                        continue;

                    object? rawValue = item.FieldValue;

                    // 🔥 AUTO TRIM FOR STRING VALUES
                    if (rawValue is string str)
                        rawValue = str.Trim();

                    if (!TryConvertObjectToValue.TryConvertValue(rawValue, prop.PropertyType, out var converted))
                        return ApiResponse<bool>.UpdatedFail($"Invalid value for '{item.FieldName}'.");

                    prop.SetValue(existingRecord, converted);
                }

                // -----------------------------------------------
                // 9) AUDIT TRAIL
                // -----------------------------------------------
                existingRecord.UpdatedById = decryptedUserEmpId;
                existingRecord.UpdatedDateTime = DateTime.UtcNow;

                // -----------------------------------------------
                // 10) SAVE CHANGES
                // -----------------------------------------------
                bool isSuccess =
                    await _unitOfWork.EmployeeEducationRepository
                        .UpdateEmployeeFieldAsync(existingRecord);

                if (!isSuccess)
                    return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");

                return ApiResponse<bool>.UpdatedSuccess("Education info updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateEducationInfoCommandHandler.");
                return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");
            }
        }
    }
}



//using AutoMapper;
//using axionpro.application.Common.Attributes;
//using axionpro.application.Common.Helpers;
//using axionpro.application.Common.Helpers.axionpro.application.Configuration;
//using axionpro.application.Common.Helpers.Converters;
//using axionpro.application.Common.Helpers.EncryptionHelper;
//using axionpro.application.DTOs.Employee;
//using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
//using axionpro.application.DTOs.Employee.AccessResponse;
//using axionpro.application.DTOS.Employee.Education;
//using axionpro.application.Interfaces;
//using axionpro.application.Interfaces.IEncryptionService;
//using axionpro.application.Interfaces.IFileStorage;
//using axionpro.application.Interfaces.IPermission;
//using axionpro.application.Interfaces.IRepositories;
//using axionpro.application.Interfaces.ITokenService;
//using axionpro.application.Wrappers;
//using axionpro.domain.Entity;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System.Reflection;

//namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
//{
//    public class UpdateEducationInfoCommand : IRequest<ApiResponse<bool>>
//    {
//        public GenericMultiFieldUpdateRequestDTO DTO { get; set; }

//        public UpdateEducationInfoCommand(GenericMultiFieldUpdateRequestDTO dto)
//        {
//            DTO = dto;
//        }
//    }

//    public class UpdateEducationInfoCommandHandler
//           : IRequestHandler<UpdateEducationInfoCommand, ApiResponse<bool>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly ILogger<UpdateEducationInfoCommandHandler> _logger;
//        private readonly IMapper _mapper;
//        private readonly IPermissionService _permissionService;
//        private readonly IConfiguration _config;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IFileStorageService _fileStorageService;

//        private readonly IIdEncoderService _idEncoderService;

//        public UpdateEducationInfoCommandHandler(
//            IUnitOfWork unitOfWork,
//            ILogger<UpdateEducationInfoCommandHandler> logger,
//            IMapper mapper,
//            IPermissionService permissionService,
//            IConfiguration configuration,
//            IHttpContextAccessor httpContextAccessor,
//            IIdEncoderService idEncoderService,
//            IFileStorageService fileStorageService)
//        {
//            _unitOfWork = unitOfWork;
//            _logger = logger;
//            _mapper = mapper;
//            _permissionService = permissionService;
//            _config = configuration;
//            _httpContextAccessor = httpContextAccessor;
//            _idEncoderService = idEncoderService;
//            _fileStorageService = fileStorageService;
//        }

//        public async Task<ApiResponse<bool>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var dto = request.DTO ?? throw new ArgumentNullException(nameof(request.DTO));

//                // --- 1) Token validation
//                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
//                    .ToString()?.Replace("Bearer ", "");
//                if (string.IsNullOrEmpty(bearerToken))
//                {
//                    _logger.LogWarning("Unauthorized: Missing token.");
//                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");
//                }

//                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
//                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
//                if (tokenClaims == null || tokenClaims.IsExpired)
//                {
//                    _logger.LogWarning("Invalid or expired token.");
//                    return ApiResponse<bool>.UpdatedFail("Invalid or expired token.");
//                }

//                // --- 2) Validate logged-in user
//                long loggedInEmpId = await _unitOfWork.CommonRepository
//                    .ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
//                if (loggedInEmpId < 1)
//                {
//                    _logger.LogWarning("Unauthorized or inactive user: {LoginId}", tokenClaims.UserId);
//                    return ApiResponse<bool>.UpdatedFail("Unauthorized or inactive user.");
//                }

//                // --- 3) Permission check
//                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
//                if (permissions == null || !permissions.Contains("UpdateEducationInfo"))
//                {
//                  //  _logger.LogWarning("Permission denied for RoleId: {RoleId}", tokenClaims.RoleId);
//                  //  return ApiResponse<bool>.Fail("Permission denied.");
//                }

//                // --- 4) Basic DTO validation
//                if ((dto.FieldsToUpdate == null || dto.FieldsToUpdate.Count == 0 ) && (dto.FilesToUpdate== null || dto.FilesToUpdate.Count == 0))
//                    return ApiResponse<bool>.UpdatedFail("No fields provided to update.");
//                // --- 5) Decrypt ids (defensive: try both possible id property names)
//                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
//                if (string.IsNullOrWhiteSpace(tenantKey))
//                    return ApiResponse<bool>.UpdatedFail("Tenant information missing in token.");

//                // sanitize keys/inputs
//                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
//                string rawUserEnc = EncryptionSanitizer.CleanEncodedInput(dto.UserEmployeeId ?? string.Empty);
//                string rawEmployeeEnc = EncryptionSanitizer.CleanEncodedInput(dto.EmployeeId ?? string.Empty);

//                if (string.IsNullOrEmpty(rawUserEnc) || string.IsNullOrEmpty(rawEmployeeEnc))
//                    return ApiResponse<bool>.UpdatedFail("Invalid request identifiers.");
//                long decryptedTenantId = _idEncoderService.DecodeId(EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId), finalKey);
//                long decryptedUserEmpId = _idEncoderService.DecodeId(rawUserEnc, finalKey);
//                long decryptedEmployeeId = _idEncoderService.DecodeId(rawEmployeeEnc, finalKey);

//                // record id: try RecordId first, fallback to Id property if present on DTO
//                string rawRecordEnc = null!;
//                var dtoType = dto.GetType();
//                var recordProp = dtoType.GetProperty("RecordId") ?? dtoType.GetProperty("Id") ?? null;
//                if (recordProp != null)
//                    rawRecordEnc = EncryptionSanitizer.CleanEncodedInput((recordProp.GetValue(dto) ?? string.Empty).ToString());
//                else
//                    return ApiResponse<bool>.Fail("Record identifier missing in request (RecordId/Id).");

//                long decryptedRecordId =  SafeParser.TryParseLong(rawRecordEnc);

//                if (decryptedUserEmpId <= 0 || decryptedEmployeeId <= 0 || decryptedRecordId <= 0)
//                {
//                    _logger.LogWarning("Decryption failed or invalid ids. User:{User} Emp:{Emp} Rec:{Rec}",
//                        decryptedUserEmpId, decryptedEmployeeId, decryptedRecordId);
//                    return ApiResponse<bool>.Fail("Invalid identifiers provided.");
//                }

//                // ensure the requester is the same user (or allow admin flow as per your policy)
//                if (decryptedUserEmpId != loggedInEmpId)
//                {
//                    _logger.LogWarning("Employee mismatch. Requester: {Req}, LoggedIn: {Logged}", decryptedUserEmpId, loggedInEmpId);
//                    return ApiResponse<bool>.Fail("Unauthorized: Employee mismatch.");
//                }


//                // --- 6) Fetch existing record
//                var existingRecord = await _unitOfWork.EmployeeEducationRepository
//                    .GetSingleRecordAsync(decryptedRecordId, true);



//                if (existingRecord == null)
//                {
//                    _logger.LogInformation("Education record not found. Id: {Id}", decryptedRecordId);
//                    return ApiResponse<bool>.UpdatedFail("Education record not found.");
//                }



//                // --- 11) Handle file uploads
//                if (dto.FilesToUpdate != null && dto.FilesToUpdate.Count > 0)
//                {
//                    //GPT  yeh wali condition theek karni hai kyuki possibility hai ki dto.FieldsToUpdate null na hoo or docname.FieldName=="Degree" bhi na hoo, aise mei  
//                    // docFileName= data.FieldValue mei file name hi nahi jaega????????

//                    string? docFileName = null;
//                    if (dto.FieldsToUpdate != null)
//                    {
//                        dto.FieldsToUpdate.Any();
//                      var data=   dto.FieldsToUpdate.FirstOrDefault(docname => docname.FieldName=="Degree");
//                        docFileName= data.FieldValue;
//                    }
//                    else
//                    {
//                        docFileName = EncryptionSanitizer.CleanEncodedInput(existingRecord.Degree.Trim().ToLower());

//                    }

//                    foreach (var fileItem in dto.FilesToUpdate)
//                    {
//                        if (fileItem.FieldValue == null || string.IsNullOrEmpty(fileItem.FieldName))
//                            continue;
//                        string? deletePath = existingRecord.FilePath;
//                        if (deletePath != null)
//                        { 
//                        //GPT folder mei jaakar yeh path ki file delete kar na hai
//                        }
//                        using var ms = new MemoryStream();
//                        await fileItem.FieldValue.CopyToAsync(ms);
//                        var fileBytes = ms.ToArray();
//                        string fileName = $"EDU-{existingRecord.EmployeeId + "_" + docFileName}-{DateTime.UtcNow:yyMMddHHmmss}.pdf";
//                        string fullFolderPath = _fileStorageService.GetEmployeeFolderPath(decryptedTenantId, decryptedEmployeeId, "education");

//                        // 🔹 Store actual name for reference in DB
//                        existingRecord.HasEducationDocUploded = true;

//                        // 🔹 Save file physically
//                      string  savedFullPath = await _fileStorageService.SaveFileAsync(fileBytes, fileName, fullFolderPath);

//                        // 🔹 If saved successfully, set relative path
//                        if (!string.IsNullOrEmpty(savedFullPath))
//                        {

//                            existingRecord.FilePath = _fileStorageService.GetRelativePath(savedFullPath);

//                            existingRecord.HasEducationDocUploded = true;
//                        }

//                    }
//                }

//                // --- 7) Build access DTO once from the existing entity
//                var accessDto = EmployeeeEducationInfoMapperHelper.ConvertToAccessResponseDTO(existingRecord);

//                // --- 8) Begin transaction
//                await _unitOfWork.BeginTransactionAsync();

//                // --- 9) Iterate fields and apply updates IN MEMORY
//                // 9️⃣ Loop on each field coming from list
//                // Pre-built readonly map
//                // --- 1) Build read-only map once (DTO level)
//                // --- 1) Build read-only map once
//                var readOnlyMap = typeof(EmployeeEducationEditableFieldsDTO)
//                    .GetProperties()
//                    .ToDictionary(p => p.Name, p => p.GetCustomAttribute<AccessControlAttribute>()?.ReadOnly ?? false);

//                // --- 2) Check fields to update
//                List<string> readOnlyFieldsAttempted = new List<string>();
//                foreach (var item in dto.FieldsToUpdate)
//                {
//                    string fieldName = item.FieldName.Trim();

//                    // Check if property exists in entity
//                    var entityProp = typeof(EmployeeEducation)
//                        .GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

//                    if (entityProp == null)
//                        continue; // ignore missing fields or you can log them

//                    // Check if read-only
//                    if (!readOnlyMap.TryGetValue(entityProp.Name, out var isReadOnly))
//                        isReadOnly = false;

//                    if (isReadOnly)
//                        readOnlyFieldsAttempted.Add(entityProp.Name);
//                }

//                // --- 3) Return comma-separated string of read-only fields
//                string readOnlyFieldsString = string.Join(", ", readOnlyFieldsAttempted);

//                // Agar koi read-only field attempt hua hai, use return kar do
//                if (readOnlyFieldsAttempted.Any())
//                {
//                    return ApiResponse<bool>.UpdatedFail($"These fields are read-only: {readOnlyFieldsString}");
//                }

//                // --- 4) Agar sab editable fields hain, yeh foreach continue karega entity update ke liye
//                foreach (var item in dto.FieldsToUpdate)
//                {
//                    string fieldName = item.FieldName.Trim();

//                    var entityProp = typeof(EmployeeEducation)
//                        .GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

//                    if (entityProp == null) continue;

//                    // Convert value
//                    if (!TryConvertObjectToValue.TryConvertValue(item.FieldValue, entityProp.PropertyType, out var convertedValue))
//                    {
//                        await _unitOfWork.RollbackTransactionAsync();
//                        return ApiResponse<bool>.UpdatedFail($"Invalid value for '{fieldName}'.");
//                    }

//                    // Set value
//                    entityProp.SetValue(existingRecord, convertedValue);
//                }


//                // --- 10) Update audit fields
//                existingRecord.UpdatedById = decryptedUserEmpId;
//                existingRecord.UpdatedDateTime = DateTime.UtcNow;

//                // --- 11) Persist changes (single DB call)
//                  bool isSuccess =  await  _unitOfWork.EmployeeEducationRepository.UpdateEmployeeFieldAsync(existingRecord); // assume this marks entity as modified
//                if (isSuccess)
//                {
//                    await _unitOfWork.CommitTransactionAsync();
//                    _logger.LogInformation("Education record {Id} updated by user {User}", decryptedRecordId, decryptedUserEmpId);
//                    return ApiResponse<bool>.UpdatedSuccess( "Education info updated successfully.");
//                }
//                else
//                {
//                     await _unitOfWork.RollbackTransactionAsync();
//                    _logger.LogError( "Unexpected error in UpdateEducationInfoCommandHandler.");
//                    return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");
//                }


//            }
//            catch (Exception ex)
//            {
//                try
//                {
//                    await _unitOfWork.RollbackTransactionAsync();
//                }
//                catch (Exception rEx)
//                {
//                    _logger.LogError(rEx, "Rollback failed after exception.");
//                }

//                _logger.LogError(ex, "Unexpected error in UpdateEducationInfoCommandHandler.");
//                return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");
//            }
//        }


//    }


//}
