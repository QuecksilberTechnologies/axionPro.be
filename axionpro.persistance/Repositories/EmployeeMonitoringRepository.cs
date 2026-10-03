using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Repositories;

/// <summary>Provides tenant-scoped persistence for employee monitoring agents and captures.</summary>
public sealed class EmployeeMonitoringRepository(WorkforceDbContext context) : IEmployeeMonitoringRepository
{
    public Task<EmployeeMonitoringPolicy?> GetPolicyAsync(long tenantId, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringPolicies
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    public Task<EmployeeMonitoringAgent?> GetAgentByCredentialHashAsync(string credentialHash, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringAgents
            .Include(x => x.Policy)
            .SingleOrDefaultAsync(x => x.CredentialHash == credentialHash && x.IsActive && !x.IsSoftDeleted, cancellationToken);

    public Task<EmployeeMonitoringAgent?> GetAgentAsync(long tenantId, Guid agentInstanceId, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringAgents
            .Include(x => x.Policy)
            .SingleOrDefaultAsync(x => x.TenantId == tenantId && x.AgentInstanceId == agentInstanceId && !x.IsSoftDeleted, cancellationToken);

    public Task<bool> EmployeeExistsAsync(long tenantId, long employeeId, CancellationToken cancellationToken) =>
        context.Employees.AnyAsync(x => x.TenantId == tenantId && x.Id == employeeId && !x.IsSoftDeleted, cancellationToken);

    public Task<bool> CaptureExistsAsync(long monitoringAgentId, Guid captureId, CancellationToken cancellationToken) =>
        context.EmployeeScreenCaptures.AnyAsync(x => x.MonitoringAgentId == monitoringAgentId && x.CaptureId == captureId, cancellationToken);

    public Task AddPolicyAsync(EmployeeMonitoringPolicy policy, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringPolicies.AddAsync(policy, cancellationToken).AsTask();

    public Task AddAgentAsync(EmployeeMonitoringAgent agent, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringAgents.AddAsync(agent, cancellationToken).AsTask();

    public Task AddCaptureAsync(EmployeeScreenCapture capture, CancellationToken cancellationToken) =>
        context.EmployeeScreenCaptures.AddAsync(capture, cancellationToken).AsTask();

    public Task<List<EmployeeMonitoringAgent>> GetAgentsAsync(long tenantId, CancellationToken cancellationToken) =>
        context.EmployeeMonitoringAgents
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsSoftDeleted)
            .OrderBy(x => x.EmployeeId)
            .ThenBy(x => x.DeviceName)
            .ToListAsync(cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesAsync(cancellationToken);
}
