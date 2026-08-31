// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Enum operations.
// ================================================================

using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Entity;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.EnumDTO;
using axionpro.application.Features.EmployeeCmd.BankInfo.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.EnumTypes
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnumController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration
        public EnumController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /// <summary>
        /// Used-In-Angular: retrieves all currencies.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): EnumApi.getAllCurrencies (app/core/services/enum-api.ts:30).</para>
        /// <para>Angular purpose: retrieves all currencies.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts); Tenants (app/features/host/tenants/tenants.ts)</para>
        /// </remarks>

        [HttpGet("get-all-currencies")]


        public async Task<IActionResult> GetCurrencies([FromQuery] GetCurrencyRequestDTO dto)
        {

            _logger.LogInfo("Fetching all currencies.");

            var data = CurrencyProvider.GetAll(dto.IsActive);
          


            return Ok(data);
        }
     
        

    }
}

