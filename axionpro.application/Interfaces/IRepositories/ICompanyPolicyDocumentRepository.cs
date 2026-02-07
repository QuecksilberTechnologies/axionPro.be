using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOS.CompanyPolicyDocument;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ICompanyPolicyDocumentRepository
    {
        // 🔹 CREATE
          Task<GetCompanyPolicyDocumentResponseDTO> AddAsync(CompanyPolicyDocument entity);

        // 🔹 GET BY ID
        Task<CompanyPolicyDocument?> GetByIdAsync( int id, long tenantId, bool isActive );

        // 🔹 GET LIST (PolicyType wise / grid)
        Task<PagedResponseDTO<GetCompanyPolicyDocumentResponseDTO>> GetListAsync(GetCompanyPolicyDocumentRequestDTO request);

        // 🔹 UPDATE (metadata / replace file path etc.)
        Task<bool> UpdateAsync(CompanyPolicyDocument entity);

        // 🔹 SOFT DELETE
        Task<bool> SoftDeleteAsync( long id);

        // 🔹 EXISTS (validation / duplicate check)
        Task<bool> ExistsAsync( int policyTypeId, long tenantId);
    }
}
