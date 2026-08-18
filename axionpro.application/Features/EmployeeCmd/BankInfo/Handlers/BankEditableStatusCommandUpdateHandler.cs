// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles BankEditableStatusCommandUpdateHandler requests using authenticated tenant context.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{

    #region Command

    /// <summary>
    /// Represents the UpdateEditableStatusCommand application component.
    /// </summary>
    public class UpdateEditableStatusCommand
      : IRequest<ApiResponse<bool>>
    {
        public UpdateEditStatusRequestDTO DTO { get; set; }

        public UpdateEditableStatusCommand(UpdateEditStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handles authenticated tenant requests for this feature.
    /// </summary>
        #endregion

    #region Handler

public class BankEditableStatusCommandUpdateHandler
        : IRequestHandler<UpdateEditableStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BankEditableStatusCommandUpdateHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public BankEditableStatusCommandUpdateHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BankEditableStatusCommandUpdateHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config, ICommonRequestService commonRequestService,

            IEncryptionService encryptionService, IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }
        public async Task<ApiResponse<bool>> Handle(
    UpdateEditableStatusCommand request,
    CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating editable status");

                // ===============================
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                #region Tenant Request Validation
                var validation = await _commonRequestService
                    .ValidateRequestAsync();
                #endregion

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                // ===============================
                // 3️⃣ DECODE EMPLOYEE ID (IMPORTANT)
                // ===============================
                var employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException(
                        "Invalid Employee Id.",
                        new List<string> { "EmployeeId is invalid after decoding." }
                    );

                // ===============================
                // 4️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "EmployeeBank",   //  Module (adjust if needed)
                //    "Update"          //  Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to update editable status.");

                // ===============================
                // 5️⃣ FETCH EMPLOYEE
                // ===============================
                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    employeeId,
                    validation.TenantId,
                    true);

                if (employee == null)
                {
                    _logger.LogWarning(
                        "Employee not found. EmployeeId: {EmployeeId}",
                        employeeId);

                    throw new ApiException("Employee not found.", 404);
                }

                // ===============================
                // 6️⃣ UPDATE EDITABLE STATUS
                // ===============================
                bool updateResult = await _unitOfWork.EmployeeBankRepository
                    .UpdateEditStatus(
                        employeeId,
                        validation.LoggedInEmployeeId,
                        request.DTO.IsEditable);

                if (!updateResult)
                {
                    _logger.LogWarning(
                        "Failed to update editable status for EmployeeId: {EmployeeId}",
                        employeeId);

                    throw new ApiException("Unexpected error occurred.", 500);
                }

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                _logger.LogInformation(
                    "Editable status updated successfully for EmployeeId: {EmployeeId}",
                    employeeId);

                return ApiResponse<bool>
                    .Success(true, "Editable update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Editable update error");

                //  IMPORTANT: middleware handle karega
                throw;
            }
        }


    }
    #endregion
}
