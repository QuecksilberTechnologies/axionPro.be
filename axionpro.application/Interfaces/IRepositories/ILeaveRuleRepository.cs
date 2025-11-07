using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ILeaveRuleRepository
    {
        // Create a new role
        Task<List<GetLeaveRuleResponseDTO>> CreateLeaveRuleAsync(LeaveRule leaveRule);
        
        Task<LeaveRule> GetLeaveRuleByIdAsync(long LeaveRuleId);
        Task<List<GetLeaveRuleResponseDTO>> GetLeaveRuleAsync(GetLeaveRuleRequestDTO dTO);
        Task<List<GetLeaveRuleResponseDTO>> GetLeaveRuleByIsSandwichAsync(GetLeaveRuleRequestDTO dTO);

        
       Task<LeaveRule> UpdateLeaveRuleAsync(LeaveRule leaveRule, long userId);

        // Delete a role by its Id
       
        Task<bool> DeleteLeaveRuleAsync(LeaveRule leave);
       
    }
}
 
