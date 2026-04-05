using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.PolicyType;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Features.PolicyTypeCmd.Handlers
{

    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetAllUnStructuredPolicyTypeCommand : IRequest<ApiResponse<List<GetUnStructuredPolicyTypeResponseDTO>>>
    {
        public GetAllUnStructuredPolicyTypeRequestDTO DTO { get; }

        public GetAllUnStructuredPolicyTypeCommand(GetAllUnStructuredPolicyTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class GetAllUnStructuredPolicyTypeCommandHandler
    : IRequestHandler<GetAllUnStructuredPolicyTypeCommand,
        ApiResponse<List<GetUnStructuredPolicyTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllUnStructuredPolicyTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public GetAllUnStructuredPolicyTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllUnStructuredPolicyTypeCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<List<GetUnStructuredPolicyTypeResponseDTO>>> Handle(
            GetAllUnStructuredPolicyTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetAllUnStructuredPolicyType started");

                // ===============================
                // 1️⃣ VALIDATION (AUTH)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK
                // ===============================
                // 🔥 Apna RBAC pattern
                // var hasAccess = await _permissionService.HasAccessAsync(
                //     validation.RoleId,
                //     Modules.PolicyType,
                //     Operations.View);

                // if (!hasAccess)
                //     throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 3️⃣ DATA FETCH
                // ===============================
                var data = await _unitOfWork
                    .UnStructuredEmployeePolicyTypeMappingRepository
                    .GetAllAsync(
                        validation.TenantId,
                        request.DTO.PolicyTypeId,                         
                        request.DTO.IsActive);

                // ===============================
                // 4️⃣ MAPPING (ENTITY → DTO)
                // ===============================
                var result = data.Select(x => new GetUnStructuredPolicyTypeResponseDTO
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    EmployeeTypeId = x.EmployeeTypeId,
                    EmployeeTypeName = x.EmployeeType != null
                        ? x.EmployeeType.TypeName
                        : string.Empty,

                    PolicyTypeId = x.PolicyTypeId,
                    PolicyTypeName = x.PolicyType.PolicyName != null
                        ? x.PolicyType.PolicyName
                        : string.Empty,
                   

                    IsActive = x.IsActive,
                    StartDate = x.StartDate
                }).ToList();

                _logger.LogInformation("✅ GetAllUnStructuredPolicyType success");

                return ApiResponse<List<GetUnStructuredPolicyTypeResponseDTO>>
                    .Success(result, "Data fetched successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetAllUnStructuredPolicyType failed");
                throw;
            }
        }
    }
}
 