using System.Security.Cryptography;
using System.Text;
using axionpro.application.Common.Models;
using axionpro.application.DTOS.EmployeeMonitoring;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Options;

namespace axionpro.application.Features.EmployeeMonitoringAgent;

#region Commands and queries

public sealed record GetAgentConfigurationQuery(string Credential)
    : IRequest<ApiResponse<EmployeeMonitoringPolicyResponseDTO>>;

public sealed record RecordAgentHeartbeatCommand(string Credential, AgentHeartbeatRequestDTO DTO)
    : IRequest<ApiResponse<bool>>;

public sealed record UploadEmployeeScreenCaptureCommand(
    string Credential,
    Guid CaptureId,
    DateTime CapturedAtUtc,
    int MonitorNumber,
    string ContentType,
    long ContentLength,
    string? ChecksumSha256,
    Stream Content)
    : IRequest<ApiResponse<bool>>;

#endregion

#region Handlers

public abstract class EmployeeMonitoringAgentHandlerBase(IEmployeeMonitoringRepository repository)
{
    protected IEmployeeMonitoringRepository Repository { get; } = repository;

    protected async Task<axionpro.domain.Entity.EmployeeMonitoringAgent> AuthenticateAsync(
        string credential,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(credential))
            throw new UnauthorizedAccessException("A valid agent credential is required.");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(credential.Trim())));
        return await Repository.GetAgentByCredentialHashAsync(hash, cancellationToken)
            ?? throw new UnauthorizedAccessException("The employee monitoring agent credential is invalid or inactive.");
    }

    protected static EmployeeMonitoringPolicyResponseDTO ToPolicyResponse(EmployeeMonitoringPolicy policy) => new(
        policy.MinimumCaptureIntervalSeconds,
        policy.MaximumCaptureIntervalSeconds,
        policy.HeartbeatIntervalSeconds,
        policy.DelayedAfterSeconds,
        policy.UnreachableAfterSeconds,
        policy.OfflineRetentionDays,
        policy.MaximumOfflineBytes,
        policy.ImageQuality,
        policy.CaptureAllMonitors);
}

public sealed class GetAgentConfigurationQueryHandler(IEmployeeMonitoringRepository repository)
    : EmployeeMonitoringAgentHandlerBase(repository), IRequestHandler<GetAgentConfigurationQuery, ApiResponse<EmployeeMonitoringPolicyResponseDTO>>
{
    public async Task<ApiResponse<EmployeeMonitoringPolicyResponseDTO>> Handle(GetAgentConfigurationQuery request, CancellationToken cancellationToken)
    {
        var agent = await AuthenticateAsync(request.Credential, cancellationToken);
        return ApiResponse<EmployeeMonitoringPolicyResponseDTO>.Success(ToPolicyResponse(agent.Policy));
    }
}

public sealed class RecordAgentHeartbeatCommandHandler(IEmployeeMonitoringRepository repository)
    : EmployeeMonitoringAgentHandlerBase(repository), IRequestHandler<RecordAgentHeartbeatCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(RecordAgentHeartbeatCommand request, CancellationToken cancellationToken)
    {
        var agent = await AuthenticateAsync(request.Credential, cancellationToken);
        if (request.DTO.PendingCaptureCount < 0 || request.DTO.AgentVersion?.Length > 50 || request.DTO.LastErrorCode?.Length > 100)
            throw new ValidationErrorException("The heartbeat payload is invalid.");
        agent.AgentVersion = request.DTO.AgentVersion?.Trim();
        agent.PendingCaptureCount = request.DTO.PendingCaptureCount;
        agent.LastCaptureDateTime = request.DTO.LastCaptureDateTime;
        agent.LastSuccessfulUploadDateTime = request.DTO.LastSuccessfulUploadDateTime;
        agent.LastErrorCode = request.DTO.LastErrorCode?.Trim();
        agent.LastHeartbeatDateTime = DateTime.UtcNow;
        agent.UpdatedDateTime = DateTime.UtcNow;
        await Repository.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Success(true);
    }
}

public sealed class UploadEmployeeScreenCaptureCommandHandler(
    IEmployeeMonitoringRepository repository,
    IEmployeeScreenshotStorage storage,
    IOptions<EmployeeMonitoringStorageOptions> options)
    : EmployeeMonitoringAgentHandlerBase(repository), IRequestHandler<UploadEmployeeScreenCaptureCommand, ApiResponse<bool>>
{
    private readonly EmployeeMonitoringStorageOptions _options = options.Value;

    public async Task<ApiResponse<bool>> Handle(UploadEmployeeScreenCaptureCommand request, CancellationToken cancellationToken)
    {
        var agent = await AuthenticateAsync(request.Credential, cancellationToken);
        if (request.CaptureId == Guid.Empty || request.MonitorNumber < 0)
            throw new ValidationErrorException("The capture identity or monitor number is invalid.");
        if (request.ContentLength <= 0 || request.ContentLength > _options.MaximumUploadBytes)
            throw new ValidationErrorException("The screenshot size is not allowed.");
        if (!_options.AllowedContentTypes.Contains(request.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new ValidationErrorException("The screenshot content type is not allowed.");
        if (request.CapturedAtUtc > DateTime.UtcNow.AddMinutes(5) || request.CapturedAtUtc < DateTime.UtcNow.AddDays(-agent.Policy.OfflineRetentionDays))
            throw new ValidationErrorException("The screenshot capture time is outside the accepted window.");
        if (await Repository.CaptureExistsAsync(agent.Id, request.CaptureId, cancellationToken))
            return ApiResponse<bool>.Success(true, "The screenshot was already synchronized.");

        await using var buffered = new MemoryStream();
        await request.Content.CopyToAsync(buffered, cancellationToken);
        if (buffered.Length != request.ContentLength || buffered.Length > _options.MaximumUploadBytes)
            throw new ValidationErrorException("The screenshot content length does not match the request.");
        var checksum = Convert.ToHexString(SHA256.HashData(buffered.ToArray()));
        if (!string.IsNullOrWhiteSpace(request.ChecksumSha256) && !checksum.Equals(request.ChecksumSha256, StringComparison.OrdinalIgnoreCase))
            throw new ValidationErrorException("The screenshot checksum does not match.");

        buffered.Position = 0;
        var objectKey = await storage.SaveAsync(agent.TenantId, agent.EmployeeId, request.CaptureId, request.ContentType, buffered, cancellationToken);
        try
        {
            await Repository.AddCaptureAsync(new EmployeeScreenCapture
            {
                CaptureId = request.CaptureId,
                TenantId = agent.TenantId,
                EmployeeId = agent.EmployeeId,
                MonitoringAgentId = agent.Id,
                CapturedAtUtc = request.CapturedAtUtc,
                ReceivedAtUtc = DateTime.UtcNow,
                MonitorNumber = request.MonitorNumber,
                ContentType = request.ContentType,
                ContentLength = buffered.Length,
                ChecksumSha256 = checksum,
                StorageObjectKey = objectKey
            }, cancellationToken);
            agent.LastSuccessfulUploadDateTime = DateTime.UtcNow;
            agent.LastCaptureDateTime = request.CapturedAtUtc;
            agent.LastErrorCode = null;
            agent.UpdatedDateTime = DateTime.UtcNow;
            await Repository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await storage.DeleteAsync(objectKey, cancellationToken);
            throw;
        }

        return ApiResponse<bool>.Success(true, "Screenshot synchronized successfully.");
    }
}

#endregion
