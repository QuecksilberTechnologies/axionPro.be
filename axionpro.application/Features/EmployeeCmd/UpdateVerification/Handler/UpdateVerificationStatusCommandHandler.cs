
 using AutoMapper;
using axionpro.application.Common.Enums;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
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
using System;

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
                // =====================================================
                // 1️⃣ VALIDATE REQUEST
                // =====================================================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    return ApiResponse<bool>.Fail("Invalid employee.");

                if (!Enum.IsDefined(typeof(TabInfoType), request.DTO.TabInfoType))
                    return ApiResponse<bool>.Fail("Invalid section type.");

                // =====================================================
                // 2️⃣ VERIFY EMPLOYEE EXISTS
                // =====================================================
                var employee = await _unitOfWork.Employees.GetByIdAsync(
                    request.DTO.Prop.EmployeeId,
                    request.DTO.Prop.TenantId,
                    true);

                if (employee == null)
                    return ApiResponse<bool>.Fail("Employee not found.");

                // =====================================================
                // 3️⃣ UPDATE VERIFICATION STATUS (COMMON REPO METHOD)
                // =====================================================
                bool updated = await _unitOfWork.Employees
                    .UpdateVerificationStatusByTabAsync(
                        request.DTO.TabInfoType,
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.UserEmployeeId,
                        request.DTO.IsVerified,
                        ct);

                if (!updated)
                {
                    _logger.LogWarning(
                        "Verification status update failed | EmpId={EmpId} | Tab={Tab}",
                        request.DTO.Prop.EmployeeId,
                        request.DTO.TabInfoType);

                    return ApiResponse<bool>.Fail("Verification update failed.");
                }

                return ApiResponse<bool>.Success(
                    true,
                    "Verification status updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verification update error");
                return ApiResponse<bool>.Fail("Unexpected error occurred.");
            }
        }
    }
}

