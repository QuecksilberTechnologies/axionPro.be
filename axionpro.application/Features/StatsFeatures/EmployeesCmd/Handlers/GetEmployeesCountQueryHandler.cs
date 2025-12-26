using AutoMapper;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.DTOS.StoreProcedures.DashboardSummeries;
using axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers;
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.StatsFeatures.EmployeesCmd.Handlers
{


    public class GetEmployeeCountsQuery : IRequest<ApiResponse<EmployeeCountResponseStatsSp>>
    {
        public EmployeeCountRequestStatsSp DTO { get; }

        public GetEmployeeCountsQuery(EmployeeCountRequestStatsSp dTO)
        {
            DTO = dTO;
        }
    }
    public class GetEmployeesCountQueryHandler: IRequestHandler<GetEmployeeCountsQuery, ApiResponse<EmployeeCountResponseStatsSp>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetEmployeesCountQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetEmployeesCountQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetEmployeesCountQueryHandler> logger,
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

        public async Task<ApiResponse<EmployeeCountResponseStatsSp>> Handle(
            GetEmployeeCountsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Validate Request
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<EmployeeCountResponseStatsSp>
                        .Fail(validation.ErrorMessage);              

                // 3️⃣ Permission check (optional strict)
                var permissions = await _permissionService
                    .GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("PersonalInfo"))
                {
                    // optional block
                }

                // 4️⃣ Fetch SP data
                var spRecords = await _unitOfWork.StoreProcedureRepository
                    .GetEmployeeCountsAsync(
                        validation.TenantId);

                if (spRecords == null)
                    return ApiResponse<EmployeeCountResponseStatsSp>
                        .Fail("No identity information found.");
                
                return ApiResponse<EmployeeCountResponseStatsSp>
                    .Success(spRecords, "Identity information retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching identity info");

                return ApiResponse<EmployeeCountResponseStatsSp>
                    .Fail(
                        "An unexpected error occurred.",
                        new List<string> { ex.Message });
            }
        }
    }








}
