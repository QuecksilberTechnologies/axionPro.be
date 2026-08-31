// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Exposes authenticated Host and Tenant endpoints for Tenant device connection configuration.
// ================================================================

using axionpro.application.DTOS.Host;
using axionpro.application.Features.HostDeviceCmd.Handlers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace axionpro.api.Controllers.HostDevice;

/// <summary>Provides authenticated Host and Tenant endpoints for separate Tenant device connection configuration.</summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class TenantDeviceConfigurationController(IMediator mediator, ILogger<TenantDeviceConfigurationController> logger) : ControllerBase
{
    /// <summary>
    /// Used-In-Angular: creates tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
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
    /// <para>Angular function(s): TenantDeviceConfigurationApi.getTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:45).</para>
    /// <para>Angular purpose: retrieves tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations/new; /app/tenant-device-configurations/:tenantDeviceConfigurationId/edit</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationForm (app/features/host/tenant-device-configurations/tenant-device-configuration-form/tenant-device-configuration-form.ts)</para>
    /// </remarks>
    [HttpGet("get-by-id/{id:long}")]
    public async Task<IActionResult> GetById(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration get-by-id request for {TenantDeviceConfigurationId}.", id);
        return Ok(await mediator.Send(new GetTenantDeviceConfigurationByIdQuery(id, accessRequest), cancellationToken));
    }

    /// <summary>
    /// Used-In-Angular: retrieves tenant device configurations.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
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
    /// Used-In-Angular: deletes tenant device configuration.
    /// </summary>
    /// <remarks>
    /// <para>Angular usage status: Used-In-Angular.</para>
    /// <para>Angular function(s): TenantDeviceConfigurationApi.deleteTenantDeviceConfiguration (app/core/services/tenant-device-configuration-api.ts:77).</para>
    /// <para>Angular purpose: deletes tenant device configuration.</para>
    /// <para>Integrated UI page(s): /app/tenant-device-configurations</para>
    /// <para>Angular UI component(s): TenantDeviceConfigurationsStore (app/features/host/tenant-device-configurations/tenant-device-configurations.store.ts); TenantDeviceConfigurations (app/features/host/tenant-device-configurations/tenant-device-configurations.ts)</para>
    /// </remarks>
    [HttpDelete("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] TenantDeviceAccessRequestDTO accessRequest, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received TenantDeviceConfiguration delete request for {TenantDeviceConfigurationId}.", id);
        return Ok(await mediator.Send(new DeleteTenantDeviceConfigurationCommand(id, accessRequest), cancellationToken));
    }
}
