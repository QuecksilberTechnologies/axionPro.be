// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated TenantConfiguration endpoint groups.
// ================================================================

using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Features.EmployeeCmd.EmployeeDeviceEnrollment.Handlers;
using axionpro.application.Features.EmployeeCmd.EmployeeWorkInfo.Handlers;
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
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, [FromQuery] TenantLocationAccessRequestDTO accessRequest, CancellationToken ct) { Logger.LogInfo("Received TenantLocation get-by-id request."); return Ok(await Mediator.Send(new GetTenantLocationByIdQuery(id, accessRequest), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] TenantLocationFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received TenantLocation list request."); return Ok(await Mediator.Send(new GetTenantLocationsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateTenantLocationRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received TenantLocation update request."); return Ok(await Mediator.Send(new UpdateTenantLocationCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantLocationStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received TenantLocation status request."); return Ok(await Mediator.Send(new UpdateTenantLocationStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, [FromQuery] TenantLocationAccessRequestDTO accessRequest, CancellationToken ct) { Logger.LogInfo("Received TenantLocation delete request."); return Ok(await Mediator.Send(new DeleteTenantLocationCommand(id, accessRequest), ct)); }
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
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeLocationAssignmentByIdQuery(id, permissionRequest), ct)); }
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeLocationAssignmentFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment list request."); return Ok(await Mediator.Send(new GetEmployeeLocationAssignmentsQuery(filter), ct)); }
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeLocationAssignmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment update request."); return Ok(await Mediator.Send(new UpdateEmployeeLocationAssignmentCommand(dto), ct)); }
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeLocationAssignmentStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment status request."); return Ok(await Mediator.Send(new UpdateEmployeeLocationAssignmentStatusCommand(dto), ct)); }
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeLocationAssignment delete request."); return Ok(await Mediator.Send(new DeleteEmployeeLocationAssignmentCommand(id, permissionRequest), ct)); }
    #endregion
}

/// <summary>Exposes employee-device-enrollment configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeDeviceEnrollmentController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Device Enrollment CRUD
    /// <summary>Creates the employee-to-device mapping required before face, card, or PIN can be sent to a physical device.</summary>
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeDeviceEnrollmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment create request."); return Ok(await Mediator.Send(new CreateEmployeeDeviceEnrollmentCommand(dto), ct)); }
    /// <summary>Reads one safe device-user view so Angular can refresh credential and command-confirmation status without exposing device enroll IDs.</summary>
    [HttpGet("get-by-id/{id}")] public async Task<IActionResult> GetById(string id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeDeviceEnrollmentByIdQuery(id, permissionRequest), ct)); }
    /// <summary>Lists employee device users for the device-user screen and filtered refreshes.</summary>
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeDeviceEnrollmentFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment list request."); return Ok(await Mediator.Send(new GetEmployeeDeviceEnrollmentsQuery(filter), ct)); }
    /// <summary>Changes only employee-specific access validity and working windows; it deliberately cannot enable or disable a physical device user.</summary>
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeDeviceEnrollmentRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment update request."); return Ok(await Mediator.Send(new UpdateEmployeeDeviceEnrollmentCommand(dto), ct)); }
    /// <summary>Enables or disables the existing device user through the durable queue, keeping face, card, and PIN credentials intact.</summary>
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeDeviceEnrollmentStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment status request."); return Ok(await Mediator.Send(new UpdateEmployeeDeviceEnrollmentStatusCommand(dto), ct)); }
    /// <summary>Queues a selected Angular JPEG or PNG face file for an already enrolled employee; raw biometric data is never persisted in the business table.</summary>
    [HttpPost("face/upsert")] [Consumes("multipart/form-data")] public async Task<IActionResult> UpsertFace([FromForm] UpsertEmployeeDeviceFaceRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received employee device face enrollment request."); return Ok(await Mediator.Send(new UpsertEmployeeDeviceFaceCommand(dto), ct)); }
    /// <summary>Queues a write-only employee PIN for an existing device user; use this instead of storing or reading a PIN in Angular.</summary>
    [HttpPost("pin/upsert")] public async Task<IActionResult> UpsertPin([FromBody] UpsertEmployeeDevicePinRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received employee device PIN enrollment request."); return Ok(await Mediator.Send(new UpsertEmployeeDevicePinCommand(dto), ct)); }
    /// <summary>Binds one Host-issued Available card to the employee device user and queues the physical card enrollment.</summary>
    [HttpPost("card/bind")] public async Task<IActionResult> BindCard([FromBody] BindEmployeeDeviceCardRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received employee device card bind request."); return Ok(await Mediator.Send(new BindEmployeeDeviceCardCommand(dto), ct)); }
    /// <summary>Queues removal of exactly one employee credential, preserving the other credentials and the employee-device mapping.</summary>
    [HttpPost("credential/remove")] public async Task<IActionResult> RemoveCredential([FromBody] RemoveEmployeeDeviceCredentialRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received employee device credential removal request."); return Ok(await Mediator.Send(new RemoveEmployeeDeviceCredentialCommand(dto), ct)); }
    /// <summary>Queues complete removal of an employee from one physical device and soft-deletes only that employee-device mapping.</summary>
    [HttpDelete("delete/{id}")] public async Task<IActionResult> Delete(string id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeDeviceEnrollment delete request."); return Ok(await Mediator.Send(new DeleteEmployeeDeviceEnrollmentCommand(id, permissionRequest), ct)); }
    #endregion
}

/// <summary>Exposes employee work-arrangement configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkArrangementController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Arrangement CRUD
    /// <summary>Creates the employee's long-running Office, Remote, Hybrid, or WFH attendance arrangement.</summary>
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkArrangementRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkArrangementCommand(dto), ct)); }
    /// <summary>Reads one employee work arrangement for edit or detail display.</summary>
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkArrangementByIdQuery(id, permissionRequest), ct)); }
    /// <summary>Lists work arrangements for employee profile and tenant administration screens.</summary>
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkArrangementFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement list request."); return Ok(await Mediator.Send(new GetEmployeeWorkArrangementsQuery(filter), ct)); }
    /// <summary>Changes an existing employee work arrangement while retaining its server-owned audit information.</summary>
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkArrangementRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkArrangementCommand(dto), ct)); }
    /// <summary>Activates or deactivates an arrangement without deleting its historical record.</summary>
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkArrangementStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkArrangementStatusCommand(dto), ct)); }
    /// <summary>Soft-deletes a work arrangement when it is no longer valid for the employee.</summary>
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkArrangement delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkArrangementCommand(id, permissionRequest), ct)); }
    #endregion
}

