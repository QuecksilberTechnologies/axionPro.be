using System.Text.Json;
using axionpro.application.Constants;
using axionpro.application.DTOS.Device.Enroll;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.application.Interfaces.IRepositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Submits employee enrollment work to the durable MQTT command pipeline.
/// Device serial numbers supplied by older callers are intentionally ignored;
/// the pipeline resolves the authoritative serial from TenantDeviceId.
/// </summary>
public sealed class EmployeeEnrollmentAtDeviceRepository(
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<EmployeeEnrollmentAtDeviceRepository> logger)
    : IEmployeeEnrollmentAtDeviceRepository
{
    /// <inheritdoc />
    public async Task<bool> EnrollEmployeeAsync(RegisterEmployeeDTORequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var payload = JsonSerializer.Serialize(new
        {
            cmd = DeviceCommands.SetUserInfo,
            enrollid = dto.EmployeeId,
            name = dto.Name,
            backupnum = 0,
            admin = 0,
            record = string.Empty
        });

        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                dto.TenantId,
                dto.DeviceId,
                DeviceCommands.SetUserInfo,
                payload,
                RequestedById: null));

        logger.LogInformation(
            "Queued employee enrollment command {DeviceCommandId} for TenantDevice {TenantDeviceId}.",
            result.DeviceCommandId,
            dto.DeviceId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteEmployeeAsync(DeleteEmployeeDTORequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var payload = JsonSerializer.Serialize(new
        {
            cmd = DeviceCommands.DeleteUser,
            enrollid = dto.EmployeeCode
        });

        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                dto.TenantId,
                dto.DeviceId,
                DeviceCommands.DeleteUser,
                payload,
                RequestedById: null));

        logger.LogInformation(
            "Queued employee delete command {DeviceCommandId} for TenantDevice {TenantDeviceId}.",
            result.DeviceCommandId,
            dto.DeviceId);
        return true;
    }
}
