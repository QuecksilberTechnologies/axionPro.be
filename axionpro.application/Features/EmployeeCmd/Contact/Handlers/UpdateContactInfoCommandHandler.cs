using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.Contact.Command;
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

namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{
    //public class UpdateContactInfoCommandHandler : IRequestHandler<UpdateContactInfoCommand, ApiResponse<bool>>
    //{ 
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ILogger<UpdateContactInfoCommandHandler> _logger;
    //    private readonly IMapper _mapper;
    //    private readonly ITokenService _tokenService;
    //    private readonly IPermissionService _permissionService;
    //    private readonly IConfiguration _config;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly IEncryptionService _encryptionService;


    //    public UpdateContactInfoCommandHandler(
    //        IBaseEmployeeRepository employeeRepository,
    //        IUnitOfWork unitOfWork,
    //        ILogger<UpdateContactInfoCommandHandler> logger,
    //        IMapper mapper,
    //        ITokenService tokenService,
    //        IPermissionService permissionRepository,
    //        IConfiguration configuration,
    //        IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService)
    //    {

    //        _unitOfWork = unitOfWork;
    //        _logger = logger;
    //        _mapper = mapper;
    //        _tokenService = tokenService;
    //        _permissionService = permissionRepository;
    //        _config = configuration;
    //        _httpContextAccessor = httpContextAccessor;
    //        _encryptionService = encryptionService;
    //    }

    //    public async Task<ApiResponse<bool>> Handle(UpdateContactInfoCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            // 🧱 Step 1: Validate JWT Token
    //            var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
    //                .ToString()?.Replace("Bearer ", "");

    //            if (string.IsNullOrEmpty(bearerToken))
    //            {
    //                _logger.LogWarning("Unauthorized access: Missing Bearer token.");
    //                return ApiResponse<bool>.Fail("Unauthorized: Token not found.");
    //            }

    //            var secretKey = _config["Jwt:Key"];
    //            var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

    //            if (tokenClaims == null || tokenClaims.IsExpired)
    //            {
    //                _logger.LogWarning("Invalid or expired JWT token.");
    //                return ApiResponse<bool>.Fail("Invalid or expired token.");
    //            }

    //            // 🧱 Step 2: Validate Logged-in User
    //            long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
    //            if (empId < 1)
    //            {
    //                _logger.LogWarning("User validation failed for LoginId: {LoginId}", tokenClaims.UserId);
    //                await _unitOfWork.RollbackTransactionAsync();
    //                return ApiResponse<bool>.Fail("User is not authorized to perform this action.");
    //            }

    //            // 🧱 Step 3: Permission Check
    //            var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
    //            if (permissions == null || !permissions.Contains("AddContactInfo"))
    //            {
    //                _logger.LogWarning("Permission denied for RoleId: {RoleId}", tokenClaims.RoleId);
    //                await _unitOfWork.RollbackTransactionAsync();
    //                return ApiResponse<bool>.Fail("You do not have permission to update Contact info.");
    //            }
    //            var tenantKey = tokenClaims.TenantEncriptionKey;

    //            var dto = request.DTO;

    //            // 🧱 Step 4: Validate DTO Input
    //            if (dto == null)
    //                return ApiResponse<bool>.Fail("Invalid request: DTO cannot be null.");

    //            if (!string.IsNullOrEmpty(request.DTO.EmployeeId))
    //                request.DTO._EmployeeId = EncryptionHelper1.DecryptId(_encryptionService, request.DTO.EmployeeId, tenantKey);

    //            if (!string.IsNullOrEmpty(request.DTO.UserEmployeeId))
    //                request.DTO.Id = (int)EncryptionHelper1.DecryptId(_encryptionService, request.DTO.UserEmployeeId, tenantKey);



    //            if (string.IsNullOrWhiteSpace(dto.FieldName))
    //                return ApiResponse<bool>.Fail("Field name is required.");

    //            // 🧱 Step 5: Fetch Existing Contact Record


    //            var existingRecord = await _unitOfWork.EmployeeContactRepository.GetSingleRecordAsync(request.DTO.Id, true);

    //            if (existingRecord == null)
    //            {
    //                _logger.LogInformation("No Contact record found for Id: {Id}", dto.UserEmployeeId);
    //                return ApiResponse<bool>.Fail("Employee Contact info not found.");
    //            }

    //            // 🧱 Step 6: Map DTO → Entity
    //            var ContactEntity = _mapper.Map<EmployeeContact>(existingRecord);

    //            // 🧱 Step 7: Access Control Check
    //            var accessDto = EmployeeContactInfoMapperHelper.ConvertToAccessResponseDTO(ContactEntity);

    //            var accessProp = typeof(GetContactAccessResponseDTO)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (accessProp == null)
    //                return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' does not exist.");

    //            var fieldWithAccess = accessProp.GetValue(accessDto);
    //            var isReadOnlyProp = fieldWithAccess?.GetType().GetProperty("IsReadOnly");
    //            bool isReadOnly = (bool?)isReadOnlyProp?.GetValue(fieldWithAccess) ?? false;

    //            if (isReadOnly)
    //            {
    //                _logger.LogWarning("Attempt to modify read-only field: {Field}", dto.FieldName);
    //                return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is read-only and cannot be updated.");
    //            }

    //            // 🧱 Step 8: Reflect actual property and convert value
    //            var entityProp = typeof(EmployeeContact)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (entityProp == null || !entityProp.CanWrite)
    //                return ApiResponse<bool>.Fail($"Property '{dto.FieldName}' is not valid or not writable.");

    //            if (!TryConvertObjectToValue.TryConvertValue(dto.FieldValue, entityProp.PropertyType, out object? convertedValue))
    //            {
    //                _logger.LogWarning("Value conversion failed for {Field} with input '{Value}'", dto.FieldName, dto.FieldValue);
    //                return ApiResponse<bool>.Fail($"Value conversion failed for property '{dto.FieldName}'.");
    //            }

    //            // 🧱 Step 9: Apply Update
    //            entityProp.SetValue(ContactEntity, convertedValue);
    //            ContactEntity.UpdatedById = dto._EmployeeId;
    //            ContactEntity.UpdatedDateTime = DateTime.UtcNow;

    //            var updateStatus = await _unitOfWork.Employees.UpdateEmployeeFieldAsync(
    //                existingRecord.Id, dto.EntityName, dto.FieldName, convertedValue, dto._EmployeeId);

    //            if (!updateStatus)
    //            {
    //                _logger.LogError("Failed to update EmployeeContactDetail for Id: {Id}", dto.UserEmployeeId);
    //                return ApiResponse<bool>.Fail("Failed to update employee Contact record.");
    //            }

    //            _logger.LogInformation("Field '{Field}' updated successfully for ContactId: {Id}", dto.FieldName, dto.UserEmployeeId);
    //            return ApiResponse<bool>.Success(true, $"Field '{dto.FieldName}' updated successfully.");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Unexpected error while updating employee Contact field.");
    //            return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
    //        }
    //    }
    //}


}
