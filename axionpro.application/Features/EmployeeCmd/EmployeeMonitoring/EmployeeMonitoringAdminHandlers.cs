using System.Security.Cryptography;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.EmployeeMonitoring;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.EmployeeMonitoring;

#region Commands and queries

public sealed record UpsertEmployeeMonitoringPolicyCommand(UpsertEmployeeMonitoringPolicyRequestDTO DTO)
    : IRequest<ApiResponse<EmployeeMonitoringPolicyResponseDTO>>;

public sealed record RegisterEmployeeMonitoringAgentCommand(RegisterEmployeeMonitoringAgentRequestDTO DTO)
    : IRequest<ApiResponse<EmployeeMonitoringAgentRegistrationResponseDTO>>;

public sealed record GetEmployeeMonitoringAgentsQuery(EmployeeMonitoringPermissionRequestDTO PermissionRequest)
    : IRequest<ApiResponse<List<EmployeeMonitoringAgentStatusResponseDTO>>>;

#endregion

#region Handlers

public sealed class UpsertEmployeeMonitoringPolicyCommandHandler(
    IEmployeeMonitoringRepository repository,
    ICommonRequestService commonRequestService)
    : IRequestHandler<UpsertEmployeeMonitoringPolicyCommand, ApiResponse<EmployeeMonitoringPolicyResponseDTO>>
{
    public async Task<ApiResponse<EmployeeMonitoringPolicyResponseDTO>> Handle(
        UpsertEmployeeMonitoringPolicyCommand request,
        CancellationToken cancellationToken)
    {
        ValidatePolicy(request.DTO);
        var context = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!context.Success)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }

        var policy = await repository.GetPolicyAsync(context.TenantId, cancellationToken);
        if (policy is null)
        {
            policy = new EmployeeMonitoringPolicy
            {
                TenantId = context.TenantId,
                IsActive = true,
                IsSoftDeleted = false,
                AddedById = context.LoggedInEmployeeId,
                AddedDateTime = DateTime.UtcNow
            };
            await repository.AddPolicyAsync(policy, cancellationToken);
        }
        else
        {
            policy.UpdatedById = context.LoggedInEmployeeId;
            policy.UpdatedDateTime = DateTime.UtcNow;
        }

        ApplyPolicy(policy, request.DTO);
        await repository.SaveChangesAsync(cancellationToken);
        return ApiResponse<EmployeeMonitoringPolicyResponseDTO>.Success(ToResponse(policy), "Employee monitoring policy saved successfully.");
    }

    private static void ValidatePolicy(UpsertEmployeeMonitoringPolicyRequestDTO dto)
    {
        if (dto.MinimumCaptureIntervalSeconds < 60 || dto.MaximumCaptureIntervalSeconds < dto.MinimumCaptureIntervalSeconds)
            throw new ValidationErrorException("The capture interval range is invalid.");
        if (dto.HeartbeatIntervalSeconds < 15 || dto.DelayedAfterSeconds <= dto.HeartbeatIntervalSeconds || dto.UnreachableAfterSeconds <= dto.DelayedAfterSeconds)
            throw new ValidationErrorException("The heartbeat status thresholds are invalid.");
        if (dto.OfflineRetentionDays < 1 || dto.MaximumOfflineBytes < 1 || dto.ImageQuality is < 1 or > 100)
            throw new ValidationErrorException("The offline retention, storage limit, or image quality is invalid.");
    }

    private static void ApplyPolicy(EmployeeMonitoringPolicy policy, UpsertEmployeeMonitoringPolicyRequestDTO dto)
    {
        policy.MinimumCaptureIntervalSeconds = dto.MinimumCaptureIntervalSeconds;
        policy.MaximumCaptureIntervalSeconds = dto.MaximumCaptureIntervalSeconds;
        policy.HeartbeatIntervalSeconds = dto.HeartbeatIntervalSeconds;
        policy.DelayedAfterSeconds = dto.DelayedAfterSeconds;
        policy.UnreachableAfterSeconds = dto.UnreachableAfterSeconds;
        policy.OfflineRetentionDays = dto.OfflineRetentionDays;
        policy.MaximumOfflineBytes = dto.MaximumOfflineBytes;
        policy.ImageQuality = dto.ImageQuality;
        policy.CaptureAllMonitors = dto.CaptureAllMonitors;
    }

    internal static EmployeeMonitoringPolicyResponseDTO ToResponse(EmployeeMonitoringPolicy policy) => new(
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

public sealed class RegisterEmployeeMonitoringAgentCommandHandler(
    IEmployeeMonitoringRepository repository,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService)
    : IRequestHandler<RegisterEmployeeMonitoringAgentCommand, ApiResponse<EmployeeMonitoringAgentRegistrationResponseDTO>>
{
    public async Task<ApiResponse<EmployeeMonitoringAgentRegistrationResponseDTO>> Handle(
        RegisterEmployeeMonitoringAgentCommand request,
        CancellationToken cancellationToken)
    {
        var context = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!context.Success)
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        if (string.IsNullOrWhiteSpace(request.DTO.DeviceName) || request.DTO.DeviceName.Length > 200)
            throw new ValidationErrorException("A valid device name is required.");

        long employeeId;
        try
        {
            employeeId = axionpro.application.Common.Helpers.RequestHelper.RequestCommonHelper.DecodeOnlyEmployeeId(
                request.DTO.EmployeeId,
                context.Claims.TenantEncriptionKey,
                idEncoderService);
        }
        catch
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        }

        if (!await repository.EmployeeExistsAsync(context.TenantId, employeeId, cancellationToken))
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);

        var policy = await repository.GetPolicyAsync(context.TenantId, cancellationToken)
            ?? throw new ValidationErrorException("Configure the employee monitoring policy before registering an agent.");
        var credentialBytes = RandomNumberGenerator.GetBytes(32);
        var credential = Convert.ToBase64String(credentialBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        var agent = new axionpro.domain.Entity.EmployeeMonitoringAgent
        {
            AgentInstanceId = Guid.NewGuid(),
            TenantId = context.TenantId,
            EmployeeId = employeeId,
            PolicyId = policy.Id,
            DeviceName = request.DTO.DeviceName.Trim(),
            CredentialHash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(credential))),
            IsActive = true,
            IsSoftDeleted = false,
            AddedById = context.LoggedInEmployeeId,
            AddedDateTime = DateTime.UtcNow
        };
        await repository.AddAgentAsync(agent, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        var response = new EmployeeMonitoringAgentRegistrationResponseDTO(
            agent.AgentInstanceId,
            credential,
            UpsertEmployeeMonitoringPolicyCommandHandler.ToResponse(policy));
        return ApiResponse<EmployeeMonitoringAgentRegistrationResponseDTO>.Success(response, "Employee monitoring agent registered. Store the credential now; it is not returned again.");
    }
}

