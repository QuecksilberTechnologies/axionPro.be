using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Command;
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

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class UpdateDependentInfoCommandHandler : IRequestHandler<UpdateDependentCommand, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateDependentInfoCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEncryptionService _encryptionService;

        public UpdateDependentInfoCommandHandler(
            IBaseEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateDependentInfoCommandHandler> logger,
            IMapper mapper,
            ITokenService tokenService,
            IPermissionService permissionRepository,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
            _permissionService = permissionRepository;
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDependentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧱 Step 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

                var secretKey = _config["Jwt:Key"];
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<bool>.Fail("Invalid or expired token.");

                var tenantKey = _config["TenantKey"];
                long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (empId < 1)
                {
                    _logger.LogWarning("User validation failed for LoginId: {LoginId}", tokenClaims.UserId);
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("User is not authorized to perform this action.");
                }

                // 🧱 Step 2: Permission Check
                var permissions = await _permissionService.GetPermissionsAsync(tokenClaims.RoleId);
                if (!permissions.Contains("EditDependentInfo"))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("You do not have permission to edit dependent info.");
                }

                var dto = request.DTO; 

                if (string.IsNullOrWhiteSpace(dto.FieldName))
                    return ApiResponse<bool>.Fail("Field name cannot be empty.");

                if (!string.IsNullOrEmpty(request.DTO.EncriptedId))
                    request.DTO.EmployeeId = EncryptionHelper1.DecryptId(_encryptionService, request.DTO.EncriptedId, tenantKey);

                // 🧱 Step 3: Fetch Dependent record

                
                 var dependent = await _employeeRepository.UpdateEmployeeFieldAsync(request.DTO.EmployeeId, request.DTO.EntityName, request.DTO.FieldName, request.DTO.FieldValue, request.DTO.EmployeeId);
 
                // 🧱 Step 4: Map DTO → Entity
                var dependentEntity = _mapper.Map<EmployeeDependent>(dependent);

                // 🧱 Step 5: Convert to Access Control DTO
                var accessDto = EmployeeDependentInfoMapperHelper.ConvertToAccessResponseDTO(dependentEntity);

                // 🧱 Step 6: Locate Field in Access DTO
                var accessProp = typeof(GetDependenAccessResponseDTO)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

                if (accessProp == null)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' does not exist.");

                var fieldWithAccess = accessProp.GetValue(accessDto);
                var isReadOnlyProp = fieldWithAccess?.GetType().GetProperty("IsReadOnly");
                bool isReadOnly = (bool?)isReadOnlyProp?.GetValue(fieldWithAccess) ?? false;

                if (isReadOnly)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is read-only and cannot be modified.");

                // 🧱 Step 7: Locate actual entity property
                var entityProp = typeof(EmployeeDependent)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

                if (entityProp == null || !entityProp.CanWrite)
                    return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is invalid or not writable.");

                // 🧱 Step 8: Safe type conversion
                if (!TryConvertObjectToValue.TryConvertValue(dto.FieldValue, entityProp.PropertyType, out object? convertedValue))
                {
                    _logger.LogWarning("Conversion failed for field '{FieldName}' with value '{FieldValue}'", dto.FieldName, dto.FieldValue);
                    return ApiResponse<bool>.Fail($"Value conversion failed for field '{dto.FieldName}'.");
                }

                // 🧱 Step 9: Apply value & audit
                entityProp.SetValue(dependentEntity, convertedValue);
                dependentEntity.UpdatedById = dto.EmployeeId;
                dependentEntity.UpdatedDateTime = DateTime.UtcNow;

                // 🧱 Step 10: Save to DB
                var updateStatus = await _unitOfWork.Employees.UpdateEmployeeFieldAsync(dependentEntity.Id,dto.EntityName, dto.FieldName, convertedValue, dto.EmployeeId);

                if (!updateStatus)
                    return ApiResponse<bool>.Fail("Failed to update employee dependent record.");

                return ApiResponse<bool>.Success(true, $"Field '{dto.FieldName}' updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating dependent info.");
                return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
            }
        }
   
    
    }
}
