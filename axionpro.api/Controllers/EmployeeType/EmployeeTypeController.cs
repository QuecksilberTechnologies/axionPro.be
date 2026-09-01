// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes employee-type endpoints and delegates application operations to handlers.
// ================================================================

using axionpro.application.Constants;
using axionpro.application.DTOs.EmployeeType;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.EmployeeType;
using axionpro.application.Features.EmployeeTypeCmd.Handlers;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EmployeeType
{
    /// <summary>
    /// Exposes employee-type endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeTypeController : ControllerBase
    {
        #region Fields

        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeTypeController> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeController"/> class.
        /// </summary>
        public EmployeeTypeController(
            IMediator mediator,
            ILogger<EmployeeTypeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        #endregion

        #region Queries

        /// <summary>
        /// Used-In-Angular: retrieves employee types.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves all employee type.</para>
        /// <para>Handler flow: No application request/handler class was statically resolved from the controller action.</para>
        /// <para>Response DTO property analysis: No concrete response DTO properties were statically resolved from the request/handler declaration.</para>
        /// <para>Angular function(s): EmployeeTypesAPI.getEmployeeTypes (app/core/services/employee-types-api.ts:52).</para>
        /// <para>Angular purpose: retrieves employee types.</para>
        /// <para>Integrated UI page(s): No static Angular route was resolved; see Angular UI component(s).</para>
        /// <para>Angular UI component(s): No consuming Angular component was statically resolved.</para>
        /// </remarks>
        [HttpGet("get")]
        public IActionResult GetAllEmployeeType(
            [FromQuery] application.DTOS.Employee.Type.GetEmployeeTypeRequestDTO requestDto)
        {
            _logger.LogInformation("Fetching all employee types.");

            var employeeTypes = new List<GetEmployeeTypeResponseDTO>
            {
                new()
                {
                    Id = 1,
                    TypeName = "Full-Time",
                    Description = "Permanent employee with all benefits",
                    IsActive = true
                },
                new()
                {
                    Id = 2,
                    TypeName = "Contract",
                    Description = "Contract-based employee",
                    IsActive = true
                },
                new()
                {
                    Id = 3,
                    TypeName = "Intern",
                    Description = "Internship employee",
                    IsActive = true
                },
                new()
                {
                    Id = 4,
                    TypeName = "Freelancer",
                    Description = "External resource",
                    IsActive = false
                }
            };

            return Ok(ApiResponse<List<GetEmployeeTypeResponseDTO>>.Success(
                employeeTypes,
                AppConstants.SuccessMessages.EmployeeTypesRetrieved));
        }

        /// <summary>
        /// Used-In-Angular: retrieves employee type options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>API endpoint purpose: retrieves employee type option.</para>
        /// <para>Handler flow: GetEmployeeTypeOptionQuery is processed by GetEmployeeTypeOptionQueryHandler; operation(s): GetEmployeeTypesOptionAsync.</para>
        /// <para>Response DTO property analysis: ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); GetEmployeeTypeResponseOptionDTO: Id (int), TypeName (string?)</para>
        /// <para>Angular function(s): EmployeeTypesAPI.getEmployeeTypeOptions (app/core/services/employee-types-api.ts:59).</para>
        /// <para>Angular purpose: retrieves employee type options.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
        /// </remarks>
        [HttpGet("option")]
        public async Task<IActionResult> GetAllEmployeeType([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInformation("Fetching employee-type options.");

            var result = await _mediator.Send(new GetEmployeeTypeOptionQuery(requestDTO));
            return Ok(result);
        }

        #endregion
    }
}
