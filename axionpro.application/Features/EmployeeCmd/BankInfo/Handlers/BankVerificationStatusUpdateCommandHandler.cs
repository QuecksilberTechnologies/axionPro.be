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

    public class UpdateVerificationStatusCommand
       : IRequest<ApiResponse<bool>>
    {
        public UpdateVerificationStatusRequestDTO DTO { get; set; }

        public UpdateVerificationStatusCommand(UpdateVerificationStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class BankVerificationStatusUpdateCommandHandler
        : IRequestHandler<UpdateVerificationStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BankVerificationStatusUpdateCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public BankVerificationStatusUpdateCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<BankVerificationStatusUpdateCommandHandler> logger,
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
    UpdateVerificationStatusCommand request,
    CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Updating verification status");

                // ===============================
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

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

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Assign values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ DECODE EMPLOYEE ID (IMPORTANT)
                // ===============================
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException(
                        "Invalid Employee Id.",
                        new List<string> { "EmployeeId is invalid after decoding." }
                    );

                // ===============================
                // 4️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "EmployeeBank",   // 🔹 Module (confirm if needed)
                //    "Update"          // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to update verification status.");

                // ===============================
                // 5️⃣ FETCH EMPLOYEE
                // ===============================
                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.TenantId,
                    true);

                if (employee == null)
                {
                    _logger.LogWarning(
                        "Employee not found. EmployeeId: {EmployeeId}",
                        request.DTO.Prop.EmployeeId);

                    throw new ApiException("Employee not found.", 404);
                }

                // ===============================
                // 6️⃣ UPDATE VERIFICATION STATUS
                // ===============================
                bool updateResult = await _unitOfWork.EmployeeBankRepository
                    .UpdateVerificationStatus(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.UserEmployeeId,
                        request.DTO.IsVerified);

                if (!updateResult)
                {
                    _logger.LogWarning(
                        "Failed to update verification status for EmployeeId: {EmployeeId}",
                        request.DTO.Prop.EmployeeId);

                    throw new ApiException("Unexpected error occurred.", 500);
                }

                // ===============================
                // 7️⃣ SUCCESS RESPONSE
                // ===============================
                _logger.LogInformation(
                    "Verification status updated successfully for EmployeeId: {EmployeeId}",
                    request.DTO.Prop.EmployeeId);

                return ApiResponse<bool>
                    .Success(true, "Verification update completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verification update error");

                // ❗ IMPORTANT: middleware handle karega
                throw;
            }
        }


    }
}

