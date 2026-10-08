using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Defines persistence operations for tenant leave-type reference data.
/// Policy behavior is managed by the generic policy framework.
/// </summary>
public interface ILeaveRepository
{
    Task<List<LeaveType>> CreateLeaveTypeAsync(LeaveType leaveType);

    Task<LeaveType?> GetLeaveByIdAsync(int leaveId);

    Task<List<LeaveType>> GetAllLeaveAsync(bool? isActive, long? tenantId);

    Task<bool> UpdateLeavTypeAsync(LeaveType leaveType, long userId);

    Task<bool> DeleteLeaveAsync(LeaveType leaveType);
}
