// ================================================================
// Purpose : Persists the MQTT/MQTTS command queue and protocol audit. The
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

/// <summary>Provides durable command submission, dispatch coordination, and idempotent response completion.</summary>
public sealed class DeviceCommandRepository(
    WorkforceDbContext context,
    ILogger<DeviceCommandRepository> logger)
    : IDeviceCommandSubmissionService, IDeviceCommandDispatchStore
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
    public async Task<DeviceCommandDispatch?> TryAcquireNextAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var activeStatuses = new[] { Publishing, AwaitingResponse, RetryScheduled };
            var candidate = await context.DeviceCommands
                .Where(command =>
                    (command.Status == Queued ||
                     (command.Status == RetryScheduled && command.NextAttemptDateTime <= now)) &&
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

        var protocol = tenantDevice.TenantDeviceConfiguration?.MqttTransport;
        if (protocol is not (short)DeviceCommunicationProtocol.Mqtt and not (short)DeviceCommunicationProtocol.Mqtts)
        {
            throw new ValidationErrorException("The Tenant device must have an MQTT or MQTTS configuration before a command can be queued.");
        }

        if (protocol == (short)DeviceCommunicationProtocol.Mqtt && !tenantDevice.DeviceMaster.SupportsMqtt ||
            protocol == (short)DeviceCommunicationProtocol.Mqtts && !tenantDevice.DeviceMaster.SupportsMqtts)
        {
            throw new ValidationErrorException("The selected device model does not support its configured MQTT transport.");
        }

        if (definition.AccessLevel == DeviceCommandAccessLevel.TenantAccessControlPermission &&
            !tenantDevice.DeviceMaster.IsAccessControlDevice)
        {
            throw new ValidationErrorException("The requested door-control command requires an access-control device.");
        }
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
