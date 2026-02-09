using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPolicyTypeInsuranceMappingRepository
    {

        // 🔹 CREATE
        Task<GetPolicyTypeInsuranceMappingResponseDTO?> AddAsync(   PolicyTypeInsuranceMapping entity);

        // 🔹 GET (By Id)
        Task<PolicyTypeInsuranceMapping?> GetByIdAsync(  int id, bool isActive);

        // 🔹 GET LIST (Grid / Listing)
        Task<PagedResponseDTO<GetPolicyTypeInsuranceMappingResponseDTO>> GetListAsync( GetPolicyTypeInsuranceMappingRequestDTO request);
        Task<List<GetPolicyTypeInsuranceMapDetailsResponseDTO>> GetMapInsuranceDetailAsync( int policyId, bool isActive);
        Task<bool> ExistsAsync(int id, bool isActive);

        //    // 🔹 UPDATE
        //  Task<bool> UpdateAsync( PolicyTypeInsuranceMapping entity);
         Task<bool> UpdateAsync(PolicyTypeInsuranceMapping entity);

    //    // 🔹 SOFT DELETE
          Task<bool> SoftDeleteAsync(PolicyTypeInsuranceMapping entity);

        //    // 🔹 EXISTS (Validation use)
        //}

}
}
