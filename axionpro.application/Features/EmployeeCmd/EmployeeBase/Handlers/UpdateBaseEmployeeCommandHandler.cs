using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{


    public class UpdateEmployeeCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeRequestDTO DTO { get; set; }

        public UpdateEmployeeCommand(UpdateEmployeeRequestDTO dto)
        {
            DTO = dto;
        }

    }
    public class UpdateBaseEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBaseEmployeeCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateBaseEmployeeCommandHandler(
            IBaseEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateBaseEmployeeCommandHandler> logger,
            IMapper mapper,
            IConfiguration config,
            IHttpContextAccessor httpContextAccessor,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService, ICommonRequestService commonRequestService, IPermissionService permissionService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService; 
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            
        }

        public async Task<ApiResponse<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {

                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                 request.DTO.EmployeeId,
                 validation.Claims.TenantEncriptionKey,
                _idEncoderService
                );


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                   // ---------- FETCH Existing Employee ----------
                var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(request.DTO.Prop.EmployeeId ,request.DTO.Prop.TenantId, true);


                if (existingEmployee == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Employee not found.");
                }

                // ---------- APPLY UPDATE (ONLY IF VALUE PROVIDED) ----------
                if (!string.IsNullOrWhiteSpace(request.DTO.FirstName))
                    existingEmployee.FirstName = request.DTO.FirstName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.MiddleName))
                    existingEmployee.MiddleName = request.DTO.MiddleName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.LastName))
                    existingEmployee.LastName = request.DTO.LastName.Trim();

                if (!string.IsNullOrWhiteSpace(request.DTO.Description))
                    existingEmployee.Description = request.DTO.Description?.Trim();

                if (request.DTO.DateOfBirth.HasValue)
                    existingEmployee.DateOfBirth = request.DTO.DateOfBirth.Value;

                if (request.DTO.GenderId is > 0)
                {
                    existingEmployee.GenderId = request.DTO.GenderId.Value;
                }




                // ---------- AUDIT ----------
                existingEmployee.UpdatedById = request.DTO.Prop.UserEmployeeId;
                existingEmployee.UpdatedDateTime = DateTime.UtcNow;
           

                // ---------- SAVE ----------
                await _unitOfWork.Employees.UpdateEmployeeAsync(existingEmployee, request.DTO.Prop.TenantId);
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<bool>.Success(true, "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating employee");
                return ApiResponse<bool>.Fail("Unexpected error.", new List<string> { ex.Message });
            }
        
        }
    }















    //public class UpdateEmployeeCommand : IRequest<ApiResponse<bool>>
    //{
    //    public GenricUpdateRequestDTO updateBaseEmployeeRequestDTO { get; set; }

    //    public UpdateEmployeeCommand(GenricUpdateRequestDTO dto)
    //    {
    //        updateBaseEmployeeRequestDTO = dto;
    //    }

    //}
    //public class UpdateBaseEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ApiResponse<bool>>
    //{
    //    private readonly IBaseEmployeeRepository _employeeRepository;
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ILogger<UpdateBaseEmployeeCommandHandler> _logger;
    //    private readonly IMapper _mapper;
    //    private readonly ITokenService _tokenService;
    //    private readonly IPermissionService _permissionService;
    //    private readonly IConfiguration _config;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly IEncryptionService _encryptionService;


    //    public UpdateBaseEmployeeCommandHandler(
    //        IBaseEmployeeRepository employeeRepository,
    //        IUnitOfWork unitOfWork,
    //        ILogger<UpdateBaseEmployeeCommandHandler> logger,
    //        IMapper mapper,
    //        ITokenService tokenService,
    //        IPermissionService permissionRepository,
    //        IConfiguration configuration,
    //        IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService)
    //    {
    //        _employeeRepository = employeeRepository;
    //        _unitOfWork = unitOfWork;
    //        _logger = logger;
    //        _mapper = mapper;
    //        _tokenService = tokenService;
    //        _permissionService = permissionRepository;
    //        _config = configuration;
    //        _httpContextAccessor = httpContextAccessor;
    //        _encryptionService = encryptionService;
    //    }

    //    public async Task<ApiResponse<bool>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            // 🧱 Step 1: Validate JWT Token
    //            var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString()?.Replace("Bearer ", "");
    //            if (string.IsNullOrEmpty(bearerToken))
    //                return ApiResponse<bool>.Fail("Unauthorized: Token not found.");

    //            var secretKey = _config["Jwt:Key"];
    //            var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

    //            if (tokenClaims == null || tokenClaims.IsExpired)
    //                return ApiResponse<bool>.Fail("Invalid or expired token.");

    //            long empId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
    //            if (empId < 1)
    //            {
    //                _logger.LogWarning("User validation failed for LoginId: {LoginId}", tokenClaims.UserId);
    //                await _unitOfWork.RollbackTransactionAsync();
    //                return ApiResponse<bool>.Fail("User is not authorized to perform this action.");
    //            }

    //            // 🧱 Step 2: Permission Check
    //            var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
    //            if (!permissions.Contains("EditEmployeeInfo"))
    //            {
    //                await _unitOfWork.RollbackTransactionAsync();
    //                return ApiResponse<bool>.Fail("You do not have permission to update employee information.");
    //            }

    //            var DTO = request.updateBaseEmployeeRequestDTO;
    //            var tenantKey = tokenClaims.TenantEncriptionKey;


    //            // 🧱 Step 4: Validate DTO Input
    //            if (DTO == null)
    //                return ApiResponse<bool>.Fail("Invalid request: DTO cannot be null.");

    //            if (!string.IsNullOrEmpty(DTO.EmployeeId))
    //                DTO._EmployeeId = EncryptionHelper1.DecryptId(_encryptionService, DTO.EmployeeId, tenantKey);



    //            if (string.IsNullOrWhiteSpace(DTO.FieldName))
    //                return ApiResponse<bool>.Fail("Field name cannot be empty.");

    //            if (!string.IsNullOrEmpty(DTO.UserEmployeeId))
    //                DTO._EmployeeId = EncryptionHelper1.DecryptId(_encryptionService, DTO.UserEmployeeId, tenantKey);


    //            if (string.IsNullOrWhiteSpace(DTO.FieldName))
    //                return ApiResponse<bool>.Fail("Field name is required.");

    //            // 🧱 Step 3: Fetch Employee Record
    //            var employee = await _employeeRepository.GetSingleRecordAsync(DTO._EmployeeId, true);
    //            if (employee == null)
    //                return ApiResponse<bool>.Fail("Employee not found.");

    //            // 🧱 Step 4: Generate FieldWithAccess DTO (for read-only check)
    //            var editableDto = _mapper.Map<EmployeeInfoEditableFieldsDTO>(employee);
    //            var accessDto = EmployeeBasicInfoMapperHelper.ConvertToAccessResponseDTO(editableDto);

    //            // 🧱 Step 5: Reflection to find requested field
    //            var accessProp = typeof(GetBaseAccessEmployeeResponseDTO)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, DTO.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (accessProp == null)
    //                return ApiResponse<bool>.Fail($"Field '{DTO.FieldName}' does not exist.");

    //            var fieldWithAccess = accessProp.GetValue(accessDto);
    //            var isReadOnlyProp = fieldWithAccess?.GetType().GetProperty("IsReadOnly");
    //            bool isReadOnly = (bool?)isReadOnlyProp?.GetValue(fieldWithAccess) ?? false;

    //            if (isReadOnly)
    //                return ApiResponse<bool>.Fail($"Field '{DTO.FieldName}' is read-only and cannot be updated.");

    //            // 🧱 Step 6: Find actual property in Employee entity
    //            var employeeProp = typeof(Employee)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, DTO.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (employeeProp == null || !employeeProp.CanWrite)
    //                return ApiResponse<bool>.Fail($"Property '{DTO.FieldName}' is not valid or not writable.");

    //            // 🧱 Step 7: Safe type conversion
    //            if (!TryConvertObjectToValue.TryConvertValue(DTO.FieldValue, employeeProp.PropertyType, out object? convertedValue))
    //            {
    //                _logger.LogWarning("Conversion failed for property '{Field}' with value '{Value}'", DTO.FieldName, DTO.FieldValue);
    //                return ApiResponse<bool>.Fail($"Value conversion failed for property '{DTO.FieldName}'.");
    //            }

    //            // 🧱 Step 8: Apply update
    //            employeeProp.SetValue(employee, convertedValue);
    //          //  employee.UpdatedById = DTO.EmployeeId;
    //          //  employee.UpdatedDateTime = DateTime.UtcNow;

    //            // 🧱 Step 9: Commit update
    //            var updateStatus = await _unitOfWork.Employees.UpdateEmployeeFieldAsync(
    //                employee.Id, DTO.EntityName, DTO.FieldName, convertedValue, DTO._EmployeeId);

    //            if (!updateStatus)
    //                return ApiResponse<bool>.Fail("Failed to update employee record.");

    //            return ApiResponse<bool>.Success(true, $"Field '{DTO.FieldName}' updated successfully.");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Unexpected error while updating employee field.");
    //            return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
    //        }
    //    }
    //}

}
