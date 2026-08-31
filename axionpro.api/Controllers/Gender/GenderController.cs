// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Gender operations.
// ================================================================

using axionpro.api.Controllers.Leave;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Gender;
using axionpro.application.Features.GenderCmd.Handlers;
using axionpro.application.Features.GenderCmd.Queries;
using axionpro.application.Features.LeaveCmd.Commands;
using axionpro.application.Features.LeaveCmd.Queries;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace axionpro.api.Controllers.Gender
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GenderController> _logger;

        public GenderController(IMediator mediator, ILogger<GenderController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Used-In-Angular: retrieves gender options.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): UsersApi.getGenderOptions (app/core/services/users-api.ts:47).</para>
        /// <para>Angular purpose: retrieves gender options.</para>
        /// <para>Integrated UI page(s): /auth/register-tenant; /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); Registration (app/features/authentication/registration/registration.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
        /// </remarks>
        [HttpGet("option")]               
        public async Task<IActionResult> getGender([FromQuery] GetOptionRequestDTO requestDTO)
        {
            _logger.LogInformation($"Received request to get Gender : {requestDTO.UserEmployeeId}");

            var command = new GetGenderOptionQuery(requestDTO);
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        //  ✅ Get All Gender 
        #region Unused
        //         /// <summary>
        //         /// Not-Used-In-Angular.
        //         /// </summary>
        //         /// <remarks>
        //         /// <para>Angular usage status: Not-Used-In-Angular.</para>
        //         /// <para>No active Angular HTTP call with the same HTTP method and normalized route was found in the scanned Angular source.</para>
        //         /// <para>Backend endpoint: GET /api/gender/get.</para>
        //         /// </remarks>
        //         [HttpGet("get")]
        //         public async Task<IActionResult> GetAllGenderAsync([FromQuery] GetGenderRequestDTO? getGenderRequestDTO)
        //         {
        //             _logger.LogInformation("Fetching all LeavePolicies...");
        //             var query = new GetAllGenderQuery(getGenderRequestDTO);
        //             var result = await _mediator.Send(query);
        //             return Ok(result);
        //         }
        #endregion
         

    }
}
