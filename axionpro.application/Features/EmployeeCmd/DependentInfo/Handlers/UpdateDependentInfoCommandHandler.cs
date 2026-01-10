using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Pagination;
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

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class UpdateDependentCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateDependentRequestDTO DTO { get; set; }

        public UpdateDependentCommand(UpdateDependentRequestDTO dto)
        {
            DTO = dto;
        }

    }

    //public class UpdateDependentInfoCommandHandler : IRequestHandler<UpdateDependentCommand, ApiResponse<bool>>
    //{
    //    private readonly IBaseEmployeeRepository _employeeRepository;
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ILogger<UpdateDependentInfoCommandHandler> _logger;
    //    private readonly IMapper _mapper;
    //    private readonly ITokenService _tokenService;
    //    private readonly IPermissionService _permissionService;
    //    private readonly IConfiguration _config;
    //    private readonly IHttpContextAccessor _httpContextAccessor;
    //    private readonly IEncryptionService _encryptionService;
    //    private readonly ICommonRequestService _commonRequestService;
    //    private readonly IIdEncoderService _idEncoderService;


    //    public UpdateDependentInfoCommandHandler(
    //        IBaseEmployeeRepository employeeRepository,
    //        IUnitOfWork unitOfWork,
    //        ILogger<UpdateDependentInfoCommandHandler> logger,
    //        IMapper mapper,
    //        ITokenService tokenService,
    //        IPermissionService permissionRepository,
    //        IConfiguration configuration,
    //        IHttpContextAccessor httpContextAccessor, IEncryptionService encryptionService, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService)
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
    //        _commonRequestService = commonRequestService;
    //        _idEncoderService = idEncoderService;
    //    }

    //    public async Task<ApiResponse<bool>> Handle(UpdateDependentCommand request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            await _unitOfWork.BeginTransactionAsync();

    //            // 🔐 STEP 1: COMMON VALIDATION (SAME AS CONTACT)
    //            var validation =
    //                await _commonRequestService.ValidateRequestAsync(
    //                    request.DTO.UserEmployeeId);

    //            if (!validation.Success)
    //                return   ApiResponse<bool>
    //                    .Fail(validation.ErrorMessage);

    //            // 🔓 STEP 2: Assign decoded values
    //            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
    //            request.DTO.Prop.TenantId = validation.TenantId;

    //            request.DTO.Prop.EmployeeId =
    //                RequestCommonHelper.DecodeOnlyEmployeeId(
    //                    request.DTO.EmployeeId,
    //                    validation.Claims.TenantEncriptionKey,
    //                    _idEncoderService
    //                );

    //            // 🔑 STEP 3: Permission check
    //            var permissions =
    //                await _permissionService.GetPermissionsAsync(validation.RoleId);

    //            if (!permissions.Contains("AddDependentInfo"))
    //            {
    //                // optional hard-stop
    //                // return ApiResponse<List<GetDependentResponseDTO>>
    //                //     .Fail("You do not have permission to add dependent info.");
    //            }
    //            var dependent =
    //               await _unitOfWork.EmployeeDependentRepository
    //                   .GetSingleRecordAsync(request.DTO.Id, request.DTO.Prop.IsActive);
    //            if (dependent == null)
    //                return ApiResponse<bool>.Fail(validation.ErrorMessage);

    //           // HasProofUploaded

    //            var dependentEntity = _mapper.Map<EmployeeDependent>(dependent);

                
                
                
                
                
                
                
    //            // 🧱 Step 5: Convert to Access Control DTO
    //            var accessDto = EmployeeDependentInfoMapperHelper.ConvertToAccessResponseDTO(dependentEntity);

    //            // 🧱 Step 6: Locate Field in Access DTO
    //            var accessProp = typeof(GetDependenAccessResponseDTO)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (accessProp == null)
    //                return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' does not exist.");

    //            var fieldWithAccess = accessProp.GetValue(accessDto);
    //            var isReadOnlyProp = fieldWithAccess?.GetType().GetProperty("IsReadOnly");
    //            bool isReadOnly = (bool?)isReadOnlyProp?.GetValue(fieldWithAccess) ?? false;

    //            if (isReadOnly)
    //                return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is read-only and cannot be modified.");

    //            // 🧱 Step 7: Locate actual entity property
    //            var entityProp = typeof(EmployeeDependent)
    //                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
    //                .FirstOrDefault(p => string.Equals(p.Name, dto.FieldName, StringComparison.OrdinalIgnoreCase));

    //            if (entityProp == null || !entityProp.CanWrite)
    //                return ApiResponse<bool>.Fail($"Field '{dto.FieldName}' is invalid or not writable.");

    //            // 🧱 Step 8: Safe type conversion
    //            if (!TryConvertObjectToValue.TryConvertValue(dto.FieldValue, entityProp.PropertyType, out object? convertedValue))
    //            {
    //                _logger.LogWarning("Conversion failed for field '{FieldName}' with value '{FieldValue}'", dto.FieldName, dto.FieldValue);
    //                return ApiResponse<bool>.Fail($"Value conversion failed for field '{dto.FieldName}'.");
    //            }

    //            // 🧱 Step 9: Apply value & audit
    //            entityProp.SetValue(dependentEntity, convertedValue);
    //            dependentEntity.UpdatedById = dto._EmployeeId;
    //            dependentEntity.UpdatedDateTime = DateTime.UtcNow;

    //            // 🧱 Step 10: Save to DB
    //            var updateStatus = await _unitOfWork.Employees.UpdateEmployeeFieldAsync(dependentEntity.Id, dto.EntityName, dto.FieldName, convertedValue, dto._EmployeeId);

    //            if (!updateStatus)
    //                return ApiResponse<bool>.Fail("Failed to update employee dependent record.");

    //            return ApiResponse<bool>.Success(true, $"Field '{dto.FieldName}' updated successfully.");
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Unexpected error occurred while updating dependent info.");
    //            return ApiResponse<bool>.Fail("An unexpected error occurred.", new List<string> { ex.Message });
    //        }
    //    }


    //}

}
