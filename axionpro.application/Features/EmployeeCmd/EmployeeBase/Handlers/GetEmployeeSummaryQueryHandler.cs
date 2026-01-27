using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Employee.BaseEmployee;
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace axionpro.application.Features.EmployeeCmd.Handlers
{
    public class GetEmployeeSummaryQuery : IRequest<ApiResponse<SummaryEmployeeInfo>>
    {
        public GetEmployeeSummaryRequestDTO DTO { get; }

        public GetEmployeeSummaryQuery(GetEmployeeSummaryRequestDTO dTO)
        {
            DTO = dTO;
        }
        
    }
    public class GetEmployeeSummaryQueryHandler
       : IRequestHandler<GetEmployeeSummaryQuery, ApiResponse<SummaryEmployeeInfo>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeeSummaryQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeeSummaryQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeeSummaryQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<SummaryEmployeeInfo>> Handle(
            GetEmployeeSummaryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // =====================================================
                // 🔐 STEP 1: COMMON VALIDATION
                // =====================================================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<SummaryEmployeeInfo>
                        .Fail(validation.ErrorMessage);

                // =====================================================
                // 🔓 STEP 2: DECODE EMPLOYEE ID
                // =====================================================
                long employeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (employeeId <= 0)
                    return ApiResponse<SummaryEmployeeInfo>
                        .Fail("Invalid EmployeeId.");

                // =====================================================
                // 🔑 STEP 3: PERMISSION CHECK (OPTIONAL / SOFT)
                // =====================================================
                var permissions =
                    await _permissionService.GetPermissionsAsync(
                        validation.RoleId);

                if (!permissions.Contains("ViewEmployeeSummary"))
                {
                    // optional hard stop
                    // return ApiResponse<SummaryEmployeeInfo>
                    //     .Fail("You do not have permission to view employee summary.");
                }

                // =====================================================
                // 📦 STEP 4: FETCH SUMMARY FROM REPOSITORY
                // =====================================================
                var summary =
                    await _unitOfWork.Employees
                        .BuildEmployeeSummaryAsync(
                            employeeId,
                            request.DTO.IsActive);

                if (summary == null)
                {
                    _logger.LogInformation(
                        "No Employee Summary found | EmployeeId: {EmployeeId}",
                        employeeId);

                    return ApiResponse<SummaryEmployeeInfo>
                        .Fail("No employee summary found.");
                }
              var resultList = ProjectionHelper.ToGetSummaryResponseDTO(summary, _idEncoderService, validation.Claims.TenantEncriptionKey, _commonRequestService);



                // =====================================================
                // ✅ STEP 5: SUCCESS RESPONSE
                // =====================================================
                return ApiResponse<SummaryEmployeeInfo>
                    .Success(
                        resultList,
                        "Employee summary retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching Employee Summary | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                return ApiResponse<SummaryEmployeeInfo>
                    .Fail(
                        "An unexpected error occurred.",
                        new List<string> { ex.Message });
            }
        }
    }

}