/// <summary>Exposes employee work-pattern configuration endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkPatternController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Pattern CRUD
    /// <summary>Creates a weekday-specific work pattern beneath an employee's main work arrangement.</summary>
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkPatternRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkPatternCommand(dto), ct)); }
    /// <summary>Reads one weekday work pattern for edit or detail display.</summary>
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkPatternByIdQuery(id, permissionRequest), ct)); }
    /// <summary>Lists weekday patterns used to render an employee's recurring schedule.</summary>
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkPatternFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern list request."); return Ok(await Mediator.Send(new GetEmployeeWorkPatternsQuery(filter), ct)); }
    /// <summary>Updates a weekday work pattern without changing the parent arrangement.</summary>
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkPatternRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkPatternCommand(dto), ct)); }
    /// <summary>Activates or deactivates a weekday pattern without deleting schedule history.</summary>
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkPatternStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkPatternStatusCommand(dto), ct)); }
    /// <summary>Soft-deletes a weekday pattern that should no longer be applied.</summary>
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkPattern delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkPatternCommand(id, permissionRequest), ct)); }
    #endregion
}

/// <summary>Exposes employee temporary work-mode override endpoints.</summary>
[Route("api/[controller]")]
public sealed class EmployeeWorkModeOverrideController(IMediator mediator, ILoggerService logger) : TenantConfigurationControllerBase(mediator, logger)
{
    #region Employee Work Mode Override CRUD
    /// <summary>Creates a temporary employee Office, Remote, Hybrid, or WFH deviation for a defined date range.</summary>
    [HttpPost("create")] public async Task<IActionResult> Create([FromBody] CreateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride create request."); return Ok(await Mediator.Send(new CreateEmployeeWorkModeOverrideCommand(dto), ct)); }
    /// <summary>Reads one temporary work-mode override together with its server-owned approval state.</summary>
    [HttpGet("get-by-id/{id:long}")] public async Task<IActionResult> GetById(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride get-by-id request."); return Ok(await Mediator.Send(new GetEmployeeWorkModeOverrideByIdQuery(id, permissionRequest), ct)); }
    /// <summary>Lists temporary work-mode requests for employee and manager workflow screens.</summary>
    [HttpGet("get-all")] public async Task<IActionResult> GetAll([FromQuery] EmployeeWorkModeOverrideFilterRequestDTO filter, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride list request."); return Ok(await Mediator.Send(new GetEmployeeWorkModeOverridesQuery(filter), ct)); }
    /// <summary>Updates a temporary work-mode request without exposing approval fields to the caller.</summary>
    [HttpPost("update")] public async Task<IActionResult> Update([FromBody] UpdateEmployeeWorkModeOverrideRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride update request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkModeOverrideCommand(dto), ct)); }
    /// <summary>Activates or deactivates an override without deleting its audit history.</summary>
    [HttpPost("update-status")] public async Task<IActionResult> UpdateStatus([FromBody] UpdateEmployeeWorkModeOverrideStatusRequestDTO dto, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride status request."); return Ok(await Mediator.Send(new UpdateEmployeeWorkModeOverrideStatusCommand(dto), ct)); }
    /// <summary>Soft-deletes a temporary work-mode request that is no longer applicable.</summary>
    [HttpDelete("delete/{id:long}")] public async Task<IActionResult> Delete(long id, [FromQuery] PermissionRequestDTO permissionRequest, CancellationToken ct) { Logger.LogInfo("Received EmployeeWorkModeOverride delete request."); return Ok(await Mediator.Send(new DeleteEmployeeWorkModeOverrideCommand(id, permissionRequest), ct)); }
    #endregion
}
