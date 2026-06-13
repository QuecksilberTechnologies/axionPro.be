using axionpro.application.DTOS.Compliances.ComplianceRule;
using axionpro.domain.Entity;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface ICompilanceRuleRepository
    {

        Task<ComplianceRule?> GetRuleAsync(
                  int complianceTypeId,
                  int countryId,
                  int? stateId,
                  long? tenantId, DateOnly effectiveFrom);
        Task<bool>  ExistsAsync(int complianceTypeId, int countryId, int? stateId, long? tenantId, DateOnly effectiveFrom);
        Task<ComplianceRule> GetByIdAsync(long id);

        Task AddAsync(ComplianceRule entity);

        Task UpdateAsync(ComplianceRule entity);
    }


}
