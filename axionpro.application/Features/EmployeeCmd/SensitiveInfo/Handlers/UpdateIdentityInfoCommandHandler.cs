using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOS.Employee.Sensitive;
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

namespace axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers
{
    public class UpdateIdentityInfoCommand : IRequest<ApiResponse<bool>>
    {
        public GenricUpdateRequestDTO DTO { get; set; }

        public UpdateIdentityInfoCommand(GenricUpdateRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class UpdateIdentityInfoCommandHandler : IRequestHandler<UpdateIdentityInfoCommand, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateIdentityInfoCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;

        public UpdateIdentityInfoCommandHandler(
            IBaseEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateIdentityInfoCommandHandler> logger,
            IMapper mapper,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IEncryptionService encryptionService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateIdentityInfoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Step 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

                var secretKey = _config["Jwt:Key"];
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);
                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");

                // ✅ Step 2: Validate Logged-in User
                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for LoginId: {LoginId}", tokenClaims.UserId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("User is not authorized to perform this action.");
                }

                // ✅ Step 3: Permission Check
                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("EditIdentityInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("You do not have permission to edit identity info.");
                }

                var tenantKey = tokenClaims.TenantEncriptionKey;
                var dto = request.DTO;

                // ✅ Step 4: Validation
                if (string.IsNullOrWhiteSpace(dto.FieldName))
                    return ApiResponse<bool>.Fail("Field name cannot be empty.");

                if (!string.IsNullOrEmpty(dto.EncriptedId))
                    dto.EmployeeId = EncryptionHelper1.DecryptId(_encryptionService, dto.EncriptedId, tenantKey);

                // ✅ Step 5: Fetch Employee Identity record
                var identityEntity = await _employeeRepository.UpdateEmployeeFieldAsync(
                    dto.EmployeeId, dto.EntityName, dto.FieldName, dto.FieldValue, dto.EmployeeId);

                if (identityEntity == null)
                    return ApiResponse<bool>.Fail("Employee identity record not found.");

                // ✅ Step 6: Map DTO → Entity
                var identity = _mapper.Map<EmployeePersonalDetail>(identityEntity);

                // ✅ Step 7: Convert to Access Control DTO
                var accessDto = EmployeePersonalInfoMapperHelper.ConvertToAccessResponseDTO(identity);

                // ✅ Step 8: Locate Field in Access DTO
                var accessProp = typeof(GetIdentityRequestDTO)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

                if (accessProp == null)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' does not exist.");

                var fieldWithAccess = accessProp.GetValue(accessDto);
                var isReadOnlyProp = fieldWithAccess?.GetType().GetProperty("IsReadOnly");
                bool isReadOnly = (bool?)isReadOnlyProp?.GetValue(fieldWithAccess) ?? false;

                if (isReadOnly)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is read-only and cannot be modified.");

                // ✅ Step 9: Locate actual entity property
                var entityProp = typeof(EmployeePersonalDetail)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

                if (entityProp == null || !entityProp.CanWrite)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is invalid or not writable.");

                // ✅ Step 10: Safe type conversion
                if (!TryConvertObjectToValue.TryConvertValue(dto.FieldValue, entityProp.PropertyType, out object? convertedValue))
                {
                    _logger.LogWarning("Conversion failed for field '{FieldName}' with value '{FieldValue}'", dto.FieldName, dto.FieldValue);
                    return ApiResponse<bool>.Fail($"Value conversion failed for field '{dto.FieldName}'.");
                }

                // ✅ Step 11: Apply value & audit
                entityProp.SetValue(identity, convertedValue);
                identity.UpdatedById = dto.EmployeeId;
                identity.UpdatedDateTime = DateTime.UtcNow;

                // ✅ Step 12: Save to DB
                var updateStatus = await _unitOfWork.Employees.UpdateEmployeeFieldAsync(
                    identity.Id, dto.EntityName, dto.FieldName, convertedValue, dto.EmployeeId);

                if (!updateStatus)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to update employee identity record.");
                }

                return ApiResponse<bool>.Success(true, $"Field '{dto.FieldName}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating identity info.");
                return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }
    }
}
