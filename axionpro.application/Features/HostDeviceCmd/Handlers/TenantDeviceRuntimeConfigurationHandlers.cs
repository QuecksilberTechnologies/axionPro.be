// ================================================================
// Purpose : Tenant-admin-only strongly typed device runtime configuration.
//           Raw setdevinfo JSON is deliberately not accepted from the client.
// ================================================================

using System.Text.Json;
using axionpro.application.Constants;
using axionpro.application.DTOS.Host;
using axionpro.application.Exceptions;
using axionpro.application.Features.TenantConfigurationCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.HostDeviceCmd.Handlers;

/// <summary>Queues a safe, typed runtime configuration update for one Tenant device.</summary>
public sealed class ApplyTenantDeviceRuntimeConfigurationCommand(ApplyTenantDeviceRuntimeConfigurationRequestDTO dto)
    : IRequest<ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>>
{
    public ApplyTenantDeviceRuntimeConfigurationRequestDTO DTO { get; } = dto;
}

/// <summary>Queues a reboot for a Tenant device through its outbound HTTPS connection.</summary>
public sealed class RebootTenantDeviceCommand(RebootTenantDeviceRequestDTO dto)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public RebootTenantDeviceRequestDTO DTO { get; } = dto;
}

/// <summary>
/// Enforces Tenant scope and produces only vendor fields confirmed by the
/// device's getdevinfo response. Secrets are encrypted before persistence.
/// </summary>
public sealed class ApplyTenantDeviceRuntimeConfigurationCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<ApplyTenantDeviceRuntimeConfigurationCommand, ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>> Handle(
        ApplyTenantDeviceRuntimeConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        Validate(dto);
        var scope = await ResolveTenantConfigurationScopeAsync(dto, cancellationToken);

        var configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                dto.TenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        EnsureHttpsConfiguration(configuration);

        var deviceMaster = await UnitOfWork.DeviceMasterRepository.GetByIdAsync(
            configuration.TenantDevice.DeviceMasterId,
            cancellationToken);
        if (deviceMaster is null || !deviceMaster.IsActive || deviceMaster.IsSoftDeleted || !deviceMaster.SupportsHttps ||
            !configuration.TenantDevice.IsActive || configuration.TenantDevice.IsSoftDeleted)
        {
            throw new ValidationErrorException("The Tenant device must be active and support HTTPS before runtime settings can be queued.");
        }

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            string? initialNormalGatewayUrl = null;
            if (string.IsNullOrWhiteSpace(configuration.HttpsIngressTokenHash))
            {
                var ingressToken = DeviceHttpsGatewaySecurity.GenerateIngressToken();
                configuration.HttpsIngressTokenHash = DeviceHttpsGatewaySecurity.HashIngressToken(ingressToken);
                initialNormalGatewayUrl = $"{configuration.ServerUrl!.TrimEnd('/')}{DeviceHttpsGatewaySecurity.RoutePrefix}/{ingressToken}";
            }

            configuration.HeartbeatIntervalSeconds = dto.HeartbeatIntervalSeconds;
            configuration.Configuration = JsonSerializer.Serialize(new
            {
                schemaVersion = 1,
                heartbeatIntervalSeconds = dto.HeartbeatIntervalSeconds,
                volume = dto.Volume,
                localWebServerEnabled = false,
                managedBy = "TenantAdmin"
            });
            configuration.UpdatedById = scope.ActorId;
            configuration.UpdatedDateTime = DateTime.UtcNow;
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            var payload = BuildSetDeviceInfoPayload(dto, initialNormalGatewayUrl);
            var configurationCommand = await deviceCommandSubmissionService.SubmitAsync(
                new DeviceCommandSubmission(
                    scope.TenantId,
                    dto.TenantDeviceId,
                    DeviceCommands.SetDeviceInfo,
                    payload,
                    scope.ActorId,
                    ProtectPayload: true),
                cancellationToken);

            DeviceCommandSubmissionResult? rebootCommand = null;
            if (dto.RebootAfterApply)
            {
                rebootCommand = await deviceCommandSubmissionService.SubmitAsync(
                    new DeviceCommandSubmission(
                        scope.TenantId,
                        dto.TenantDeviceId,
                        DeviceCommands.Reboot,
                        JsonSerializer.Serialize(new { cmd = DeviceCommands.Reboot }),
                        scope.ActorId),
                    cancellationToken);
            }

            await UnitOfWork.CommitTransactionAsync(cancellationToken);
            return ApiResponse<TenantDeviceRuntimeConfigurationResponseDTO>.Success(
                new TenantDeviceRuntimeConfigurationResponseDTO
                {
                    ConfigurationCommandId = configurationCommand.DeviceCommandId,
                    ConfigurationTrackingId = configurationCommand.InternalTrackingId,
                    RebootCommandId = rebootCommand?.DeviceCommandId,
                    RebootTrackingId = rebootCommand?.InternalTrackingId,
                    Status = configurationCommand.Status.ToString()
                },
                "Tenant device configuration has been queued securely.");
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static void Validate(ApplyTenantDeviceRuntimeConfigurationRequestDTO dto)
    {
        if (dto.TenantDeviceId <= 0 ||
            string.IsNullOrWhiteSpace(dto.CurrentWebServerPassword) ||
            dto.CurrentWebServerPassword.Length is < 4 or > 128 ||
            dto.HeartbeatIntervalSeconds is < 10 or > 3600 ||
            dto.Volume is < 0 or > 15 ||
            !dto.DisableLocalWebServer ||
            (!string.IsNullOrWhiteSpace(dto.NewWebServerPassword) && dto.NewWebServerPassword.Length is < 8 or > 128))
        {
            throw new ValidationErrorException(
                "A valid device password, 10–3600 second heartbeat, volume 0–15, and local WebServer disable policy are required.");
        }
    }

    private static void EnsureHttpsConfiguration(TenantDeviceConfiguration configuration)
    {
        var transport = configuration.CommandTransport ?? configuration.MqttTransport;
        if (transport != (short)DeviceCommunicationProtocol.Https ||
            !Uri.TryCreate(configuration.ServerUrl, UriKind.Absolute, out var serverUri) ||
            !string.Equals(serverUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
            !DeviceHttpsGatewaySecurity.IsValidGatewayPath(configuration.ServerPath) ||
            configuration.ServerPort is not null and not 443)
        {
            throw new ValidationErrorException(
                "Configure this Tenant device for HTTPS, port 443, and the /device-gateway server path before applying runtime settings.");
        }
    }

    private static string BuildSetDeviceInfoPayload(
        ApplyTenantDeviceRuntimeConfigurationRequestDTO dto,
        string? initialNormalGatewayUrl)
    {
        var payload = new Dictionary<string, object?>
        {
            ["cmd"] = DeviceCommands.SetDeviceInfo,
            ["password"] = dto.CurrentWebServerPassword,
            ["nowtime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ["server_response_time"] = dto.HeartbeatIntervalSeconds,
            ["use_webserver"] = 0
        };

        if (dto.Volume.HasValue)
        {
            payload["volume"] = dto.Volume.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.NewWebServerPassword))
        {
            payload["webserver_pwd"] = dto.NewWebServerPassword;
        }

        if (!string.IsNullOrWhiteSpace(initialNormalGatewayUrl))
        {
            payload["use_bs"] = 1;
            payload["use_domain_name"] = 1;
            payload["bs_domain_name"] = initialNormalGatewayUrl;
            payload["serverport"] = 443;
        }

        return JsonSerializer.Serialize(payload);
    }
}

/// <summary>Queues a tenant-authorized reboot without exposing the generic command endpoint.</summary>
public sealed class RebootTenantDeviceCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<RebootTenantDeviceCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    /// <inheritdoc />
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        RebootTenantDeviceCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (dto.TenantDeviceId <= 0)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        var scope = await ResolveTenantConfigurationScopeAsync(dto, cancellationToken);
        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                scope.TenantId,
                dto.TenantDeviceId,
                DeviceCommands.Reboot,
                JsonSerializer.Serialize(new { cmd = DeviceCommands.Reboot }),
                scope.ActorId),
            cancellationToken);

        return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
            new DeviceCommandSubmissionResponseDTO
            {
                DeviceCommandId = result.DeviceCommandId,
                InternalTrackingId = result.InternalTrackingId,
                DeviceSerialNumber = result.DeviceSerialNumber,
                Status = result.Status.ToString()
            },
            "Tenant device reboot has been queued securely.");
    }
}
