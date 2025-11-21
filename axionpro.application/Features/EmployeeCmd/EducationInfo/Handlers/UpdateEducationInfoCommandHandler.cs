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
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        private readonly IIdEncoderService _idEncoderService;

        public UpdateEducationInfoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateEducationInfoCommandHandler> logger,
            IMapper mapper,
            IPermissionService permissionService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _permissionService = permissionService;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateEducationInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var dto = request.DTO ?? throw new ArgumentNullException(nameof(request.DTO));

                // --- 1) Token validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                {
                    _logger.LogWarning("Unauthorized: Missing token.");
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");
                }

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                {
                    _logger.LogWarning("Invalid or expired token.");
                    return ApiResponse<bool>.UpdatedFail("Invalid or expired token.");
                }

                // --- 2) Validate logged-in user
                long loggedInEmpId = await _unitOfWork.CommonRepository
                    .ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("Unauthorized or inactive user: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<bool>.UpdatedFail("Unauthorized or inactive user.");
                }

                // --- 3) Permission check
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (permissions == null || !permissions.Contains("UpdateEducationInfo"))
                {
                  //  _logger.LogWarning("Permission denied for RoleId: {RoleId}", tokenClaims.RoleId);
                  //  return ApiResponse<bool>.Fail("Permission denied.");
                }

                // --- 4) Basic DTO validation
                if (dto.FieldsToUpdate == null || dto.FieldsToUpdate.Count == 0)
                    return ApiResponse<bool>.UpdatedFail("No fields provided to update.");

                // --- 5) Decrypt ids (defensive: try both possible id property names)
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrWhiteSpace(tenantKey))
                    return ApiResponse<bool>.UpdatedFail("Tenant information missing in token.");

                // sanitize keys/inputs
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string rawUserEnc = EncryptionSanitizer.CleanEncodedInput(dto.UserEmployeeId ?? string.Empty);
                string rawEmployeeEnc = EncryptionSanitizer.CleanEncodedInput(dto.EmployeeId ?? string.Empty);

                if (string.IsNullOrEmpty(rawUserEnc) || string.IsNullOrEmpty(rawEmployeeEnc))
                    return ApiResponse<bool>.UpdatedFail("Invalid request identifiers.");

                long decryptedUserEmpId = _idEncoderService.DecodeId(rawUserEnc, finalKey);
                long decryptedEmployeeId = _idEncoderService.DecodeId(rawEmployeeEnc, finalKey);

                // record id: try RecordId first, fallback to Id property if present on DTO
                string rawRecordEnc = null!;
                var dtoType = dto.GetType();
                var recordProp = dtoType.GetProperty("RecordId") ?? dtoType.GetProperty("Id") ?? null;
                if (recordProp != null)
                    rawRecordEnc = EncryptionSanitizer.CleanEncodedInput((recordProp.GetValue(dto) ?? string.Empty).ToString());
                else
                    return ApiResponse<bool>.Fail("Record identifier missing in request (RecordId/Id).");

                long decryptedRecordId =  SafeParser.TryParseLong(rawRecordEnc);

                if (decryptedUserEmpId <= 0 || decryptedEmployeeId <= 0 || decryptedRecordId <= 0)
                {
                    _logger.LogWarning("Decryption failed or invalid ids. User:{User} Emp:{Emp} Rec:{Rec}",
                        decryptedUserEmpId, decryptedEmployeeId, decryptedRecordId);
                    return ApiResponse<bool>.Fail("Invalid identifiers provided.");
                }

                // ensure the requester is the same user (or allow admin flow as per your policy)
                if (decryptedUserEmpId != loggedInEmpId)
                {
                    _logger.LogWarning("Employee mismatch. Requester: {Req}, LoggedIn: {Logged}", decryptedUserEmpId, loggedInEmpId);
                    return ApiResponse<bool>.Fail("Unauthorized: Employee mismatch.");
                }

                // --- 6) Fetch existing record
                var existingRecord = await _unitOfWork.EmployeeEducationRepository
                    .GetSingleRecordAsync(decryptedRecordId, true);



                if (existingRecord == null)
                {
                    _logger.LogInformation("Education record not found. Id: {Id}", decryptedRecordId);
                    return ApiResponse<bool>.UpdatedFail("Education record not found.");
                }

                // --- 7) Build access DTO once from the existing entity
                var accessDto = EmployeeeEducationInfoMapperHelper.ConvertToAccessResponseDTO(existingRecord);

                // --- 8) Begin transaction
                await _unitOfWork.BeginTransactionAsync();

                // --- 9) Iterate fields and apply updates IN MEMORY
                // 9️⃣ Loop on each field coming from list
                // Pre-built readonly map
                var readOnlyMap = typeof(EmployeeEducationEditableFieldsDTO)
                    .GetProperties()
                    .ToDictionary(p => p.Name, p => p.GetCustomAttribute<AccessControlAttribute>()?.ReadOnly ?? false);

                foreach (var item in dto.FieldsToUpdate)
                {
                    string fieldName = item.FieldName.Trim();

                    // ❗ Check if field exists in Access DTO
                    var accessProp = typeof(GetEducationAccessResponseDTO)
                        .GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (accessProp == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.UpdatedFail($"Field '{fieldName}' not found.");
                    }

                    // ❗ Check if field is read-only
                    var fieldAccessObj = accessProp.GetValue(accessDto);
                    bool isReadOnly = (bool?)fieldAccessObj?
                        .GetType().GetProperty("IsReadOnly")?.GetValue(fieldAccessObj) ?? false;

                    if (isReadOnly)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.UpdatedFail($"Field '{fieldName}' is read-only.");
                    }


                    // 🟢 GET ENTITY PROPERTY (ACTUAL DB COLUMN)
                    var entityProp = typeof(EmployeeEducation)
                        .GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (entityProp == null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.UpdatedFail($"Property '{fieldName}' not found in entity.");
                    }

                    // 🟢 Convert incoming value (string → int/datetime/bool/etc)
                    if (!TryConvertObjectToValue.TryConvertValue(item.FieldValue, entityProp.PropertyType, out var convertedValue))
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return ApiResponse<bool>.UpdatedFail($"Invalid value for '{fieldName}'.");
                    }

                    // 🟢 APPLY VALUE INTO ENTITY (THIS IS MAIN UPDATE OPERATION)
                    entityProp.SetValue(existingRecord, convertedValue);
                }

                // --- 10) Update audit fields
                existingRecord.UpdatedById = decryptedUserEmpId;
                existingRecord.UpdatedDateTime = DateTime.UtcNow;

                // --- 11) Persist changes (single DB call)
                  bool isSuccess =  await  _unitOfWork.EmployeeEducationRepository.UpdateEmployeeFieldAsync(existingRecord); // assume this marks entity as modified
                if (isSuccess)
                {
                    await _unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("Education record {Id} updated by user {User}", decryptedRecordId, decryptedUserEmpId);
                    return ApiResponse<bool>.UpdatedSuccess( "Education info updated successfully.");
                }
                else
                {
                     await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError( "Unexpected error in UpdateEducationInfoCommandHandler.");
                    return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");
                }

                   
            }
            catch (Exception ex)
            {
                try
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                catch (Exception rEx)
                {
                    _logger.LogError(rEx, "Rollback failed after exception.");
                }

                _logger.LogError(ex, "Unexpected error in UpdateEducationInfoCommandHandler.");
                return ApiResponse<bool>.UpdatedFail("Unexpected error occurred.");
            }
        }
   
    
    }


}
