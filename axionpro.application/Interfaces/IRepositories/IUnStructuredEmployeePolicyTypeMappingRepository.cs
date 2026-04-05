using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IUnStructuredEmployeePolicyTypeMappingRepository
    {
        Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetAllAsync(long tenantId);

        Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetByEmployeeTypeIdAsync(int employeeTypeId, long tenantId);

        Task<List<UnStructuredPolicyTypeMappingWithEmployeeType>> GetByEmployeeTypeByPolicyTypeIdAsync(int policyTypeId, long tenantId);

        Task<UnStructuredPolicyTypeMappingWithEmployeeType?> GetByIdAsync(long id);

        Task<bool> AddRangeAsync(List<UnStructuredPolicyTypeMappingWithEmployeeType> entities);

        Task<bool> UpdateAsync(UnStructuredPolicyTypeMappingWithEmployeeType entity);

        Task<bool> SoftDeleteAsync(UnStructuredPolicyTypeMappingWithEmployeeType entity);
    }
}
