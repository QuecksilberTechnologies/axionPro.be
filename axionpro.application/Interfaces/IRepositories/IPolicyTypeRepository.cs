using axionpro.application.DTOs.PolicyType;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPolicyTypeRepository
    {
        // ================================
        // 🔹 CREATE
        // ================================
        Task<GetPolicyTypeResponseDTO> CreatePolicyTypeAsync(
            PolicyType policyType);

        Task<List<PolicyType>> AutoCreatePolicyTypesAsync(List<PolicyType> policyTypes);
        // ================================
        // 🔹 READ (Get by Id)
        // Used by handler to check existence
        // ================================
        Task<PolicyType?> GetPolicyTypeByIdAsync(
            int id, bool? isActive);
      

        // ================================
        // 🔹 READ (Get all by Tenant)
        // Used for listing / dropdown / grid
        // ================================
        Task<ApiResponse<List<GetAllPolicyTypeResponseDTO>>?> GetAllPolicyTypesAsync(long tenantId, bool isActive, int enumval);
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
