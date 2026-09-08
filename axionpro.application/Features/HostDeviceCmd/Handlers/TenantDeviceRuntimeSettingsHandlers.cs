// ================================================================
// Purpose : Queues typed Tenant device-settings commands. The physical device
//           receives them only from its configured outbound transport; this
//           feature never opens a connection to a device LAN address.
// ================================================================

using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using AutoMapper;
using axionpro.application.Common.Enums;
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

/// <summary>Queues exactly one typed device settings section.</summary>
public sealed class ApplyTenantDeviceSettingsCommand(
    TenantDeviceSettingRequestDTO dto,
    TenantDeviceSettingsSection section)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public TenantDeviceSettingRequestDTO DTO { get; } = dto;
    public TenantDeviceSettingsSection Section { get; } = section;
}

/// <summary>Sets the device UTC clock through the durable command queue.</summary>
public sealed class SyncTenantDeviceTimeCommand(SyncTenantDeviceTimeRequestDTO dto)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public SyncTenantDeviceTimeRequestDTO DTO { get; } = dto;
}

/// <summary>Changes only the assigned Tenant location; DeviceMaster assignment remains Host-owned.</summary>
public sealed class UpdateTenantDeviceLocationCommand(UpdateTenantDeviceLocationRequestDTO dto)
    : IRequest<ApiResponse<TenantDeviceResponseDTO>>
{
    public UpdateTenantDeviceLocationRequestDTO DTO { get; } = dto;
}

/// <summary>Gets non-secret gateway address metadata for the selected device.</summary>
public sealed class GetTenantDeviceGatewayAddressQuery(string encryptedTenantDeviceId, TenantDeviceAccessRequestDTO accessRequest)
    : IRequest<ApiResponse<TenantDeviceGatewayAddressResponseDTO>>
{
    public string EncryptedTenantDeviceId { get; } = encryptedTenantDeviceId;
    public TenantDeviceAccessRequestDTO AccessRequest { get; } = accessRequest;
}

/// <summary>Queues a two-phase replacement of the opaque HTTPS gateway URL.</summary>
public sealed class ReplaceTenantDeviceHttpsGatewayUrlCommand(ReplaceTenantDeviceHttpsGatewayUrlRequestDTO dto)
    : IRequest<ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public ReplaceTenantDeviceHttpsGatewayUrlRequestDTO DTO { get; } = dto;
}

/// <summary>Publishes the next queued command for an MQTTS device on explicit Tenant request.</summary>
public sealed class DispatchTenantDeviceMqttsNowCommand(DispatchTenantDeviceMqttsNowRequestDTO dto)
    : IRequest<ApiResponse<ManualTenantDeviceCommandDispatchResponseDTO>>
{
    public DispatchTenantDeviceMqttsNowRequestDTO DTO { get; } = dto;
}

/// <summary>Handles all typed <c>setdevinfo</c> section changes without a raw JSON escape hatch.</summary>
public sealed class ApplyTenantDeviceSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger,
    ILogger<ApplyTenantDeviceSettingsCommandHandler> logger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<ApplyTenantDeviceSettingsCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        ApplyTenantDeviceSettingsCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(dto, cancellationToken);
        TenantDeviceRuntimeSettingsValidation.ValidateCommon(dto);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");

        _ = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        var payload = TenantDeviceRuntimeSettingsPayloadBuilder.BuildSetDeviceInfo(dto, request.Section);
        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                scope.TenantId,
                tenantDeviceId,
                DeviceCommands.SetDeviceInfo,
                payload,
                scope.ActorId,
                ProtectPayload: true),
            cancellationToken);

        logger.LogInformation(
            "Queued Tenant device {Section} settings for TenantDevice {TenantDeviceId}. Command {DeviceCommandId}.",
            request.Section,
            tenantDeviceId,
            result.DeviceCommandId);

        return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
            ToSubmissionResponse(result),
            $"{TenantDeviceRuntimeSettingsPayloadBuilder.GetSectionLabel(request.Section)} settings have been queued for the device.");
    }

    private static DeviceCommandSubmissionResponseDTO ToSubmissionResponse(DeviceCommandSubmissionResult result) =>
        new()
        {
            DeviceCommandId = result.DeviceCommandId,
            InternalTrackingId = result.InternalTrackingId,
            Status = result.Status.ToString()
        };
}

