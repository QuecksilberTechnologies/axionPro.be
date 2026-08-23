// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated TenantConfiguration endpoint groups.
// ================================================================

using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces.ILogger;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.TenantConfiguration;

/// <summary>Provides common dependencies for TenantConfiguration endpoint groups.</summary>
[Authorize]
[ApiController]
public abstract class TenantConfigurationControllerBase(IMediator mediator, ILoggerService logger) : ControllerBase
{
    /// <summary>Dispatches TenantConfiguration requests.</summary>
    protected IMediator Mediator { get; } = mediator;
    /// <summary>Records TenantConfiguration endpoint activity.</summary>
    protected ILoggerService Logger { get; } = logger;
}

/// <summary>Exposes Tenant location configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class TenantLocationController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Tenant Location CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateTenantLocationRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received TenantLocation create request."); return Ok(await Mediator.Send(new CreateTenantLocationCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received TenantLocation get-by-id request."); return Ok(await Mediator.Send(new GetTenantLocationByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] TenantLocationFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received TenantLocation list request."); return Ok(await Mediator.Send(new GetTenantLocationsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateTenantLocationRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received TenantLocation update request."); return Ok(await Mediator.Send(new UpdateTenantLocationCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantLocationStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received TenantLocation status request."); return Ok(await Mediator.Send(new UpdateTenantLocationStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received TenantLocation delete request."); return Ok(await Mediator.Send(new DeleteTenantLocationCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes attendance-policy configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class AttendancePolicyController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Attendance Policy CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateAttendancePolicyRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy create request."); return Ok(await Mediator.Send(new CreateAttendancePolicyCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:int}")] public async Task<IActionResult> GetById(int id, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy get-by-id request."); return Ok(await Mediator.Send(new GetAttendancePolicyByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] AttendancePolicyFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy list request."); return Ok(await Mediator.Send(new GetAttendancePoliciesQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateAttendancePolicyRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy update request."); return Ok(await Mediator.Send(new UpdateAttendancePolicyCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateAttendancePolicyStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy status request."); return Ok(await Mediator.Send(new UpdateAttendancePolicyStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) { Logger.LogInfo("Received AttendancePolicy delete request."); return Ok(await Mediator.Send(new DeleteAttendancePolicyCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes employee-location-assignment configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeLocationAssignmentController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Location Assignment CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeLocationAssignmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment create request."); return Ok(await Mediator.Send(new CreateEmployeeLocationAssignmentCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeLocationAssignmentByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeLocationAssignmentFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment list request."); return Ok(await Mediator.Send(new GetEmployeeLocationAssignmentsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeLocationAssignmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment update request."); return Ok(await Mediator.Send(new UpdateEmployeeLocationAssignmentCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeLocationAssignmentStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment status request."); return Ok(await Mediator.Send(new UpdateEmployeeLocationAssignmentStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment delete request."); return Ok(await Mediator.Send(new DeleteEmployeeLocationAssignmentCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes employee-device-enrollment configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeDeviceEnrollmentController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Device Enrollment CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeDeviceEnrollmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment create request."); return Ok(await Mediator.Send(new CreateEmployeeDeviceEnrollmentCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeDeviceEnrollmentByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeDeviceEnrollmentFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment list request."); return Ok(await Mediator.Send(new GetEmployeeDeviceEnrollmentsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeDeviceEnrollmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment update request."); return Ok(await Mediator.Send(new UpdateEmployeeDeviceEnrollmentCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeDeviceEnrollmentStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment status request."); return Ok(await Mediator.Send(new UpdateEmployeeDeviceEnrollmentStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment delete request."); return Ok(await Mediator.Send(new DeleteEmployeeDeviceEnrollmentCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes employee work-arrangement configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkArrangementController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Arrangement CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkArrangementRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkArrangementCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkArrangementByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkArrangementFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement list request."); return Ok(await Mediator.Send(new GetEmployeeWorkArrangementsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkArrangementRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkArrangementCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkArrangementStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkArrangementStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkArrangementCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes employee work-pattern configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkPatternController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Pattern CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkPatternRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkPatternCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkPatternByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkPatternFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern list request."); return Ok(await Mediator.Send(new GetEmployeeWorkPatternsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkPatternRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkPatternCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkPatternStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkPatternStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkPatternCommand(id), ct)); }
    #endregion
}

/// <summary>Exposes employee temporary work-mode override endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkModeOverrideController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Mode Override CRUD
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkModeOverrideCommand(dto), ct)); }
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkModeOverrideByIdQuery(id), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkModeOverrideFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride list request."); return Ok(await Mediator.Send(new GetEmployeeWorkModeOverridesQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkModeOverrideCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkModeOverrideStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkModeOverrideStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkModeOverrideCommand(id), ct)); }
    #endregion
}
