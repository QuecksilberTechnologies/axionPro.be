using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.EmployeeMonitoring;

public sealed class UpsertEmployeeMonitoringPolicyRequestDTO : PermissionRequestDTO
{
    public int MinimumCaptureIntervalSeconds { get; set; }
    public int MaximumCaptureIntervalSeconds { get; set; }
    public int HeartbeatIntervalSeconds { get; set; }
    public int DelayedAfterSeconds { get; set; }
    public int UnreachableAfterSeconds { get; set; }
    public int OfflineRetentionDays { get; set; }
    public long MaximumOfflineBytes { get; set; }
    public int ImageQuality { get; set; }
    public bool CaptureAllMonitors { get; set; }
}

public sealed class RegisterEmployeeMonitoringAgentRequestDTO : PermissionRequestDTO
{
    public string EmployeeId { get; set; } = string.Empty;
    public string DeviceName { get; set; } = string.Empty;
}

public sealed class EmployeeMonitoringPermissionRequestDTO : PermissionRequestDTO;

public sealed record EmployeeMonitoringPolicyResponseDTO(
    int MinimumCaptureIntervalSeconds,
    int MaximumCaptureIntervalSeconds,
    int HeartbeatIntervalSeconds,
    int DelayedAfterSeconds,
    int UnreachableAfterSeconds,
    int OfflineRetentionDays,
    long MaximumOfflineBytes,
    int ImageQuality,
    bool CaptureAllMonitors);

public sealed record EmployeeMonitoringAgentRegistrationResponseDTO(
    Guid AgentInstanceId,
    string AgentCredential,
    EmployeeMonitoringPolicyResponseDTO Policy);

public sealed record EmployeeMonitoringAgentStatusResponseDTO(
    Guid AgentInstanceId,
    string EmployeeId,
    string DeviceName,
    string ConnectionStatus,
    DateTime? LastHeartbeatDateTime,
    DateTime? LastCaptureDateTime,
    DateTime? LastSuccessfulUploadDateTime,
    int PendingCaptureCount,
    string? LastErrorCode,
    string? AgentVersion);

public sealed record AgentHeartbeatRequestDTO(
    string? AgentVersion,
    int PendingCaptureCount,
    DateTime? LastCaptureDateTime,
    DateTime? LastSuccessfulUploadDateTime,
    string? LastErrorCode);