/// <summary>Handles an explicit device clock synchronization.</summary>
public sealed class SyncTenantDeviceTimeCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger,
    ILogger<SyncTenantDeviceTimeCommandHandler> logger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<SyncTenantDeviceTimeCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        SyncTenantDeviceTimeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(dto, cancellationToken);
        TenantDeviceRuntimeSettingsValidation.ValidateCommon(dto);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");

        _ = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        var payload = TenantDeviceRuntimeSettingsPayloadBuilder.BuildSetTime(dto);
        var result = await deviceCommandSubmissionService.SubmitAsync(
            new DeviceCommandSubmission(
                scope.TenantId,
                tenantDeviceId,
                DeviceCommands.SetTime,
                payload,
                scope.ActorId,
                ProtectPayload: true),
            cancellationToken);

        logger.LogInformation("Queued device time synchronization for TenantDevice {TenantDeviceId}. Command {DeviceCommandId}.", tenantDeviceId, result.DeviceCommandId);
        return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
            new DeviceCommandSubmissionResponseDTO
            {
                DeviceCommandId = result.DeviceCommandId,
                InternalTrackingId = result.InternalTrackingId,
                Status = result.Status.ToString()
            },
            "Device time synchronization has been queued.");
    }
}

/// <summary>Handles Tenant-owned location changes without exposing the DeviceMaster assignment operation.</summary>
public sealed class UpdateTenantDeviceLocationCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger,
    ILogger<UpdateTenantDeviceLocationCommandHandler> logger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<UpdateTenantDeviceLocationCommand, ApiResponse<TenantDeviceResponseDTO>>
{
    public async Task<ApiResponse<TenantDeviceResponseDTO>> Handle(
        UpdateTenantDeviceLocationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(dto, cancellationToken);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");
        if (dto.TenantLocationId <= 0 ||
            !await UnitOfWork.TenantDeviceRepository.IsActiveTenantLocationAsync(dto.TenantLocationId, cancellationToken) ||
            !await UnitOfWork.TenantDeviceRepository.TenantLocationBelongsToTenantAsync(scope.TenantId, dto.TenantLocationId, cancellationToken))
        {
            throw new ValidationErrorException("Select an active location that belongs to this Tenant.");
        }

        var device = await UnitOfWork.TenantDeviceRepository.GetForUpdateAsync(scope.TenantId, tenantDeviceId, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        device.TenantLocationId = dto.TenantLocationId;
        device.UpdatedById = scope.ActorId;
        device.UpdatedDateTime = DateTime.UtcNow;
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var stored = await UnitOfWork.TenantDeviceRepository.GetByIdAsync(scope.TenantId, tenantDeviceId, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceNotFound);
        logger.LogInformation("Moved TenantDevice {TenantDeviceId} to TenantLocation {TenantLocationId}.", tenantDeviceId, dto.TenantLocationId);
        return ApiResponse<TenantDeviceResponseDTO>.Success(
            MapDeviceResponse(mapper, stored, scope),
            "Device location has been updated.");
    }
}

/// <summary>Returns gateway metadata without ever returning the bearer URL token.</summary>
public sealed class GetTenantDeviceGatewayAddressQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<GetTenantDeviceGatewayAddressQuery, ApiResponse<TenantDeviceGatewayAddressResponseDTO>>
{
    public async Task<ApiResponse<TenantDeviceGatewayAddressResponseDTO>> Handle(
        GetTenantDeviceGatewayAddressQuery request,
        CancellationToken cancellationToken)
    {
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(request.AccessRequest, cancellationToken);
        var tenantDeviceId = DecodeIdentifier(request.EncryptedTenantDeviceId, scope, "TenantDeviceId");
        var configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        return ApiResponse<TenantDeviceGatewayAddressResponseDTO>.Success(
            new TenantDeviceGatewayAddressResponseDTO
            {
                TenantDeviceId = EncryptIdentifier(tenantDeviceId, scope.TenantEncryptionKey),
                ServerUrl = configuration.ServerUrl ?? string.Empty,
                ServerPath = configuration.ServerPath ?? string.Empty,
                ServerPort = configuration.ServerPort ?? 443,
                HasActiveGatewayUrl = !string.IsNullOrWhiteSpace(configuration.HttpsIngressTokenHash),
                IsReplacementPending = !string.IsNullOrWhiteSpace(configuration.PendingHttpsIngressTokenHash) &&
                                       configuration.PendingHttpsIngressTokenExpiresDateTime > DateTime.UtcNow,
                ReplacementExpiresDateTime = configuration.PendingHttpsIngressTokenExpiresDateTime
            },
            "Device gateway address details retrieved successfully.");
    }
}

/// <summary>Queues a seamless HTTPS URL replacement through the still-valid current device gateway.</summary>
public sealed class ReplaceTenantDeviceHttpsGatewayUrlCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandSubmissionService deviceCommandSubmissionService,
    ILogger<TenantConfigurationHandlerBase> tenantLogger,
    ILogger<ReplaceTenantDeviceHttpsGatewayUrlCommandHandler> logger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<ReplaceTenantDeviceHttpsGatewayUrlCommand, ApiResponse<DeviceCommandSubmissionResponseDTO>>
{
    public async Task<ApiResponse<DeviceCommandSubmissionResponseDTO>> Handle(
        ReplaceTenantDeviceHttpsGatewayUrlCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(dto, cancellationToken);
        TenantDeviceRuntimeSettingsValidation.ValidateCommon(dto);
        if (dto.ReplacementLifetimeMinutes is < 5 or > 1_440)
        {
            throw new ValidationErrorException("Gateway replacement lifetime must be between 5 and 1,440 minutes.");
        }

        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");
        var configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);
        TenantDeviceConfigurationValidation.EnsureHttpsGateway(configuration);
        if (string.IsNullOrWhiteSpace(configuration.HttpsIngressTokenHash))
        {
            throw new ValidationErrorException("Generate the current HTTPS gateway URL before replacing it remotely.");
        }

        if (!string.IsNullOrWhiteSpace(configuration.PendingHttpsIngressTokenHash) &&
            configuration.PendingHttpsIngressTokenExpiresDateTime > DateTime.UtcNow)
        {
            throw new ConflictException("A device gateway URL replacement is already awaiting device confirmation.");
        }

        var rawPendingToken = DeviceHttpsGatewaySecurity.GenerateIngressToken();
        var pendingUrl = $"{configuration.ServerUrl!.TrimEnd('/')}{DeviceHttpsGatewaySecurity.RoutePrefix}/{rawPendingToken}";
        var expiresDateTime = DateTime.UtcNow.AddMinutes(dto.ReplacementLifetimeMinutes);

        await UnitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            configuration.PendingHttpsIngressTokenHash = DeviceHttpsGatewaySecurity.HashIngressToken(rawPendingToken);
            configuration.PendingHttpsIngressTokenExpiresDateTime = expiresDateTime;
            configuration.UpdatedById = scope.ActorId;
            configuration.UpdatedDateTime = DateTime.UtcNow;
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            var payload = TenantDeviceRuntimeSettingsPayloadBuilder.BuildGatewayReplacement(dto.CurrentWebServerPassword, pendingUrl);
            var result = await deviceCommandSubmissionService.SubmitAsync(
                new DeviceCommandSubmission(
                    scope.TenantId,
                    tenantDeviceId,
                    DeviceCommands.SetDeviceInfo,
                    payload,
                    scope.ActorId,
                    ProtectPayload: true),
                cancellationToken);

            await UnitOfWork.CommitTransactionAsync(cancellationToken);
            logger.LogInformation("Queued seamless HTTPS gateway replacement for TenantDevice {TenantDeviceId}. Command {DeviceCommandId}.", tenantDeviceId, result.DeviceCommandId);
            return ApiResponse<DeviceCommandSubmissionResponseDTO>.Success(
                new DeviceCommandSubmissionResponseDTO
                {
                    DeviceCommandId = result.DeviceCommandId,
                    InternalTrackingId = result.InternalTrackingId,
                    Status = result.Status.ToString()
                },
                "Device gateway URL replacement has been queued. The existing URL remains active until the device confirms the replacement.");
        }
        catch
        {
            await UnitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

/// <summary>Runs one existing MQTTS queue item now without accepting device JSON from Angular.</summary>
public sealed class DispatchTenantDeviceMqttsNowCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService,
    IDeviceCommandManualDispatcher manualDispatcher,
    ILogger<TenantConfigurationHandlerBase> tenantLogger,
    ILogger<DispatchTenantDeviceMqttsNowCommandHandler> logger)
    : TenantDeviceAccessHandlerBase(unitOfWork, commonRequestService, idEncoderService, tenantLogger),
        IRequestHandler<DispatchTenantDeviceMqttsNowCommand, ApiResponse<ManualTenantDeviceCommandDispatchResponseDTO>>
{
    public async Task<ApiResponse<ManualTenantDeviceCommandDispatchResponseDTO>> Handle(
        DispatchTenantDeviceMqttsNowCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var scope = await ResolveAuthorizedTenantConfigurationScopeAsync(dto, cancellationToken);
        var tenantDeviceId = DecodeIdentifier(dto.TenantDeviceId, scope, "TenantDeviceId");
        var configuration = await UnitOfWork.TenantDeviceConfigurationRepository.GetForUpdateByTenantDeviceAsync(
                scope.TenantId,
                tenantDeviceId,
                cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.TenantDeviceConfigurationNotFound);

        var transport = configuration.CommandTransport ?? configuration.MqttTransport;
        if (transport != (short)DeviceCommunicationProtocol.Mqtts)
        {
            throw new ValidationErrorException(
                "Manual dispatch is available only for MQTTS devices. HTTPS devices receive queued commands on their next heartbeat poll.");
        }

        var result = await manualDispatcher.DispatchNextMqttsAsync(tenantDeviceId, cancellationToken);
        logger.LogInformation(
            "Tenant requested manual MQTTS dispatch for TenantDevice {TenantDeviceId}. Dispatched: {WasDispatched}.",
            tenantDeviceId,
            result.WasDispatched);
        return ApiResponse<ManualTenantDeviceCommandDispatchResponseDTO>.Success(
            new ManualTenantDeviceCommandDispatchResponseDTO
            {
                WasDispatched = result.WasDispatched,
                Message = result.Message
            },
            result.Message);
    }
}

/// <summary>Validates typed device settings before vendor JSON is generated.</summary>
internal static class TenantDeviceRuntimeSettingsValidation
{
    internal static void ValidateCommon(TenantDeviceSettingRequestDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TenantDeviceId) ||
            string.IsNullOrWhiteSpace(dto.CurrentWebServerPassword) ||
            dto.CurrentWebServerPassword.Length is < 4 or > 128)
        {
            throw new ValidationErrorException("A valid Tenant device and current device Web UI/API password are required.");
        }
    }

    internal static void ValidateRange(int value, int minimum, int maximum, string label)
    {
        if (value < minimum || value > maximum)
        {
            throw new ValidationErrorException($"{label} must be between {minimum} and {maximum}.");
        }
    }

    internal static void ValidateOneOf(int value, string label, params int[] allowedValues)
    {
        if (!allowedValues.Contains(value))
        {
            throw new ValidationErrorException($"Select a supported {label} option.");
        }
    }

    internal static int ParseMonthDay(string? value, string label)
    {
        if (!DateTime.TryParseExact(value?.Trim(), ["M/d", "MM/dd"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            throw new ValidationErrorException($"{label} must use M/d format, for example 3/21.");
        }

        return parsed.Month * 32 + parsed.Day;
    }

    internal static int ParseMinuteOfDay(string? value, string label)
    {
        if (!TimeOnly.TryParseExact(value?.Trim(), ["H:mm", "HH:mm"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            throw new ValidationErrorException($"{label} must use 24-hour HH:mm format.");
        }

        return parsed.Hour * 60 + parsed.Minute;
    }

    internal static uint ParseIpv4(string? value, string label)
    {
        if (!IPAddress.TryParse(value, out var address) || address.AddressFamily != AddressFamily.InterNetwork)
        {
            throw new ValidationErrorException($"{label} must be a valid IPv4 address.");
        }

        var bytes = address.GetAddressBytes();
        return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
    }
}

/// <summary>Converts readable request properties into the exact keys accepted by the connected AiFace firmware.</summary>
internal static class TenantDeviceRuntimeSettingsPayloadBuilder
{
    internal static string BuildSetDeviceInfo(TenantDeviceSettingRequestDTO dto, TenantDeviceSettingsSection section)
    {
        var fields = section switch
        {
            TenantDeviceSettingsSection.Time when dto is UpdateTenantDeviceTimeSettingsRequestDTO time => BuildTime(time),
            TenantDeviceSettingsSection.Bell when dto is UpdateTenantDeviceBellSettingsRequestDTO bell => BuildBell(bell),
            TenantDeviceSettingsSection.DeviceSetup when dto is UpdateTenantDeviceSetupSettingsRequestDTO setup => BuildDeviceSetup(setup),
            TenantDeviceSettingsSection.Advanced when dto is UpdateTenantDeviceAdvancedSettingsRequestDTO advanced => BuildAdvanced(advanced),
            TenantDeviceSettingsSection.Lock when dto is UpdateTenantDeviceLockSettingsRequestDTO lockSettings => BuildLock(lockSettings),
            TenantDeviceSettingsSection.Serial when dto is UpdateTenantDeviceSerialSettingsRequestDTO serial => BuildSerial(serial),
            TenantDeviceSettingsSection.Ethernet when dto is UpdateTenantDeviceEthernetSettingsRequestDTO ethernet => BuildEthernet(ethernet),
            TenantDeviceSettingsSection.Wifi when dto is UpdateTenantDeviceWifiSettingsRequestDTO wifi => BuildWifi(wifi),
            TenantDeviceSettingsSection.AppNotification when dto is UpdateTenantDeviceAppNotificationSettingsRequestDTO notification => BuildAppNotification(notification),
            TenantDeviceSettingsSection.WebAccess when dto is UpdateTenantDeviceWebAccessRequestDTO webAccess => BuildWebAccess(webAccess),
            TenantDeviceSettingsSection.ScreenMenuPin when dto is UpdateTenantDeviceScreenMenuPinRequestDTO pin => BuildScreenMenuPin(pin),
            _ => throw new ValidationErrorException("The supplied settings do not match the requested device section.")
        };

        fields["cmd"] = DeviceCommands.SetDeviceInfo;
        fields["password"] = dto.CurrentWebServerPassword;
        fields["nowtime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return JsonSerializer.Serialize(fields);
    }

    internal static string BuildSetTime(SyncTenantDeviceTimeRequestDTO dto)
    {
        var utcDateTime = dto.UtcDateTime?.ToUniversalTime() ?? DateTime.UtcNow;
        return JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["cmd"] = DeviceCommands.SetTime,
            ["password"] = dto.CurrentWebServerPassword,
            ["cloudtime"] = utcDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            ["nowtime"] = new DateTimeOffset(utcDateTime).ToUnixTimeMilliseconds()
        });
    }

    internal static string BuildGatewayReplacement(string currentWebServerPassword, string pendingGatewayUrl) =>
        JsonSerializer.Serialize(new Dictionary<string, object?>
        {
            ["cmd"] = DeviceCommands.SetDeviceInfo,
            ["password"] = currentWebServerPassword,
            ["nowtime"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ["use_bs"] = 1,
            ["use_domain_name"] = 1,
            ["bs_domain_name"] = pendingGatewayUrl,
            ["serverport"] = 443
        });

    internal static string GetSectionLabel(TenantDeviceSettingsSection section) => section switch
    {
        TenantDeviceSettingsSection.Time => "Time",
        TenantDeviceSettingsSection.Bell => "Bell",
        TenantDeviceSettingsSection.DeviceSetup => "Device setup",
        TenantDeviceSettingsSection.Advanced => "Advanced",
        TenantDeviceSettingsSection.Lock => "Door and access control",
        TenantDeviceSettingsSection.Serial => "Serial connection",
        TenantDeviceSettingsSection.Ethernet => "Ethernet network",
        TenantDeviceSettingsSection.Wifi => "Wi-Fi network",
        TenantDeviceSettingsSection.AppNotification => "App notification",
        TenantDeviceSettingsSection.WebAccess => "Local Web UI/API access",
        TenantDeviceSettingsSection.ScreenMenuPin => "Screen System-menu PIN",
        _ => "Device"
    };

    private static Dictionary<string, object?> BuildTime(UpdateTenantDeviceTimeSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.TimeFormat, "clock format", 0, 1);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.DateFormat, "date format", 0, 1, 2);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.TimeZone, 0, 32, "Time zone");
        return new()
        {
            ["time_format"] = dto.TimeFormat,
            ["date_format"] = dto.DateFormat,
            ["use_dst"] = Bool(dto.DaylightSavingEnabled),
            ["dst_start"] = TenantDeviceRuntimeSettingsValidation.ParseMonthDay(dto.DaylightSavingStart, "DST start"),
            ["dst_end"] = TenantDeviceRuntimeSettingsValidation.ParseMonthDay(dto.DaylightSavingEnd, "DST end"),
            ["use_ntp"] = Bool(dto.NetworkTimeEnabled),
            ["ntp_timezone"] = dto.TimeZone,
            ["reboottime1"] = TenantDeviceRuntimeSettingsValidation.ParseMinuteOfDay(dto.RebootTime1, "Reboot time 1"),
            ["reboottime2"] = TenantDeviceRuntimeSettingsValidation.ParseMinuteOfDay(dto.RebootTime2, "Reboot time 2"),
            ["reboottime3"] = TenantDeviceRuntimeSettingsValidation.ParseMinuteOfDay(dto.RebootTime3, "Reboot time 3")
        };
    }

    private static Dictionary<string, object?> BuildBell(UpdateTenantDeviceBellSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.BellCount, 0, 100, "Bell count");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.RingStyle, "bell ring pattern", 0, 1);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.BellOutput, "bell output", 0, 1);
        return new() { ["bellcount"] = dto.BellCount, ["ring_style"] = dto.RingStyle, ["belloutput"] = dto.BellOutput };
    }

    private static Dictionary<string, object?> BuildDeviceSetup(UpdateTenantDeviceSetupSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.Language, "language", 0, 24);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.VoiceVolume, 0, 10, "Voice volume");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.ResultDisplaySeconds, 0, 99, "Result display time");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.ScreenSaverIdleSeconds, 0, 99_999_999, "Screen saver idle time");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.SleepModeSeconds, 0, 99_999_999, "Sleep mode time");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.ScreenWakeUpMethod, "screen wake-up method", 0, 1);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.FaceWakeUpSeconds, 0, 10, "Face wake-up time");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.ResultDisplayStyle, "result display style", 0, 1, 2);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.FaceRecognitionDistance, "face recognition distance", 0, 1, 2);
        return new()
        {
            ["language"] = dto.Language,
            ["volume"] = dto.VoiceVolume,
            ["tts_voice"] = Bool(dto.AnnouncePersonName),
            ["multifaces"] = Bool(dto.DetectMultipleFaces),
            ["showresulttime"] = dto.ResultDisplaySeconds,
            ["screensavertime"] = dto.ScreenSaverIdleSeconds,
            ["sleeptime"] = dto.SleepModeSeconds,
            ["screensaver_wakeup"] = dto.ScreenWakeUpMethod,
            ["face_wakeup_seconds"] = dto.FaceWakeUpSeconds,
            ["label_style"] = dto.ResultDisplayStyle,
            ["identify_distance"] = dto.FaceRecognitionDistance,
            ["live_detect"] = Bool(dto.LivenessDetectionEnabled),
            ["show_avatar"] = Bool(dto.ShowAvatar)
        };
    }

    private static Dictionary<string, object?> BuildAdvanced(UpdateTenantDeviceAdvancedSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.MaximumAdministrators, 0, 99_999_999, "Maximum administrators");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.VerificationMode, "verification method", 0, 2, 3, 8, 9, 10, 11, 14);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.QrCodeMode, "QR code mode", 0, 1, 2, 3);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.FaceMatchThreshold, 0, 99, "Face match threshold");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.LivenessThreshold, 0, 99, "Liveness threshold");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.FingerprintMatchThreshold, 0, 10, "Fingerprint threshold");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.FingerprintsPerUser, 0, 10, "Fingerprints per user");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.MaskThreshold, 0, 99, "Mask threshold");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.FillLightMode, "fill light mode", 0, 1, 2);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.ExposureCompensation, 0, 100, "Exposure compensation");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.PalmVeinMatchThreshold, 0, 99, "Palm vein threshold");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.PalmDetectionThreshold, 0, 99, "Palm detection threshold");
        return new()
        {
            ["managers"] = dto.MaximumAdministrators,
            ["verifymode"] = dto.VerificationMode,
            ["use_qrcode"] = dto.QrCodeMode,
            ["hide_privacy"] = Bool(dto.HidePrivacyInformation),
            ["fcmatch_level"] = dto.FaceMatchThreshold,
            ["live_treshold"] = dto.LivenessThreshold,
            ["fpmatch_level"] = dto.FingerprintMatchThreshold,
            ["fps_peruser"] = dto.FingerprintsPerUser,
            ["wear_mask"] = Bool(dto.MaskDetectionEnabled),
            ["mask_treshold"] = dto.MaskThreshold,
            ["filllight"] = dto.FillLightMode,
            ["filllight_period"] = ParseTimeRange(dto.ConstantFillLightPeriod, "Constant fill-light period"),
            ["exposure_value"] = dto.ExposureCompensation,
            ["palm_vein_level"] = dto.PalmVeinMatchThreshold,
            ["palm_det_level"] = dto.PalmDetectionThreshold,
            ["disable_face"] = Bool(dto.DisableFaceRecognition),
            ["online_debug"] = Bool(dto.OnlineDebugEnabled)
        };
    }

    private static Dictionary<string, object?> BuildLock(UpdateTenantDeviceLockSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.DoorOpenDelaySeconds, 0, 99_999_999, "Door open delay");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.DoorSensorMode, "door sensor mode", 0, 1, 2);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.DoorSensorDelaySeconds, 0, 99_999_999, "Door sensor delay");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.DoorPassword, 0, 99_999_999, "Door password");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.RequiredUsersForDoorOpen, 0, 99, "Required users");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.AntiPassbackMode, "anti-passback mode", 0, 1, 2, 3);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.WiegandOutput, "Wiegand output", 0, 1, 2, 3, 4);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.WiegandFormat, "Wiegand format", 0, 1, 2, 3, 4, 5);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.AccessLimit, 0, 65_535, "Access limit");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.CardDisplayFormat, "card display format", 0, 1, 2);
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.FailedVerificationLimit, 0, 99, "Failed verification limit");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.TimeZonePunchLimit, 0, 255, "Time-zone punch limit");
        return new()
        {
            ["door_opentime"] = dto.DoorOpenDelaySeconds,
            ["door_sensor_type"] = dto.DoorSensorMode,
            ["door_alarm_time"] = dto.DoorSensorDelaySeconds,
            ["stranger_lock"] = Bool(dto.BlockStrangerAccess),
            ["door_password"] = dto.DoorPassword,
            ["door_mutiopen"] = dto.RequiredUsersForDoorOpen,
            ["door_antipass"] = dto.AntiPassbackMode,
            ["wiegand_output_mode"] = dto.WiegandOutput,
            ["wiegand_output_bit"] = dto.WiegandFormat,
            ["access_times"] = dto.AccessLimit,
            ["card_disp_format"] = dto.CardDisplayFormat,
            ["card_reversal"] = Bool(dto.ReverseCardPin),
            ["wg_reversal"] = Bool(dto.ReverseWiegandOutput),
            ["outwg_shot"] = Bool(dto.ExternalWiegandSnapshotEnabled),
            ["door_interlock"] = Bool(dto.InterlockEnabled),
            ["door_firealarmin"] = Bool(dto.AlarmProcessingEnabled),
            ["door_trycount"] = dto.FailedVerificationLimit,
            ["timezone_punchs"] = dto.TimeZonePunchLimit,
            ["access_denied_nolog"] = Bool(dto.SuppressAccessDeniedLog),
            ["outof_notimzone_denied"] = Bool(dto.DenyOutsideNormallyOpenTimeZone)
        };
    }

    private static Dictionary<string, object?> BuildSerial(UpdateTenantDeviceSerialSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.DeviceAddress, 0, 9_999, "Device address");
        TenantDeviceRuntimeSettingsValidation.ValidateRange(dto.NetworkPort, 1, 99_999_999, "Network port");
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.BaudRate, "baud rate", 0, 1, 2, 3, 4);
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.SerialFunction, "serial function", 0, 1, 2);
        return new() { ["deviceid"] = dto.DeviceAddress, ["netportnum"] = dto.NetworkPort, ["combaurate"] = dto.BaudRate, ["rs485_fun"] = dto.SerialFunction };
    }

    private static Dictionary<string, object?> BuildEthernet(UpdateTenantDeviceEthernetSettingsRequestDTO dto)
    {
        var fields = new Dictionary<string, object?> { ["use_eth_dhcp"] = Bool(dto.DhcpEnabled), ["hide_ip"] = Bool(dto.HideIpAddress) };
        if (!dto.DhcpEnabled)
        {
            fields["netipaddress"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.IpAddress, "Ethernet IP address");
            fields["netmask"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.SubnetMask, "Ethernet subnet mask");
            fields["netgateway"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.Gateway, "Ethernet gateway");
            fields["dns_server_ip"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.DnsServer, "DNS server");
        }

        return fields;
    }

    private static Dictionary<string, object?> BuildWifi(UpdateTenantDeviceWifiSettingsRequestDTO dto)
    {
        var fields = new Dictionary<string, object?> { ["use_wlan_dhcp"] = Bool(dto.DhcpEnabled) };
        if (!dto.DhcpEnabled)
        {
            fields["wlan_ip_address"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.IpAddress, "Wi-Fi IP address");
            fields["wlan_net_mask"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.SubnetMask, "Wi-Fi subnet mask");
            fields["wlan_gateway"] = TenantDeviceRuntimeSettingsValidation.ParseIpv4(dto.Gateway, "Wi-Fi gateway");
        }

        return fields;
    }

    private static Dictionary<string, object?> BuildAppNotification(UpdateTenantDeviceAppNotificationSettingsRequestDTO dto)
    {
        TenantDeviceRuntimeSettingsValidation.ValidateOneOf(dto.NotificationType, "notification frequency", 0, 1, 2, 3);
        if (dto.AppNotificationEnabled && string.IsNullOrWhiteSpace(dto.AppToken))
        {
            throw new ValidationErrorException("An app token is required when app notifications are enabled.");
        }

        if (dto.AppToken?.Length > 4_000)
        {
            throw new ValidationErrorException("App token cannot exceed 4,000 characters.");
        }

        var fields = new Dictionary<string, object?>
        {
            ["line_notify"] = Bool(dto.AppNotificationEnabled),
            ["line_push_type"] = dto.NotificationType
        };
        if (!string.IsNullOrWhiteSpace(dto.AppToken))
        {
            fields["app_token"] = dto.AppToken;
        }

        return fields;
    }

    private static Dictionary<string, object?> BuildWebAccess(UpdateTenantDeviceWebAccessRequestDTO dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.NewWebServerPassword) && dto.NewWebServerPassword.Length is < 4 or > 128)
        {
            throw new ValidationErrorException("New local Web UI/API password must be between 4 and 128 characters.");
        }

        var fields = new Dictionary<string, object?> { ["use_webserver"] = Bool(dto.LocalWebServerEnabled) };
        if (!string.IsNullOrWhiteSpace(dto.NewWebServerPassword))
        {
            fields["webserver_pwd"] = dto.NewWebServerPassword;
        }

        return fields;
    }

    private static Dictionary<string, object?> BuildScreenMenuPin(UpdateTenantDeviceScreenMenuPinRequestDTO dto)
    {
        if (dto.ScreenMenuPin.Length is < 4 or > 12 || !dto.ScreenMenuPin.All(char.IsDigit))
        {
            throw new ValidationErrorException("Screen System-menu PIN must contain 4 to 12 digits.");
        }

        return new() { ["local_manager_pwd"] = long.Parse(dto.ScreenMenuPin, CultureInfo.InvariantCulture) };
    }

    private static int Bool(bool value) => value ? 1 : 0;

    private static int ParseTimeRange(string? value, string label)
    {
        var parts = value?.Split('~', StringSplitOptions.TrimEntries);
        if (parts is null || parts.Length != 2)
        {
            throw new ValidationErrorException($"{label} must use HH:mm~HH:mm format.");
        }

        var start = TenantDeviceRuntimeSettingsValidation.ParseMinuteOfDay(parts[0], label);
        var end = TenantDeviceRuntimeSettingsValidation.ParseMinuteOfDay(parts[1], label);
        return ((start / 60) << 24) | ((start % 60) << 16) | ((end / 60) << 8) | (end % 60);
    }
}
