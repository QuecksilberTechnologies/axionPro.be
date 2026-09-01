// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Manages Host-controlled physical Tenant device registration, configuration, and lifecycle.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides authenticated Host and Tenant endpoints for physical Tenant device administration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceController(IMediator mediator, ILogger<TenantDeviceController> logger) : ControllerBase
{
    #region Tenant Device Endpoints

    /// <summary>
    /// Used-In-Angular: creates tenant device.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: creates tenant device.</para>
    /// <para>Handler flow: CreateTenantDeviceCommand is processed by CreateTenantDeviceCommandHandler; operation(s): GetForUpdateAsync, AddAsync, SaveChangesAsync, GetByIdAsync.</para>
    /// <para>Response DTO property analysis: CreateTenantDeviceRequestDTO: No public properties were statically resolved.; ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceResponseDTO: TenantId (string), TenantLocationId (long), DeviceMasterId (long), DeviceCode (string), DeviceName (string?), InstalledDateTime (DateTime?), InstalledBy (long?), InstallationRemark (string?), IsAttendanceDevice (bool), Description (string?), Remark (string?), IsActive (bool)</para>
    /// <para>Angular function(s): TenantDeviceApi.addTenantDevice (app/core/services/tenant-device-api.ts:43).</para>
    /// <para>Angular purpose: creates tenant device.</para>
    /// <para>Integrated UI page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceForm (app/features/host/tenant-devices/tenant-device-form/tenant-device-form.ts)</para>
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice create request for Tenant {TenantId}.", dto.TenantId); return Ok(await mediator.Send(new CreateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Used-In-Angular: retrieves tenant device.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves tenant device by id.</para>
    /// <para>Handler flow: GetTenantDeviceByIdQuery is processed by GetTenantDeviceByIdQueryHandler; operation(s): GetByIdAsync.</para>
    /// <para>Response DTO property analysis: TenantDeviceAccessRequestDTO: TenantId (string?); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceResponseDTO: TenantId (string), TenantLocationId (long), DeviceMasterId (long), DeviceCode (string), DeviceName (string?), InstalledDateTime (DateTime?), InstalledBy (long?), InstallationRemark (string?), IsAttendanceDevice (bool), Description (string?), Remark (string?), IsActive (bool)</para>
    /// <para>Angular function(s): TenantDeviceApi.getTenantDevice (app/core/services/tenant-device-api.ts:37).</para>
    /// <para>Angular purpose: retrieves tenant device.</para>
    /// <para>Integrated UI page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceForm (app/features/host/tenant-devices/tenant-device-form/tenant-device-form.ts)</para>
    /// </remarks>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice get-by-id request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new GetTenantDeviceByIdQuery(id, accessRequest), cancellationToken)); }

    /// <summary>
    /// Used-In-Angular: retrieves tenant devices.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves all tenant devices.</para>
    /// <para>Handler flow: GetAllTenantDevicesQuery is processed by GetAllTenantDevicesQueryHandler; operation(s): GetPagedAsync, GetHostPagedAsync.</para>
    /// <para>Response DTO property analysis: GetTenantDeviceListRequestDTO: Search (string?), TenantLocationId (long?), DeviceMasterId (long?), IsAttendanceDevice (bool?), IsActive (bool?), PageNumber (int), PageSize (int); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceResponseDTO: TenantId (string), TenantLocationId (long), DeviceMasterId (long), DeviceCode (string), DeviceName (string?), InstalledDateTime (DateTime?), InstalledBy (long?), InstallationRemark (string?), IsAttendanceDevice (bool), Description (string?), Remark (string?), IsActive (bool)</para>
    /// <para>Angular function(s): TenantDeviceApi.getTenantDevices (app/core/services/tenant-device-api.ts:28).</para>
    /// <para>Angular purpose: retrieves tenant devices.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations/new; /app/tenant-device-configurations/:tenantDeviceConfigurationId/edit; /app/tenant-device-configurations; /app/tenant-devices; /app/profile/device-enrollment-info</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationForm (app/features/host/tenant-device-configurations/tenant-device-configuration-form/tenant-device-configuration-form.ts); TenantDeviceConfigurationsStore (app/features/host/tenant-device-configurations/tenant-device-configurations.store.ts); TenantDevicesStore (app/features/host/tenant-devices/tenant-devices.store.ts); EmployeeDeviceEnrollmentForm (app/features/user-menu/employee-profile/employee-device-enrollment-info/employee-device-enrollment-form/employee-device-enrollment-form.ts); TenantDeviceConfigurations (app/features/host/tenant-device-configurations/tenant-device-configurations.ts); TenantDevices (app/features/host/tenant-devices/tenant-devices.ts); EmployeeDeviceEnrollmentInfo (app/features/user-menu/employee-profile/employee-device-enrollment-info/employee-device-enrollment-info.ts)</para>
    /// </remarks>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceListRequestDTO filter, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice list request."); return Ok(await mediator.Send(new GetAllTenantDevicesQuery(filter), cancellationToken)); }

    /// <summary>
    /// Used-In-Angular: updates tenant device.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: updates tenant device.</para>
    /// <para>Handler flow: UpdateTenantDeviceCommand is processed by UpdateTenantDeviceCommandHandler; operation(s): GetForUpdateAsync, Map, SaveChangesAsync, GetByIdAsync.</para>
    /// <para>Response DTO property analysis: UpdateTenantDeviceRequestDTO: Id (long); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceResponseDTO: TenantId (string), TenantLocationId (long), DeviceMasterId (long), DeviceCode (string), DeviceName (string?), InstalledDateTime (DateTime?), InstalledBy (long?), InstallationRemark (string?), IsAttendanceDevice (bool), Description (string?), Remark (string?), IsActive (bool)</para>
    /// <para>Angular function(s): TenantDeviceApi.updateTenantDevice (app/core/services/tenant-device-api.ts:50).</para>
    /// <para>Angular purpose: updates tenant device.</para>
    /// <para>Integrated UI page(s): /app/tenant-devices/new; /app/tenant-devices/:tenantDeviceId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceForm (app/features/host/tenant-devices/tenant-device-form/tenant-device-form.ts)</para>
    /// </remarks>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice update request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceCommand(dto), cancellationToken)); }

    /// <summary>
    /// Used-In-Angular: updates tenant device status.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: updates tenant device status.</para>
    /// <para>Handler flow: UpdateTenantDeviceStatusCommand is processed by UpdateTenantDeviceStatusCommandHandler; operation(s): GetForUpdateAsync, SaveChangesAsync, GetByIdAsync.</para>
    /// <para>Response DTO property analysis: UpdateTenantDeviceStatusRequestDTO: Id (long), IsActive (bool); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceResponseDTO: TenantId (string), TenantLocationId (long), DeviceMasterId (long), DeviceCode (string), DeviceName (string?), InstalledDateTime (DateTime?), InstalledBy (long?), InstallationRemark (string?), IsAttendanceDevice (bool), Description (string?), Remark (string?), IsActive (bool)</para>
    /// <para>Angular function(s): TenantDeviceApi.setTenantDeviceStatus (app/core/services/tenant-device-api.ts:56).</para>
    /// <para>Angular purpose: updates tenant device status.</para>
    /// <para>Integrated UI page(s): /app/tenant-devices</para>
    /// <para>Angular UI component(s): TenantDevicesStore (app/features/host/tenant-devices/tenant-devices.store.ts); TenantDevices (app/features/host/tenant-devices/tenant-devices.ts)</para>
    /// </remarks>
    [HttpPost("update-status")]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTenantDeviceStatusRequestDTO dto, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice status request for {TenantDeviceId}.", dto.Id); return Ok(await mediator.Send(new UpdateTenantDeviceStatusCommand(dto), cancellationToken)); }

    /// <summary>
    /// Used-In-Angular: deletes tenant device.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: deletes tenant device.</para>
    /// <para>Handler flow: DeleteTenantDeviceCommand is processed by DeleteTenantDeviceCommandHandler; operation(s): GetForUpdateAsync, SaveChangesAsync.</para>
    /// <para>Response DTO property analysis: TenantDeviceAccessRequestDTO: TenantId (string?); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
    /// <para>Angular function(s): TenantDeviceApi.deleteTenantDevice (app/core/services/tenant-device-api.ts:64).</para>
    /// <para>Angular purpose: deletes tenant device.</para>
    /// <para>Integrated UI page(s): /app/tenant-devices</para>
    /// <para>Angular UI component(s): TenantDevicesStore (app/features/host/tenant-devices/tenant-devices.store.ts); TenantDevices (app/features/host/tenant-devices/tenant-devices.ts)</para>
    /// </remarks>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    { logger.LogInformation("Received TenantDevice delete request for {TenantDeviceId}.", id); return Ok(await mediator.Send(new DeleteTenantDeviceCommand(id, accessRequest), cancellationToken)); }

    #endregion
}
