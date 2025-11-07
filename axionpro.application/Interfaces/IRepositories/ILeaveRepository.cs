using axionpro.application.DTOs.Leave;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ILeaveRepository
    {
        // Create a new role
        
        Task<List<PolicyLeaveTypeMapping>> CreateLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicyType);
        Task<List<LeaveType>> CreateLeaveTypeAsync(LeaveType leaveType);
        // Get a role by its Id
        Task<LeaveType> GetLeaveByIdAsync(int leaveId);
        Task<PolicyLeaveTypeMapping> GetLeavePolicyByIdAsync(long leavePolicyId);
       
        // Get all roles
        Task<List<LeaveType>> GetAllLeaveAsync( bool? IsActive, long? tenantId);
        Task<List<PolicyLeaveTypeMapping>> GetAllLeavePolicyByTenantIdAsync(long tenantId,bool Isactive);
        Task<List<GetLeaveTypeWithPolicyMappingResponseDTO>> GetAllLeavePolicyByEmployeeTypeIdAsync(GetPolicyLeaveTypeByEmpTypeIdRequestDTO getPolicyLeaveTypeByEmpTypeIdRequestDTO);
        Task<List<PolicyLeaveTypeMapping>> GetAllLeavePolicyAsync(bool IsActive);

        // Update an existing role
        Task<bool> UpdateLeavTypeAsync(LeaveType leaveType, long userId);
        Task<bool> UpdateLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy, long userId);
        Task<bool> UpdateLeaveAssignOnlyAsync(long policyId, long userId, bool IsUpdate);
        
        // Delete a role by its Id
        Task<bool> DeleteLeaveAsync(LeaveType leaveType);
         
        Task<bool> DeleteLeavePolicyAsync(PolicyLeaveTypeMapping leavePolicy);
    }
}
 
