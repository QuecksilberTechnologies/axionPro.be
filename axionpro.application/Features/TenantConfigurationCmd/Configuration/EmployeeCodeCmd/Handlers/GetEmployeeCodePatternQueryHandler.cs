using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers
{
    // ======================= QUERY ============================
    public class GetEmployeeCodePatternQuery
        : IRequest<ApiResponse<GetEmployeeCodePatternResponseDTO>>
    {
        public EmployeeCodePatternRequestDTO DTO { get; }

        public GetEmployeeCodePatternQuery(EmployeeCodePatternRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================= HANDLER ============================
    public class GetEmployeeCodePatternQueryHandler
        : IRequestHandler<GetEmployeeCodePatternQuery, ApiResponse<GetEmployeeCodePatternResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<GetEmployeeCodePatternQueryHandler> _logger;

        public GetEmployeeCodePatternQueryHandler(
            IUnitOfWork unitOfWork,
            ICommonRequestService commonRequestService,
            ILogger<GetEmployeeCodePatternQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        public async Task<ApiResponse<GetEmployeeCodePatternResponseDTO>> Handle(
            GetEmployeeCodePatternQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // ----------------------------------------------------------
                // 1️⃣ VALIDATE COMMON REQUEST (TenantId, Role, UserId)
                // ----------------------------------------------------------
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<GetEmployeeCodePatternResponseDTO>
                        .Fail(validation.ErrorMessage);

                // ----------------------------------------------------------
                // 2️⃣ Assign TenantId coming from validation
                // ----------------------------------------------------------
                request.DTO.TenantId = validation.TenantId;

                // ----------------------------------------------------------
                // 3️⃣ CALL REPOSITORY
                // ----------------------------------------------------------
                var pattern = await _unitOfWork
                    .TenantEmployeeCodePatternRepository.GetTenantEmployeeCodePatternAsync(request.DTO.TenantId, request.DTO.IsActive);

                if (pattern == null)
                {
                    return ApiResponse<GetEmployeeCodePatternResponseDTO>
                        .Fail("No active employee code pattern found for this tenant.");
                }

                // ----------------------------------------------------------
                // 4️⃣ SUCCESS RESPONSE
                // ----------------------------------------------------------
                return ApiResponse<GetEmployeeCodePatternResponseDTO>
                    .Success(pattern, "Pattern fetched successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "❌ Error fetching Employee Code Pattern for TenantId: {TenantId}",
                    request.DTO?.TenantId);

                return ApiResponse<GetEmployeeCodePatternResponseDTO>
                    .Fail("An unexpected error occurred.");
            }
        }
    }
}

