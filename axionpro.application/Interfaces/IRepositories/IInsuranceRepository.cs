using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static axionpro.application.DTOS.InsurancePolicy.GetAlllnsurancePolicyResponseDTO;
namespace axionpro.application.Interfaces.IRepositories
{
    public interface IInsuranceRepository
    {

        // 🔹 CREATE
        Task<GetInsurancePolicyResponseDTO?> AddAsync(InsurancePolicy policy);

        // 🔹 GET (By Id)
        Task<InsurancePolicy?> GetByIdAsync( int insurancePolicyId,    long tenantId, bool isActive);

        // 🔹 GET LIST (Grid / Listing)
        Task<PagedResponseDTO<GetInsurancePolicyResponseDTO>> GetListAsync(  GetInsurancePolicyRequestDTO request  );

        Task<ApiResponse<List<GetAlllnsurancePolicyResponseDTO>>> GetAllListAsync(int policyId, bool isActive);
        Task<ApiResponse<List<GetAlllnsurancePolicyWithDetailsResponseDTO>>> GetAllPolicyListWithConsumedDetailsAsync(long employeeId, int policyId, bool isActive);
         
      
        // 🔹 UPDATE
        Task<bool> UpdateAsync(InsurancePolicy policy);

        // 🔹 SOFT DELETE
        Task<bool> SoftDeleteAsync(InsurancePolicy policyType);
       

        // 🔹 EXISTS (Validation use)
        Task<bool> ExistsAsync(
            string insurancePolicyName,
            long tenantId,
            CancellationToken cancellationToken);
    }

}
