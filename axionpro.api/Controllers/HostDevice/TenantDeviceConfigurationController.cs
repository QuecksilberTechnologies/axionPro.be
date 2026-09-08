// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes Host bootstrap plus Host-authorized and Tenant-admin device connection configuration endpoints.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides Host initial provisioning plus Host-authorized and Tenant-admin connection configuration endpoints.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceConfigurationController(IMediator mediator, ILogger<TenantDeviceConfigurationController> logger) : ControllerBase
{
    #region Initial Device Provisioning and Runtime Configuration

    /// <summary>
    /// Issues a short-lived initial HTTPS gateway URL so a technician can connect a new unassigned physical device without exposing a permanent credential.
    /// </summary>
    /// <remarks>
    /// Host provisioning permission is enforced by the HostDevice permission behavior.
    /// The returned URL is shown only once and must be entered on the physical device.
    /// </remarks>
    [HttpPost("issue-bootstrap-url")]
    public async Task<IActionResult> IssueBootstrapUrl(
        [FromBody] IssueInitialDeviceBootstrapRequestDTO dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received initial device bootstrap request for DeviceMaster {DeviceMasterId}.", dto.DeviceMasterId);
        return Ok(await mediator.Send(new IssueInitialDeviceBootstrapCommand(dto), cancellationToken));
    }

    /// <summary>
    /// Queues the approved Host-authorized or Tenant-admin runtime settings so the device is configured through its outbound transport instead of a direct LAN call.
    /// </summary>
    [HttpPost("apply-runtime-configuration")]
    public async Task<IActionResult> ApplyRuntimeConfiguration(
        [FromBody] ApplyTenantDeviceRuntimeConfigurationRequestDTO dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received runtime configuration request for TenantDevice {TenantDeviceId}.", dto.TenantDeviceId);
        return Ok(await mediator.Send(new ApplyTenantDeviceRuntimeConfigurationCommand(dto), cancellationToken));
    }

    /// <summary>
    /// Queues a Host-authorized or Tenant-admin device reboot so restart is auditable and works for both configured transports.
    /// </summary>
    [HttpPost("reboot")]
    public async Task<IActionResult> Reboot(
        [FromBody] RebootTenantDeviceRequestDTO dto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Received reboot request for TenantDevice {TenantDeviceId}.", dto.TenantDeviceId);
        return Ok(await mediator.Send(new RebootTenantDeviceCommand(dto), cancellationToken));
    }

    /// <summary>Queues one typed device time-settings change so the Time UI updates only this firmware configuration section.</summary>
    [HttpPost("settings/time")]
    public Task<IActionResult> UpdateTimeSettings([FromBody] UpdateTenantDeviceTimeSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Time, cancellationToken);

    /// <summary>Queues an explicit device-clock synchronization when an admin needs the physical device clock corrected without changing time-format settings.</summary>
    [HttpPost("settings/time/sync")]
    public async Task<IActionResult> SyncTime([FromBody] SyncTenantDeviceTimeRequestDTO dto, CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new SyncTenantDeviceTimeCommand(dto), cancellationToken));

    /// <summary>Queues typed bell settings so the Bell UI can update output behavior without sending raw vendor JSON.</summary>
    [HttpPost("settings/bell")]
    public Task<IActionResult> UpdateBellSettings([FromBody] UpdateTenantDeviceBellSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Bell, cancellationToken);

    /// <summary>Queues typed device display and recognition settings so the Device setup UI uses readable business properties.</summary>
    [HttpPost("settings/device-setup")]
    public Task<IActionResult> UpdateDeviceSetupSettings([FromBody] UpdateTenantDeviceSetupSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.DeviceSetup, cancellationToken);

    /// <summary>Queues typed advanced face, verification, privacy, and fill-light settings so these sensitive controls remain permission-checked.</summary>
    [HttpPost("settings/advanced")]
    public Task<IActionResult> UpdateAdvancedSettings([FromBody] UpdateTenantDeviceAdvancedSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Advanced, cancellationToken);

    /// <summary>Queues typed door, Wiegand, and access-control settings so a lock configuration change is logged and transport-neutral.</summary>
    [HttpPost("settings/lock")]
    public Task<IActionResult> UpdateLockSettings([FromBody] UpdateTenantDeviceLockSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Lock, cancellationToken);

    /// <summary>Queues typed serial port settings so the Serial UI uses validated device options.</summary>
    [HttpPost("settings/serial")]
    public Task<IActionResult> UpdateSerialSettings([FromBody] UpdateTenantDeviceSerialSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Serial, cancellationToken);

    /// <summary>Queues typed Ethernet settings so network changes are delivered safely through the configured outbound transport; static addressing applies only when DHCP is disabled.</summary>
    [HttpPost("settings/ethernet")]
    public Task<IActionResult> UpdateEthernetSettings([FromBody] UpdateTenantDeviceEthernetSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Ethernet, cancellationToken);

    /// <summary>Queues typed Wi-Fi network settings so the tenant can manage connectivity without a direct browser call to the device; static addressing applies only when DHCP is disabled.</summary>
    [HttpPost("settings/wifi")]
    public Task<IActionResult> UpdateWifiSettings([FromBody] UpdateTenantDeviceWifiSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.Wifi, cancellationToken);

    /// <summary>Queues typed optional app-notification settings so third-party notification details stay protected inside the command queue.</summary>
    [HttpPost("settings/app-notification")]
    public Task<IActionResult> UpdateAppNotificationSettings([FromBody] UpdateTenantDeviceAppNotificationSettingsRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.AppNotification, cancellationToken);

    /// <summary>Queues local device Web UI/API enablement and password rotation so the Tenant can lock or restore local access without exposing stored credentials.</summary>
    [HttpPost("settings/web-access")]
    public Task<IActionResult> UpdateWebAccess([FromBody] UpdateTenantDeviceWebAccessRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.WebAccess, cancellationToken);

    /// <summary>Queues the PIN required by the device physical System/Local Manager menu so unauthorized people cannot change settings at the device screen.</summary>
    [HttpPost("settings/screen-menu-pin")]
    public Task<IActionResult> UpdateScreenMenuPin([FromBody] UpdateTenantDeviceScreenMenuPinRequestDTO dto, CancellationToken cancellationToken) =>
        SendSettingsAsync(dto, TenantDeviceSettingsSection.ScreenMenuPin, cancellationToken);

    /// <summary>Gets the device cloud gateway base address and replacement state so Angular can show connection health without returning a bearer token.</summary>
    [HttpGet("gateway-address/{tenantDeviceId}")]
    public async Task<IActionResult> GetGatewayAddress(
        string tenantDeviceId,
        [FromQuery] TenantDeviceAccessRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new GetTenantDeviceGatewayAddressQuery(tenantDeviceId, dto), cancellationToken));

    /// <summary>
    /// Replaces the device HTTPS gateway remotely so the Tenant can rotate the connection address without visiting the device. The current URL remains usable
    /// until the device polls through the new URL; heartbeat is not changed.
    /// </summary>
    [HttpPost("replace-https-gateway-url")]
    public async Task<IActionResult> ReplaceHttpsGatewayUrl(
        [FromBody] ReplaceTenantDeviceHttpsGatewayUrlRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new ReplaceTenantDeviceHttpsGatewayUrlCommand(dto), cancellationToken));

    /// <summary>
    /// Publishes the next command already in the MQTTS queue now when a Tenant explicitly needs immediate broker delivery. HTTPS devices
    /// deliberately use their next outbound heartbeat instead.
    /// </summary>
    [HttpPost("dispatch-mqtts-now")]
    public async Task<IActionResult> DispatchMqttsNow(
        [FromBody] DispatchTenantDeviceMqttsNowRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new DispatchTenantDeviceMqttsNowCommand(dto), cancellationToken));

    private async Task<IActionResult> SendSettingsAsync(
        TenantDeviceSettingRequestDTO dto,
        TenantDeviceSettingsSection section,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new ApplyTenantDeviceSettingsCommand(dto, section), cancellationToken));

    #endregion

    #region Tenant Device Configuration CRUD

    /// <summary>
    /// Used-In-Angular: creates tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: creates tenant device configuration.</para>
    /// <para>Handler flow: CreateTenantDeviceConfigurationCommand is processed by CreateTenantDeviceConfigurationCommandHandler; operation(s): AddAsync, SaveChangesAsync, GetByIdAsync.</para>
    /// <para>Response DTO property analysis: CreateTenantDeviceConfigurationRequestDTO: No public properties were statically resolved.; ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceConfigurationResponseDTO: TenantId (string), Id (long), TenantDeviceId (long), IpAddress (string?), MacAddress (string?), DevicePort (int?), CommunicationType (DeviceCommunicationType?), ServerHost (string?), ServerPort (int?), ServerPath (string?), ServerUrl (string?), PushMode (string?), HeartbeatIntervalSeconds (int?), TimeZoneId (string?), Configuration (string?), IsEnrollmentEnabled (bool), IsAttendancePushEnabled (bool), IsAutoSyncEnabled (bool), LastHeartbeatDateTime (DateTime?), LastSyncDateTime (DateTime?), LastAttendanceReceivedDateTime (DateTime?), LastSuccessfulConnectionDateTime (DateTime?), LastFailedConnectionDateTime (DateTime?), LastConnectionError (string?), DeviceCode (string?), DeviceName (string?), DeviceMasterName (string?), DeviceMasterSNo (string?), AddedDateTime (DateTime), UpdatedDateTime (DateTime?)</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.addTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:57).</para>
    /// <para>Angular purpose: creates tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations/new; /app/tenant-device-configurations/:tenantDeviceConfigurationId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationForm (app/features/host/tenant-device-configurations/tenant-device-configuration-form/tenant-device-configuration-form.ts)</para>
    /// </remarks>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDeviceConfigurationRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration create request for TenantDevice {TenantDeviceId}.", dto.TenantDeviceId);
        return Ok(await mediator.Send(new CreateTenantDeviceConfigurationCommand(dto), cancellationToken));
    }

    /// <summary>
    /// Used-In-Angular: retrieves tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves tenant device configuration by id.</para>
    /// <para>Handler flow: GetTenantDeviceConfigurationByIdQuery is processed by GetTenantDeviceConfigurationByIdQueryHandler; operation(s): GetByIdAsync.</para>
    /// <para>Response DTO property analysis: TenantDeviceAccessRequestDTO: TenantId (string?); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceConfigurationResponseDTO: TenantId (string), Id (long), TenantDeviceId (long), IpAddress (string?), MacAddress (string?), DevicePort (int?), CommunicationType (DeviceCommunicationType?), ServerHost (string?), ServerPort (int?), ServerPath (string?), ServerUrl (string?), PushMode (string?), HeartbeatIntervalSeconds (int?), TimeZoneId (string?), Configuration (string?), IsEnrollmentEnabled (bool), IsAttendancePushEnabled (bool), IsAutoSyncEnabled (bool), LastHeartbeatDateTime (DateTime?), LastSyncDateTime (DateTime?), LastAttendanceReceivedDateTime (DateTime?), LastSuccessfulConnectionDateTime (DateTime?), LastFailedConnectionDateTime (DateTime?), LastConnectionError (string?), DeviceCode (string?), DeviceName (string?), DeviceMasterName (string?), DeviceMasterSNo (string?), AddedDateTime (DateTime), UpdatedDateTime (DateTime?)</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.getTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:45).</para>
    /// <para>Angular purpose: retrieves tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations/new; /app/tenant-device-configurations/:tenantDeviceConfigurationId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationForm (app/features/host/tenant-device-configurations/tenant-device-configuration-form/tenant-device-configuration-form.ts)</para>
    /// </remarks>
    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration get-by-id request.");
        return Ok(await mediator.Send(new GetTenantDeviceConfigurationByIdQuery(id, accessRequest), cancellationToken));
    }

    /// <summary>
    /// Used-In-Angular: retrieves tenant device configurations.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: retrieves all tenant device configurations.</para>
    /// <para>Handler flow: GetAllTenantDeviceConfigurationsQuery is processed by GetAllTenantDeviceConfigurationsQueryHandler; operation(s): GetPagedAsync, GetHostPagedAsync.</para>
    /// <para>Response DTO property analysis: GetTenantDeviceConfigurationListRequestDTO: Search (string?), TenantDeviceId (long?), CommunicationType (DeviceCommunicationType?), IsEnrollmentEnabled (bool?), PageNumber (int), PageSize (int); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceConfigurationResponseDTO: TenantId (string), Id (long), TenantDeviceId (long), IpAddress (string?), MacAddress (string?), DevicePort (int?), CommunicationType (DeviceCommunicationType?), ServerHost (string?), ServerPort (int?), ServerPath (string?), ServerUrl (string?), PushMode (string?), HeartbeatIntervalSeconds (int?), TimeZoneId (string?), Configuration (string?), IsEnrollmentEnabled (bool), IsAttendancePushEnabled (bool), IsAutoSyncEnabled (bool), LastHeartbeatDateTime (DateTime?), LastSyncDateTime (DateTime?), LastAttendanceReceivedDateTime (DateTime?), LastSuccessfulConnectionDateTime (DateTime?), LastFailedConnectionDateTime (DateTime?), LastConnectionError (string?), DeviceCode (string?), DeviceName (string?), DeviceMasterName (string?), DeviceMasterSNo (string?), AddedDateTime (DateTime), UpdatedDateTime (DateTime?)</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.getTenantDeviceConfigurations (app/core/services/tenant-device-configuration-api.ts:29).</para>
    /// <para>Angular purpose: retrieves tenant device configurations.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationsStore (app/features/host/tenant-device-configurations/tenant-device-configurations.store.ts); TenantDeviceConfigurations (app/features/host/tenant-device-configurations/tenant-device-configurations.ts)</para>
    /// </remarks>
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetTenantDeviceConfigurationListRequestDTO filter, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration list request.");
        return Ok(await mediator.Send(new GetAllTenantDeviceConfigurationsQuery(filter), cancellationToken));
    }

    /// <summary>
    /// Used-In-Angular: updates tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: updates tenant device configuration.</para>
    /// <para>Handler flow: UpdateTenantDeviceConfigurationCommand is processed by UpdateTenantDeviceConfigurationCommandHandler; operation(s): GetForUpdateAsync, Map, SaveChangesAsync, GetByIdAsync.</para>
    /// <para>Response DTO property analysis: UpdateTenantDeviceConfigurationRequestDTO: Id (long); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?); TenantDeviceConfigurationResponseDTO: TenantId (string), Id (long), TenantDeviceId (long), IpAddress (string?), MacAddress (string?), DevicePort (int?), CommunicationType (DeviceCommunicationType?), ServerHost (string?), ServerPort (int?), ServerPath (string?), ServerUrl (string?), PushMode (string?), HeartbeatIntervalSeconds (int?), TimeZoneId (string?), Configuration (string?), IsEnrollmentEnabled (bool), IsAttendancePushEnabled (bool), IsAutoSyncEnabled (bool), LastHeartbeatDateTime (DateTime?), LastSyncDateTime (DateTime?), LastAttendanceReceivedDateTime (DateTime?), LastSuccessfulConnectionDateTime (DateTime?), LastFailedConnectionDateTime (DateTime?), LastConnectionError (string?), DeviceCode (string?), DeviceName (string?), DeviceMasterName (string?), DeviceMasterSNo (string?), AddedDateTime (DateTime), UpdatedDateTime (DateTime?)</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.updateTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:66).</para>
    /// <para>Angular purpose: updates tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations/new; /app/tenant-device-configurations/:tenantDeviceConfigurationId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationForm (app/features/host/tenant-device-configurations/tenant-device-configuration-form/tenant-device-configuration-form.ts)</para>
    /// </remarks>
    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTenantDeviceConfigurationRequestDTO dto, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration update request for {TenantDeviceConfigurationId}.", dto.Id);
        return Ok(await mediator.Send(new UpdateTenantDeviceConfigurationCommand(dto), cancellationToken));
    }

    /// <summary>
    /// Generates a one-time opaque HTTPS device gateway URL. The caller must copy
    /// it to the physical device immediately; later reads intentionally omit it.
    /// </summary>
    [HttpPost("rotate-https-ingress-token")]
    public async Task<IActionResult> RotateHttpsIngressToken(
        [FromBody] RotateTenantDeviceHttpsIngressTokenRequestDTO dto,
        CancellationToken cancellationToken) =>
        Ok(await mediator.Send(new RotateTenantDeviceHttpsIngressTokenCommand(dto), cancellationToken));

    /// <summary>
    /// Used-In-Angular: deletes tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>API endpoint purpose: deletes tenant device configuration.</para>
    /// <para>Handler flow: DeleteTenantDeviceConfigurationCommand is processed by DeleteTenantDeviceConfigurationCommandHandler; operation(s): GetForUpdateAsync, Remove, SaveChangesAsync.</para>
    /// <para>Response DTO property analysis: TenantDeviceAccessRequestDTO: TenantId (string?); ApiResponse: IsSucceeded (bool), Message (string), Data (T), Errors (List&lt;string&gt;), ErrorCode (string?), PageNumber (int?), PageSize (int?), TotalRecords (int?), TotalPages (int?), IsPrimaryMarked (bool?), HasAllDocUploaded (bool?), CompletionPercentage (double?)</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.deleteTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:77).</para>
    /// <para>Angular purpose: deletes tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationsStore (app/features/host/tenant-device-configurations/tenant-device-configurations.store.ts); TenantDeviceConfigurations (app/features/host/tenant-device-configurations/tenant-device-configurations.ts)</para>
    /// </remarks>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(string id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration delete request.");
        return Ok(await mediator.Send(new DeleteTenantDeviceConfigurationCommand(id, accessRequest), cancellationToken));
    }

    #endregion
}
