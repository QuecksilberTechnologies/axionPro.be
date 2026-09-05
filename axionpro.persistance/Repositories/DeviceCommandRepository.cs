// ================================================================
// Purpose : Persists the transport-neutral command queue and protocol audit. The
//           database, not client-supplied tenant data, resolves every device.
// ================================================================

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using axionpro.application.Constants;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.IDeviceCommunication;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace axionpro.persistance.Repositories;

/// <summary>Provides durable command submission, transport dispatch coordination, and idempotent response completion.</summary>
public sealed class DeviceCommandRepository(
    WorkforceDbContext context,
    ILogger<DeviceCommandRepository> logger)
    : IDeviceCommandSubmissionService, IDeviceCommandDispatchStore, IDeviceHttpsPollingService
{
    private static readonly short Queued = (short)DeviceCommandStatus.Queued;
    private static readonly short Publishing = (short)DeviceCommandStatus.Publishing;
    private static readonly short AwaitingResponse = (short)DeviceCommandStatus.AwaitingResponse;
    private static readonly short Completed = (short)DeviceCommandStatus.Completed;
    private static readonly short Failed = (short)DeviceCommandStatus.Failed;
    private static readonly short RetryScheduled = (short)DeviceCommandStatus.RetryScheduled;

    /// <inheritdoc />
    public async Task<DeviceCommandSubmissionResult> SubmitAsync(
        DeviceCommandSubmission submission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(submission);

        var definition = DeviceProtocolCommandCatalog.GetRequired(submission.CommandName);
        var payload = DeviceProtocolCommandCatalog.ValidatePayload(definition.Name, submission.Payload);
        var tenantDevice = await context.TenantDevices
            .Include(device => device.DeviceMaster)
            .Include(device => device.TenantDeviceConfiguration)
            .FirstOrDefaultAsync(
                device => device.Id == submission.TenantDeviceId && device.TenantId == submission.TenantId,
                cancellationToken)
            ?? throw new NotFoundException("The requested Tenant device was not found.");

        ValidateTarget(tenantDevice, definition);
        EnsurePayloadSerialDoesNotConflict(payload, tenantDevice.DeviceMaster.SNo);

        var now = DateTime.UtcNow;
        var command = new DeviceCommand
        {
            InternalTrackingId = Guid.NewGuid(),
            TenantId = tenantDevice.TenantId,
            TenantDeviceId = tenantDevice.Id,
            TenantLocationId = tenantDevice.TenantLocationId,
            DeviceSerialNumber = tenantDevice.DeviceMaster.SNo.Trim(),
            CommandName = definition.Name,
            RequestPayload = payload,
            MatchCriteria = DeviceProtocolCommandCatalog.BuildMatchCriteria(definition, payload),
            Status = Queued,
            ResponseMode = (short)definition.ResponseMode,
            AccessLevel = (short)definition.AccessLevel,
            MaxAttempts = Math.Clamp(submission.MaxAttempts, 1, 10),
            RequestedById = submission.RequestedById,
            AddedDateTime = now
        };

        await context.DeviceCommands.AddAsync(command, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Queued DeviceCommand {DeviceCommandId} ({CommandName}) for TenantDevice {TenantDeviceId}.",
            command.Id,
            command.CommandName,
            command.TenantDeviceId);

        return new DeviceCommandSubmissionResult(
            command.Id,
            command.InternalTrackingId,
            command.DeviceSerialNumber,
            DeviceCommandStatus.Queued);
    }

    /// <inheritdoc />
    public async Task<DeviceCommandDispatch?> TryAcquireNextAsync(
        IReadOnlyCollection<DeviceCommunicationProtocol> transports,
        long? tenantDeviceId = null,
        CancellationToken cancellationToken = default)
    {
        if (transports.Count == 0)
        {
            return null;
        }

        var transportCodes = transports.Select(transport => (short)transport).Distinct().ToArray();
        var now = DateTime.UtcNow;
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var activeStatuses = new[] { Publishing, AwaitingResponse, RetryScheduled };
            var candidate = await context.DeviceCommands
                .Where(command =>
                    (!tenantDeviceId.HasValue || command.TenantDeviceId == tenantDeviceId.Value) &&
                    (command.Status == Queued ||
                     (command.Status == RetryScheduled && command.NextAttemptDateTime <= now)) &&
                    context.TenantDeviceConfigurations.Any(configuration =>
                        configuration.TenantDeviceId == command.TenantDeviceId &&
                        transportCodes.Contains(configuration.CommandTransport ?? configuration.MqttTransport ?? 0)) &&
                    !context.DeviceCommands.Any(other =>
                        other.DeviceSerialNumber == command.DeviceSerialNumber &&
                        other.Id != command.Id &&
                        activeStatuses.Contains(other.Status)) &&
                    !context.DeviceCommands.Any(other =>
                        other.DeviceSerialNumber == command.DeviceSerialNumber &&
                        other.Id != command.Id &&
                        other.Status == Queued &&
                        (other.AddedDateTime < command.AddedDateTime ||
                         (other.AddedDateTime == command.AddedDateTime && other.Id < command.Id))))
                .OrderBy(command => command.AddedDateTime)
                .ThenBy(command => command.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (candidate is null)
            {
                await transaction.CommitAsync(cancellationToken);
                return null;
            }

            candidate.Status = Publishing;
            candidate.AttemptCount++;
            candidate.NextAttemptDateTime = null;
            candidate.UpdatedDateTime = now;
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new DeviceCommandDispatch(
                candidate.Id,
                candidate.TenantId,
                candidate.TenantDeviceId,
                candidate.DeviceSerialNumber,
                candidate.CommandName,
                candidate.RequestPayload,
                (DeviceCommandResponseMode)candidate.ResponseMode,
                candidate.AttemptCount);
        }
        catch (DbUpdateException exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogDebug(exception, "Another dispatcher acquired a command first.");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task RecoverExpiredResponseDeadlinesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredCommands = await context.DeviceCommands
            .Where(command =>
                command.Status == AwaitingResponse &&
                command.ResponseDeadlineDateTime != null &&
                command.ResponseDeadlineDateTime <= now)
            .ToListAsync(cancellationToken);

        foreach (var command in expiredCommands)
        {
            command.FailureReason = "The device did not return its documented response before the response deadline.";
            command.UpdatedDateTime = now;
            command.ResponseDeadlineDateTime = null;
            if (command.AttemptCount >= command.MaxAttempts)
            {
                command.Status = Failed;
                command.CompletedDateTime = now;
            }
            else
            {
                command.Status = RetryScheduled;
                command.NextAttemptDateTime = now.AddSeconds(Math.Min(60, command.AttemptCount * 10));
            }
        }

        if (expiredCommands.Count > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                "Scheduled retry or final failure for {ExpiredCommandCount} expired device command response deadline(s).",
                expiredCommands.Count);
        }
    }

    /// <inheritdoc />
    public async Task MarkPublishedAsync(
        DeviceCommandDispatch dispatch,
        string topic,
        int qualityOfService,
        DateTime publishedDateTime,
        CancellationToken cancellationToken = default)
    {
        var command = await context.DeviceCommands.FirstOrDefaultAsync(item => item.Id == dispatch.DeviceCommandId, cancellationToken);
        if (command is null || command.Status != Publishing)
        {
            return;
        }

        await context.DeviceMessageLogs.AddAsync(new DeviceMessageLog
        {
            TenantId = command.TenantId,
            TenantDeviceId = command.TenantDeviceId,
            DeviceSerialNumber = command.DeviceSerialNumber,
            Topic = topic,
            Direction = (short)DeviceMessageDirection.Outbound,
            QualityOfService = qualityOfService,
            IsDuplicateDelivery = false,
            PayloadHash = Hash(command.RequestPayload),
            RawPayload = command.RequestPayload,
            OccurredDateTime = publishedDateTime,
            AddedDateTime = publishedDateTime
        }, cancellationToken);

        command.PublishedDateTime = publishedDateTime;
        command.UpdatedDateTime = publishedDateTime;
        if ((DeviceCommandResponseMode)command.ResponseMode == DeviceCommandResponseMode.PublishOnly)
        {
            command.Status = Completed;
            command.CompletedDateTime = publishedDateTime;
        }
        else
        {
            command.Status = AwaitingResponse;
            command.ResponseDeadlineDateTime = publishedDateTime.AddSeconds(60);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task ScheduleRetryOrFailAsync(
        DeviceCommandDispatch dispatch,
        string failureReason,
        DateTime failedDateTime,
        CancellationToken cancellationToken = default)
    {
        var command = await context.DeviceCommands.FirstOrDefaultAsync(item => item.Id == dispatch.DeviceCommandId, cancellationToken);
        if (command is null || command.Status != Publishing)
        {
            return;
        }

        // Preserve every outbound attempt, including an MQTT client/broker failure.
        // The raw audit is therefore complete even when the command is scheduled to retry.
        await context.DeviceMessageLogs.AddAsync(new DeviceMessageLog
        {
            TenantId = command.TenantId,
            TenantDeviceId = command.TenantDeviceId,
            DeviceSerialNumber = command.DeviceSerialNumber,
            Topic = $"aiface/{command.DeviceSerialNumber}/pub",
            Direction = (short)DeviceMessageDirection.Outbound,
            QualityOfService = 1,
            IsDuplicateDelivery = false,
            PayloadHash = Hash(command.RequestPayload),
            RawPayload = command.RequestPayload,
            OccurredDateTime = failedDateTime,
            AddedDateTime = failedDateTime
        }, cancellationToken);

        command.FailureReason = Truncate(failureReason, 2000);
        command.UpdatedDateTime = failedDateTime;
        if (command.AttemptCount >= command.MaxAttempts)
        {
            command.Status = Failed;
            command.CompletedDateTime = failedDateTime;
        }
        else
        {
            command.Status = RetryScheduled;
            command.NextAttemptDateTime = failedDateTime.AddSeconds(Math.Min(60, command.AttemptCount * 10));
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeviceHttpsPollingResponse> ProcessAsync(
        DeviceHttpsPollingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!DeviceHttpsGatewaySecurity.IsValidIngressToken(request.IngressToken) ||
            string.IsNullOrWhiteSpace(request.Payload))
        {
            return DeviceHttpsPollingResponse.Rejected;
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(request.Payload);
        }
        catch (JsonException)
        {
            return DeviceHttpsPollingResponse.Rejected;
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("sn", out var serialProperty) ||
                serialProperty.ValueKind != JsonValueKind.String)
            {
                return DeviceHttpsPollingResponse.Rejected;
            }

            var serialNumber = serialProperty.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(serialNumber) || serialNumber.Length > 100)
            {
                return DeviceHttpsPollingResponse.Rejected;
            }

            var tokenHash = DeviceHttpsGatewaySecurity.HashIngressToken(request.IngressToken);
            var devices = await context.TenantDevices
                .Include(device => device.DeviceMaster)
                .Include(device => device.TenantDeviceConfiguration)
                .Where(device =>
                    device.IsActive && !device.IsSoftDeleted &&
                    device.DeviceMaster.IsActive && !device.DeviceMaster.IsSoftDeleted &&
                    device.DeviceMaster.SupportsHttps &&
                    device.TenantDeviceConfiguration != null &&
                    (device.TenantDeviceConfiguration.CommandTransport ?? device.TenantDeviceConfiguration.MqttTransport) ==
                        (short)DeviceCommunicationProtocol.Https &&
                    device.TenantDeviceConfiguration.HttpsIngressTokenHash == tokenHash)
                .Take(2)
                .ToListAsync(cancellationToken);

            var device = devices.Count == 1 ? devices[0] : null;
            if (device is null ||
                !string.Equals(device.DeviceMaster.SNo.Trim(), serialNumber, StringComparison.Ordinal))
            {
                return DeviceHttpsPollingResponse.Rejected;
            }

            // A device request never executes its own cmd value. It is only audited,
            // matched to a prior AxionPro command response, and then receives the next
            // server-owned queue item in this same outbound HTTPS response.
            await RecordInboundAsync(
                new DeviceMqttInboundMessage(
                    "https",
                    serialNumber,
                    request.Payload,
                    QualityOfService: 0,
                    IsDuplicateDelivery: false,
                    IsProtocolIdentityValid: true,
                    ReceivedDateTime: request.ReceivedDateTime),
                cancellationToken);

            var configuration = device.TenantDeviceConfiguration!;
            configuration.LastHeartbeatDateTime = request.ReceivedDateTime;
            configuration.LastSuccessfulConnectionDateTime = request.ReceivedDateTime;
            configuration.LastConnectionError = null;
            configuration.UpdatedDateTime = request.ReceivedDateTime;
            await context.SaveChangesAsync(cancellationToken);

            var dispatch = await TryAcquireNextAsync(
                new[] { DeviceCommunicationProtocol.Https },
                device.Id,
                cancellationToken);
            if (dispatch is not null)
            {
                await MarkPublishedAsync(dispatch, "https", qualityOfService: 0, request.ReceivedDateTime, cancellationToken);
                return new DeviceHttpsPollingResponse(true, dispatch.Payload);
            }

            return new DeviceHttpsPollingResponse(
                true,
                BuildHttpsAcknowledgement(root, serialNumber, configuration.HeartbeatIntervalSeconds ?? 15));
        }
    }

    /// <inheritdoc />
    public async Task RecordInboundAsync(DeviceMqttInboundMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        var devices = await context.TenantDevices
            .Include(device => device.DeviceMaster)
            .Where(device =>
                device.DeviceMaster.SNo == message.DeviceSerialNumber &&
                device.IsActive && !device.IsSoftDeleted &&
                device.DeviceMaster.IsActive && !device.DeviceMaster.IsSoftDeleted)
            .Take(2)
            .ToListAsync(cancellationToken);
        var device = devices.Count == 1 ? devices[0] : null;

        // Raw audit is committed before response matching. A duplicate delivery therefore
        // remains visible even when the command completion below is intentionally idempotent.
        await context.DeviceMessageLogs.AddAsync(new DeviceMessageLog
        {
            TenantId = device?.TenantId,
            TenantDeviceId = device?.Id,
            DeviceSerialNumber = message.DeviceSerialNumber,
            Topic = message.Topic,
            Direction = (short)DeviceMessageDirection.Inbound,
            QualityOfService = message.QualityOfService,
            IsDuplicateDelivery = message.IsDuplicateDelivery,
            PayloadHash = Hash(message.Payload),
            RawPayload = message.Payload,
            OccurredDateTime = message.ReceivedDateTime,
            AddedDateTime = DateTime.UtcNow
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        if (!message.IsProtocolIdentityValid)
        {
            logger.LogWarning(
                "Ignored MQTT payload processing for serial {DeviceSerialNumber}: topic and payload identity did not agree.",
                message.DeviceSerialNumber);
            return;
        }

        if (device is null)
        {
            logger.LogWarning(
                "Ignored MQTT command processing for serial {DeviceSerialNumber}: active TenantDevice mapping was missing or ambiguous.",
                message.DeviceSerialNumber);
            return;
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(message.Payload);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "MQTT payload for serial {DeviceSerialNumber} is not JSON.", message.DeviceSerialNumber);
            return;
        }

        using (document)
        {
            var root = document.RootElement;
            if (root.TryGetProperty("cmd", out var inboundCommand) && inboundCommand.ValueKind == JsonValueKind.String &&
                string.Equals(inboundCommand.GetString(), DeviceCommands.CheckLive, StringComparison.OrdinalIgnoreCase))
            {
                var configuration = await context.TenantDeviceConfigurations
                    .FirstOrDefaultAsync(configuration => configuration.TenantDeviceId == device.Id, cancellationToken);
                if (configuration is not null)
                {
                    configuration.LastHeartbeatDateTime = message.ReceivedDateTime;
                    configuration.LastSuccessfulConnectionDateTime = message.ReceivedDateTime;
                    configuration.LastConnectionError = null;
                    configuration.UpdatedDateTime = message.ReceivedDateTime;
                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            if (!root.TryGetProperty("ret", out var ret) || ret.ValueKind != JsonValueKind.String)
            {
                return;
            }

            var commandName = ret.GetString()?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(commandName))
            {
                return;
            }

            var awaitingCommands = await context.DeviceCommands
                .Where(command =>
                    command.TenantDeviceId == device.Id &&
                    (command.Status == AwaitingResponse || command.Status == RetryScheduled) &&
                    command.CommandName == commandName)
                .OrderBy(command => command.PublishedDateTime)
                .ThenBy(command => command.Id)
                .ToListAsync(cancellationToken);
            var matchedCommand = awaitingCommands.FirstOrDefault(command => DeviceProtocolCommandCatalog.ResponseMatches(command, root));
            var result = root.TryGetProperty("result", out var resultProperty) && resultProperty.ValueKind is JsonValueKind.True or JsonValueKind.False
                ? resultProperty.GetBoolean()
                : (bool?)null;

            if (message.IsDuplicateDelivery && await context.DeviceCommandResponses.AnyAsync(
                    response =>
                        response.TenantDeviceId == device.Id &&
                        response.ResponseCommandName == commandName &&
                        response.ResponsePayload == message.Payload,
                    cancellationToken))
            {
                // The raw MQTT audit is intentionally retained above, but QoS 1 redelivery
                // must not create another parsed response or change command state.
                return;
            }

            await context.DeviceCommandResponses.AddAsync(new DeviceCommandResponse
            {
                DeviceCommandId = matchedCommand?.Id,
                TenantId = device.TenantId,
                TenantDeviceId = device.Id,
                DeviceSerialNumber = message.DeviceSerialNumber,
                ResponseCommandName = commandName,
                Result = result,
                FailureReason = result == false ? ExtractFailureReason(root) : null,
                ResponsePayload = message.Payload,
                ReceivedDateTime = message.ReceivedDateTime
            }, cancellationToken);

            if (matchedCommand is not null)
            {
                matchedCommand.Status = result == false ? Failed : Completed;
                matchedCommand.CompletedDateTime = message.ReceivedDateTime;
                matchedCommand.FailureReason = result == false ? ExtractFailureReason(root) : null;
                matchedCommand.UpdatedDateTime = message.ReceivedDateTime;
            }

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (matchedCommand is not null)
            {
                // The unique response FK is the durable idempotency boundary for QoS 1 redelivery.
                logger.LogInformation(exception, "Duplicate response for DeviceCommand {DeviceCommandId} was ignored.", matchedCommand.Id);
                context.ChangeTracker.Clear();
            }
        }
    }

    private static void ValidateTarget(TenantDevice tenantDevice, DeviceProtocolCommandDefinition definition)
    {
        if (!tenantDevice.IsActive || tenantDevice.IsSoftDeleted ||
            !tenantDevice.DeviceMaster.IsActive || tenantDevice.DeviceMaster.IsSoftDeleted)
        {
            throw new ConflictException("The requested Tenant device is not active.");
        }

        var transportCode = tenantDevice.TenantDeviceConfiguration?.CommandTransport
            ?? tenantDevice.TenantDeviceConfiguration?.MqttTransport;
        if (!transportCode.HasValue || !Enum.IsDefined((DeviceCommunicationProtocol)transportCode.Value))
        {
            throw new ValidationErrorException("The Tenant device must have a valid command transport configuration before a command can be queued.");
        }

        var transport = (DeviceCommunicationProtocol)transportCode.Value;
        if (!SupportsTransport(tenantDevice.DeviceMaster, transport))
        {
            throw new ValidationErrorException("The selected device model does not support its configured command transport.");
        }

        if (transport is not (DeviceCommunicationProtocol.Mqtt or DeviceCommunicationProtocol.Mqtts or DeviceCommunicationProtocol.Https))
        {
            throw new ValidationErrorException("The selected command transport does not yet have an enabled AxionPro transport adapter.");
        }

        if (transport == DeviceCommunicationProtocol.Https &&
            string.IsNullOrWhiteSpace(tenantDevice.TenantDeviceConfiguration?.HttpsIngressTokenHash))
        {
            throw new ValidationErrorException("Generate the HTTPS device gateway URL before queueing a command for this device.");
        }

        if (definition.AccessLevel == DeviceCommandAccessLevel.TenantAccessControlPermission &&
            !tenantDevice.DeviceMaster.IsAccessControlDevice)
        {
            throw new ValidationErrorException("The requested door-control command requires an access-control device.");
        }
    }

    private static bool SupportsTransport(DeviceMaster deviceMaster, DeviceCommunicationProtocol transport) => transport switch
    {
        DeviceCommunicationProtocol.Mqtt => deviceMaster.SupportsMqtt,
        DeviceCommunicationProtocol.Mqtts => deviceMaster.SupportsMqtts,
        DeviceCommunicationProtocol.Http => deviceMaster.SupportsHttp,
        DeviceCommunicationProtocol.Https => deviceMaster.SupportsHttps,
        DeviceCommunicationProtocol.WebSocket or DeviceCommunicationProtocol.WebSocketSecure => deviceMaster.SupportsWebSocket,
        _ => false
    };

    private static void EnsurePayloadSerialDoesNotConflict(string payload, string deviceSerialNumber)
    {
        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.TryGetProperty("sn", out var serialProperty) &&
            serialProperty.ValueKind == JsonValueKind.String &&
            !string.Equals(serialProperty.GetString()?.Trim(), deviceSerialNumber.Trim(), StringComparison.Ordinal))
        {
            throw new ValidationErrorException("The command payload serial number does not match the selected Tenant device.");
        }
    }

    private static string BuildHttpsAcknowledgement(JsonElement request, string serialNumber, int heartbeatIntervalSeconds)
    {
        var responseCommand = request.TryGetProperty("cmd", out var commandProperty) &&
                              commandProperty.ValueKind == JsonValueKind.String
            ? commandProperty.GetString()?.Trim()
            : request.TryGetProperty("ret", out var responseProperty) && responseProperty.ValueKind == JsonValueKind.String
                ? responseProperty.GetString()?.Trim()
                : DeviceCommands.CheckLive;

        return JsonSerializer.Serialize(new
        {
            ret = string.IsNullOrWhiteSpace(responseCommand) ? DeviceCommands.CheckLive : responseCommand,
            sn = serialNumber,
            result = true,
            tryseconds = Math.Clamp(heartbeatIntervalSeconds, 10, 3600)
        });
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static string? ExtractFailureReason(JsonElement root)
    {
        foreach (var name in new[] { "message", "msg", "reason", "error" })
        {
            if (root.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String)
            {
                return Truncate(value.GetString(), 2000);
            }
        }

        return "The device returned result=false.";
    }

    private static string? Truncate(string? value, int maxLength) =>
        string.IsNullOrWhiteSpace(value) || value.Length <= maxLength ? value : value[..maxLength];
}
