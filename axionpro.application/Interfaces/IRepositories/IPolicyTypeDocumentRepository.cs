using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IPolicyTypeDocumentRepository
    {
        // 🔹 CREATE
          Task<GetPolicyTypeDocumentResponseDTO> AddAsync(PolicyTypeDocument entity);

        // 🔹 GET BY ID
        Task<PolicyTypeDocument?> GetPolicyTypeOnlyDocByIdAsync(  long Id);
        Task<PolicyTypeDocument?> GetByIdAsync( int id, long tenantId, bool isActive );

        // 🔹 BULK FETCH (PolicyType wise)
        Task<List<PolicyTypeDocument>>  GetByPolicyTypeIdsAsync(  List<int> policyTypeIds, long tenantId);

        // 🔹 GET LIST (PolicyType wise / grid)
        Task<PagedResponseDTO<GetPolicyTypeDocumentResponseDTO>> GetListAsync(GetPolicyTypeDocumentRequestDTO request);

        // 🔹 UPDATE (metadata / replace file path etc.)
        Task<bool> UpdateAsync(PolicyTypeDocument entity);

        // 🔹 SOFT DELETE

          Task<bool> SoftDeleteByPolicyTypeIdAsync( int policyTypeId, long deletedById);

        Task<bool> SoftDeleteOnlyDocAsync(PolicyTypeDocument entity);

        // 🔹 EXISTS (validation / duplicate check)
        Task<bool> ExistsAsync( int policyTypeId, long tenantId);
    }
}
