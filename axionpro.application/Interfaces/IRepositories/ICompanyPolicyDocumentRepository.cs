using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;

using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ICompanyPolicyDocumentRepository
    {
        // 🔹 CREATE
          Task<GetCompanyPolicyDocumentResponseDTO> AddAsync(Companypolicydocument entity);

        // 🔹 GET BY ID
        Task<Companypolicydocument?> GetByIdAsync( int id, long tenantId, bool isActive );

        // 🔹 BULK FETCH (PolicyType wise)
        Task<List<Companypolicydocument>>  GetByPolicyTypeIdsAsync(  List<int> policyTypeIds, long tenantId);

        // 🔹 GET LIST (PolicyType wise / grid)
        Task<PagedResponseDTO<GetCompanyPolicyDocumentResponseDTO>> GetListAsync(GetCompanyPolicyDocumentRequestDTO request);

        // 🔹 UPDATE (metadata / replace file path etc.)
        Task<bool> UpdateAsync(Companypolicydocument entity);

        // 🔹 SOFT DELETE

          Task<bool> SoftDeleteByPolicyTypeIdAsync( int policyTypeId, long deletedById);
       

        // 🔹 EXISTS (validation / duplicate check)
        Task<bool> ExistsAsync( int policyTypeId, long tenantId);
    }
}