public sealed class GetEmployeeMonitoringAgentsQueryHandler(
    IEmployeeMonitoringRepository repository,
    ICommonRequestService commonRequestService,
    IIdEncoderService idEncoderService)
    : IRequestHandler<GetEmployeeMonitoringAgentsQuery, ApiResponse<List<EmployeeMonitoringAgentStatusResponseDTO>>>
{
    public async Task<ApiResponse<List<EmployeeMonitoringAgentStatusResponseDTO>>> Handle(
        GetEmployeeMonitoringAgentsQuery request,
        CancellationToken cancellationToken)
    {
        var context = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!context.Success)
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        var policy = await repository.GetPolicyAsync(context.TenantId, cancellationToken)
            ?? throw new NotFoundException("Employee monitoring policy was not found.");
        var now = DateTime.UtcNow;
        var agents = await repository.GetAgentsAsync(context.TenantId, cancellationToken);
        var response = agents.Select(agent => new EmployeeMonitoringAgentStatusResponseDTO(
            agent.AgentInstanceId,
            idEncoderService.EncodeId_long(agent.EmployeeId, context.Claims.TenantEncriptionKey),
            agent.DeviceName,
            ResolveStatus(agent, policy, now),
            agent.LastHeartbeatDateTime,
            agent.LastCaptureDateTime,
            agent.LastSuccessfulUploadDateTime,
            agent.PendingCaptureCount,
            agent.LastErrorCode,
            agent.AgentVersion)).ToList();
        return ApiResponse<List<EmployeeMonitoringAgentStatusResponseDTO>>.Success(response);
    }

    private static string ResolveStatus(axionpro.domain.Entity.EmployeeMonitoringAgent agent, EmployeeMonitoringPolicy policy, DateTime now)
    {
        if (!agent.IsActive || agent.IsSoftDeleted) return EmployeeMonitoringConnectionStatus.Inactive.ToString();
        if (!agent.LastHeartbeatDateTime.HasValue) return EmployeeMonitoringConnectionStatus.NeverConnected.ToString();
        var elapsed = (now - agent.LastHeartbeatDateTime.Value).TotalSeconds;
        if (elapsed >= policy.UnreachableAfterSeconds) return EmployeeMonitoringConnectionStatus.Unreachable.ToString();
        if (elapsed >= policy.DelayedAfterSeconds) return EmployeeMonitoringConnectionStatus.Delayed.ToString();
        if (!string.IsNullOrWhiteSpace(agent.LastErrorCode)) return EmployeeMonitoringConnectionStatus.UploadFailure.ToString();
        return EmployeeMonitoringConnectionStatus.Online.ToString();
    }
}

#endregion
