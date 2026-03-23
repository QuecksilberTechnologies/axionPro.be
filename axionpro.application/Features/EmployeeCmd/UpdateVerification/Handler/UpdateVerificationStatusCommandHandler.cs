using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.UpdateVerification.Handler
{
    // ============================
    // COMMAND
    // ============================

    public class UpdateVerificationStatusCommand
        : IRequest<ApiResponse<bool>>
    {
        public UpdateVerificationStatusRequestDTO_ DTO { get; }

        public UpdateVerificationStatusCommand(UpdateVerificationStatusRequestDTO_ dto)
        {
            DTO = dto;
        }
    }

    // ============================
    // HANDLER
    // ============================
    public class UpdateVerificationStatusCommandHandler
        : IRequestHandler<UpdateVerificationStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateVerificationStatusCommandHandler> _logger;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateVerificationStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateVerificationStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(
     UpdateVerificationStatusCommand request,
     CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("UpdateVerificationStatus started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid employee.");

                // ===============================
                // 3️⃣ ENUM VALIDATION
                // ===============================
                if (!Enum.IsDefined(typeof(TabInfoType), request.DTO.TabInfoType))
                    throw new ValidationErrorException("Invalid section type.");

                // ===============================
                // 4️⃣ PERMISSION (CRITICAL 🚨)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update verification.");

                // ===============================
                // 5️⃣ FETCH EMPLOYEE
                // ===============================
                var employee =
                    await _unitOfWork.Employees.GetByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId,
                        true);

                if (employee == null)
                    throw new ApiException("Employee not found.", 404);

                // ===============================
                // 6️⃣ UPDATE STATUS
                // ===============================
                var updated =
                    await _unitOfWork.Employees
                        .UpdateVerificationStatusByTabAsync(
                            request.DTO.TabInfoType,
                            request.DTO.Prop.EmployeeId,
                            validation.UserEmployeeId,
                            request.DTO.IsVerified,
                            ct);

                if (!updated)
                {
                    _logger.LogWarning(
                        "Verification update failed | EmpId={EmpId} | Tab={Tab}",
                        request.DTO.Prop.EmployeeId,
                        request.DTO.TabInfoType);

                    throw new ApiException("Verification update failed.", 500);
                }

                _logger.LogInformation("UpdateVerificationStatus success");

                return ApiResponse<bool>.Success(
                    true,
                    "Verification status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verification update error");

                throw; // 🚨 MUST
            }
        }
    }

}

