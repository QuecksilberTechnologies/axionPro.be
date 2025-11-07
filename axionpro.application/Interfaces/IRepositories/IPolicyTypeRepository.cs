using axionpro.application.DTOs.Category;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPolicyTypeRepository
    {
        // Create: Add a new Policy Type
         Task<List<GetPolicyTypeResponseDTO>> CreatePolicyTypeAsync(PolicyType request);   

        // Read: Get all Policy Types
        Task<IEnumerable<GetPolicyTypeResponseDTO>> GetAllPolicyTypesAsync(GetPolicyTypeResponseDTO getPolicyTypeDTO); 

        // Read: Get a Policy Type by ID
    //    Task<GetAllPolicyTypeResponseDTO> GetPolicyTypeByIdAsync(int id);

        // Update: Update an existing Policy Type
        Task<bool> UpdatePolicyTypeAsync(UpdatePolicyTypeDTO request);

        // Delete: Delete a Policy Type by ID (soft delete preferred)
        Task<bool> SoftDeletePolicyTypeAsync(DeletePolicyTypeDTO request);
       
    }
}
