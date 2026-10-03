using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeMonitoringRepository
{
    Task<EmployeeMonitoringPolicy?> GetPolicyAsync(long tenantId, CancellationToken cancellationToken);
    Task<EmployeeMonitoringAgent?> GetAgentByCredentialHashAsync(string credentialHash, CancellationToken cancellationToken);
    Task<EmployeeMonitoringAgent?> GetAgentAsync(long tenantId, Guid agentInstanceId, CancellationToken cancellationToken);
    Task<bool> EmployeeExistsAsync(long tenantId, long employeeId, CancellationToken cancellationToken);
    Task<bool> CaptureExistsAsync(long monitoringAgentId, Guid captureId, CancellationToken cancellationToken);
    Task AddPolicyAsync(EmployeeMonitoringPolicy policy, CancellationToken cancellationToken);
    Task AddAgentAsync(EmployeeMonitoringAgent agent, CancellationToken cancellationToken);
    Task AddCaptureAsync(EmployeeScreenCapture capture, CancellationToken cancellationToken);
    Task<List<EmployeeMonitoringAgent>> GetAgentsAsync(long tenantId, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
