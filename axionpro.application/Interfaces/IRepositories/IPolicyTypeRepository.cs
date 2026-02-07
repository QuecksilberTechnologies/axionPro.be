using axionpro.application.DTOs.PolicyType;
using axionpro.domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPolicyTypeRepository
    {
        // ================================
        // 🔹 CREATE
        // ================================
        Task<GetPolicyTypeResponseDTO> CreatePolicyTypeAsync(
            PolicyType policyType);

        // ================================
        // 🔹 READ (Get by Id)
        // Used by handler to check existence
        // ================================
        Task<PolicyType?> GetPolicyTypeByIdAsync(
            int id);

        // ================================
        // 🔹 READ (Get all by Tenant)
        // Used for listing / dropdown / grid
        // ================================
        Task<IEnumerable<GetAllPolicyTypeResponseDTO>> GetAllPolicyTypesAsync(long tenantId, bool isActive);
        Task<IEnumerable<GetPolicyTypeResponseDTO>> GetPolicyTypesAsync(long tenantId, bool isActive);

        // ================================
        // 🔹 UPDATE
        // Used for normal update
        // ================================
        Task<bool> UpdatePolicyTypeAsync(
            PolicyType policyType);

        // ================================
        // 🔹 SOFT DELETE
        // NOTE:
        // Handler sets IsSoftDelete / IsActive / audit fields
        // Repository only persists changes
        // ================================
        Task<bool> SoftDeletePolicyTypeAsync(
            PolicyType policyType);
    }
}
