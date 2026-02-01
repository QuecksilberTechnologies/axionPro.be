using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IInsuranceRepository
    {

        // 🔹 CREATE
        Task<GetInsurancePolicyResponseDTO?> AddAsync(InsurancePolicy policy);

        // 🔹 GET (By Id)
        Task<InsurancePolicy?> GetByIdAsync(
            int insurancePolicyId,
            long tenantId,
            CancellationToken cancellationToken);

        // 🔹 GET LIST (Grid / Listing)
        Task<PagedResponseDTO<GetInsurancePolicyResponseDTO>> GetListAsync(
        GetInsurancePolicyRequestDTO request  );


        // 🔹 UPDATE
        Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken);

        // 🔹 SOFT DELETE
        Task SoftDeleteAsync(
            int insurancePolicyId,
            long tenantId,
            long deletedById,
            CancellationToken cancellationToken);

        // 🔹 EXISTS (Validation use)
        Task<bool> ExistsAsync(
            string insurancePolicyName,
            long tenantId,
            CancellationToken cancellationToken);
    }
}
