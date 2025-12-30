using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
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
                // =====================================================
                // STEP 1: Validate User / Tenant / Token
                // =====================================================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // =====================================================
                // STEP 2: Decode EmployeeId (Tenant safe)
                // =====================================================
                long employeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

                if (employeeId <= 0)
                    return ApiResponse<bool>.Fail("Invalid employee.");

                // =====================================================
                // STEP 3: Validate Sections
                // =====================================================
                if (request.DTO.Sections == null || !request.DTO.Sections.Any())
                    return ApiResponse<bool>.Fail("No section selected.");

                // =====================================================
                // STEP 4: (Optional) Permission check
                // =====================================================
                // var permissions = await _permissionService
                //     .GetPermissionsAsync(validation.RoleId);

                // =====================================================
                // STEP 5: LOOP SECTIONS (LOGICAL LOOP ONLY)
                // 🔥 No data loading here
                // 🔥 Each loop = single optimized SQL in repo
                // =====================================================
                foreach (var section in request.DTO.Sections)
                {
                    if (string.IsNullOrWhiteSpace(section.SectionName))
                        continue;

                    bool updated = await _unitOfWork.Employees.UpdateSectionVerifyStatusAsync( section.SectionName.Trim().ToLowerInvariant(), // decides TABLE
                            employeeId,
                            validation.TenantId,
                            section.IsVerified ?? false,
                            section.IsEditAllowed ?? false,
                            validation.UserEmployeeId,
                            ct);

                    if (!updated)
                    {
                        _logger.LogWarning(
                            "Section update failed | Section={Section} | EmpId={EmpId}",
                            section.SectionName,
                            employeeId);
                    }
                }

                return ApiResponse<bool>.Success(
                    true,
                    "Section status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSectionBulkCommand failed");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }
    }
}
