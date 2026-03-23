using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    // ============================
    // COMMAND
    // ============================
    public class UpdateSectionBulkCommand
        : IRequest<ApiResponse<bool>>
    {
        public UpdateEmployeeSectionStatusRequestDTO DTO { get; }

        public UpdateSectionBulkCommand(UpdateEmployeeSectionStatusRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ============================
    // HANDLER
    // ============================
    public class UpdateSectionBulkCommandHandler
        : IRequestHandler<UpdateSectionBulkCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateSectionBulkCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;

        public UpdateSectionBulkCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateSectionBulkCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<bool>> Handle(
    UpdateSectionBulkCommand request,
    CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("UpdateSectionBulk started");

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

                // ===============================
                // 3️⃣ DECODE EMPLOYEE ID
                // ===============================
                var employeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    throw new ValidationErrorException("Invalid employee.");

                // ===============================
                // 4️⃣ VALIDATE SECTIONS
                // ===============================
                if (request.DTO.Sections == null || !request.DTO.Sections.Any())
                    throw new ValidationErrorException("No section selected.");

                // ===============================
                // 5️⃣ PERMISSION (YOUR PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Update);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to update sections.");

                // ===============================
                // 6️⃣ LOOP (OPTIMIZED)
                // ===============================
                foreach (var section in request.DTO.Sections)
                {
                    if (!Enum.IsDefined(typeof(TabInfoType), section.TabInfoType))
                    {
                        _logger.LogWarning(
                            "Invalid TabInfoType | Value={Value} | EmpId={EmpId}",
                            section.TabInfoType,
                            employeeId
                        );
                        continue;
                    }

                    var updated =
                        await _unitOfWork.Employees
                            .UpdateSectionVerifyStatusAsync(
                                (int)section.TabInfoType,
                                employeeId,
                                validation.TenantId,
                                section.IsVerified ?? false,
                                section.IsEditAllowed ?? false,
                                validation.UserEmployeeId,
                                ct
                            );

                    if (!updated)
                    {
                        _logger.LogWarning(
                            "Section update failed | Section={Section} | EmpId={EmpId}",
                            section.TabInfoType,
                            employeeId
                        );
                    }
                }

                _logger.LogInformation("UpdateSectionBulk success");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<bool>.Success(
                    true,
                    "Section status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSectionBulk failed");

                throw; // 🚨 MUST
            }
        }


    }

}
