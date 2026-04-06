using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.InsuranceInfo.Handlers
{
    public class GetAllInsuranceForEmployee : IRequest<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
    {
        public GetInsuranceForEmployeeDDLRequestDTO DTO { get; }

        public GetAllInsuranceForEmployee(
            GetInsuranceForEmployeeDDLRequestDTO dto)
        {
            DTO = dto;
        }
        public class GetAllInsuranceForEmployeeCommandHandler
    : IRequestHandler<
        GetAllInsuranceForEmployee,
        ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ILogger<GetAllInsuranceForEmployeeCommandHandler> _logger;
            private readonly ICommonRequestService _commonRequestService;
            private readonly IPermissionService _permissionService;

            public GetAllInsuranceForEmployeeCommandHandler(
                IUnitOfWork unitOfWork,
                ILogger<GetAllInsuranceForEmployeeCommandHandler> logger,
                ICommonRequestService commonRequestService,
                IPermissionService permissionService)
            {
                _unitOfWork = unitOfWork;
                _logger = logger;
                _commonRequestService = commonRequestService;
                _permissionService = permissionService;
            }

            public async Task<ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>> Handle(
                GetAllInsuranceForEmployee request,
                CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("🔹 GetAllInsuranceForEmployee started");

                    // ===============================
                    // ✅ AUTH VALIDATION
                    // ===============================
                    var validation = await _commonRequestService.ValidateRequestAsync();

                    if (!validation.Success)
                        throw new UnauthorizedAccessException(validation.ErrorMessage);

                    // ===============================
                    // ✅ RBAC PERMISSION CHECK
                    // ===============================
                    // (agar chaho enable karo)
                    // var hasAccess = await _permissionService.HasAccessAsync(
                    //     validation.RoleId,
                    //     Modules.PolicyType,
                    //     Operations.View);

                    // if (!hasAccess)
                    //     throw new UnauthorizedAccessException("Access denied.");

                    // ===============================
                    // ✅ REQUEST VALIDATION
                    // ===============================
                    if (request?.DTO == null)
                        throw new ValidationErrorException("Invalid request.");

                    // ===============================
                    // 🔥 FETCH DATA (REPOSITORY CALL)
                    // ===============================
                    var result = await _unitOfWork.PolicyTypeInsuranceMappingRepository.GetMapInsuranceDDLForEmployeeMappingAsync(
                            validation.TenantId, true ,
                            false); // only active

                    _logger.LogInformation("✅ GetAllInsuranceForEmployee success");

                    return ApiResponse<List<GetPolicyTypeInsuranceMappingResponseDTO>>
                        .Success(result, "Insurance list fetched successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ GetAllInsuranceForEmployee failed");
                    throw;
                }
            }
        }

    }
     
}
