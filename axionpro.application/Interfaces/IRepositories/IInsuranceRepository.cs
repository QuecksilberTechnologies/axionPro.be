// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines persistence operations for insurance policies and insurance query projections.
// ================================================================

using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using static axionpro.application.DTOS.InsurancePolicy.GetAlllnsurancePolicyResponseDTO;

namespace axionpro.application.Interfaces.IRepositories
{
    /// <summary>
    /// Defines persistence operations for insurance policy data and query projections.
    /// </summary>
    public interface IInsuranceRepository
    {
        /// <summary>
        /// Persists an insurance policy and returns its projected data.
        /// </summary>
        Task<GetInsurancePolicyResponseDTO?> AddAsync(InsurancePolicy policy);

        /// <summary>
        /// Gets an insurance policy for the trusted tenant scope.
        /// </summary>
        Task<InsurancePolicy?> GetByIdAsync(int insurancePolicyId, long tenantId, bool isActive);

        /// <summary>
        /// Gets a paged insurance policy projection result.
        /// </summary>
        Task<PagedResponseDTO<GetInsurancePolicyResponseDTO>> GetListAsync(GetInsurancePolicyRequestDTO request);

        /// <summary>
        /// Gets available insurance-policy option projections.
        /// </summary>
        Task<List<GetAlllnsurancePolicyResponseDTO>> GetAllListAsync(int policyId, bool isActive);

        /// <summary>
        /// Gets insurance-policy projections enriched with an employee's consumption details.
        /// </summary>
        Task<List<GetAlllnsurancePolicyWithDetailsResponseDTO>> GetAllPolicyListWithConsumedDetailsAsync(
            long employeeId,
            int policyId,
            bool isActive);

        /// <summary>
        /// Persists an updated insurance policy.
        /// </summary>
        Task<bool> UpdateAsync(InsurancePolicy policy);

        /// <summary>
        /// Soft deletes an insurance policy.
        /// </summary>
        Task<bool> SoftDeleteAsync(InsurancePolicy policyType);

        /// <summary>
        /// Determines whether an insurance-policy name exists for a tenant.
        /// </summary>
        Task<bool> ExistsAsync(
            string insurancePolicyName,
            long tenantId,
            CancellationToken cancellationToken);
    }
}
