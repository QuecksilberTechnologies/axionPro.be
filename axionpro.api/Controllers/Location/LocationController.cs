// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Coordinates HTTP requests for Location operations.
// ================================================================

using axionpro.application.DTOs.Operation;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Location;
using axionpro.application.DTOS.Role;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Features.DesignationCmd.Handlers;
using axionpro.application.Features.GenderCmd.Handlers;
using axionpro.application.Features.LocationCmd.Handlers;
using axionpro.application.Features.OperationCmd.Commands;
using axionpro.application.Features.OperationCmd.Queries;
using axionpro.application.Features.RoleCmd.Handlers;
using axionpro.application.Features.TransportCmd.Commands;
using axionpro.application.Features.TransportCmd.Queries;
using axionpro.application.Interfaces.ILogger;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.Location
{
    /// <summary>
    /// handled-DDL/Option-related-actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;  // Logger service ka declaration

        public LocationController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }  
        
       
        /// <summary>
        /// Used-In-Angular: retrieves countries.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LocationsApi.getCountries (app/core/services/locations-api.ts:63).</para>
        /// <para>Angular purpose: retrieves countries.</para>
        /// <para>Integrated UI page(s): /auth/register-tenant; /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); Registration (app/features/authentication/registration/registration.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
        /// </remarks>
        [HttpGet("country/option")] 
        public async Task<IActionResult> getCountry([FromQuery] GetCountryOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get Country : {requestDTO.UserEmployeeId}");

            var command = new GetCountryQuery(requestDTO);
            var result = await _mediator.Send(command);
            return Ok(result);        }

        /// <summary>
        /// Used-In-Angular: retrieves states.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LocationsApi.getStates (app/core/services/locations-api.ts:70).</para>
        /// <para>Angular purpose: retrieves states.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); EmployeeContactForm (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-form/employee-contact-form.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
        /// </remarks>
        [HttpGet("State/option")]   
        public async Task<IActionResult> getState([FromQuery] GetStateOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get State : {requestDTO.UserEmployeeId}");
            var command = new GetStateQuery(requestDTO);
            var result = await _mediator.Send(command);         
            return Ok(result);
        }
        /// <summary>
        /// Used-In-Angular: retrieves districts.
        /// </summary>
        /// <remarks>
        /// <para>Angular usage status: Used-In-Angular.</para>
        /// <para>Angular function(s): LocationsApi.getDistricts (app/core/services/locations-api.ts:77).</para>
        /// <para>Angular purpose: retrieves districts.</para>
        /// <para>Integrated UI page(s): /app/designations; /app/employees; /app/tenants/new; /app/tenants/:tenantId/edit; /app/tenants; /app/tenant-locations/new; /app/tenant-locations/:tenantLocationId/edit; /app/tenant-locations</para>
        /// <para>Angular UI component(s): LookupStore (app/core/stores/lookup.store.ts); EmployeeContactForm (app/features/user-menu/employee-profile/employee-contact-info/employee-contact-form/employee-contact-form.ts); DashboardHostStore (app/features/dashboard/dashboard-host/dashboard-host.store.ts); DepartmentsStore (app/features/departments/departments.store.ts); Designations (app/features/designations/designations.ts); Employees (app/features/employees/employees.ts); TenantDetail (app/features/host/tenants/tenant-detail/tenant-detail.ts); TenantForm (app/features/host/tenants/tenant-form/tenant-form.ts)</para>
        /// </remarks>
        [HttpGet("District/option")]         
        public async Task<IActionResult> getDistrict([FromQuery] GetDistrictOptionRequestDTO requestDTO)
        {
            _logger.LogInfo($"Received request to get District : {requestDTO.UserEmployeeId}");
            var command = new GetDistrictQuery(requestDTO);
            var result = await _mediator.Send(command);          
            return Ok(result);
        }
    }



}
