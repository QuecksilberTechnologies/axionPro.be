using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeExperienceDocumentRepository
    {
        Task AddAsync(EmployeeExperienceDocument entity);
        Task AddRangeAsync(List<EmployeeExperienceDocument> entities);

        Task<bool> SoftDeleteAsync(EmployeeExperienceDocument entity);

        Task<List<EmployeeExperienceDocument>> GetByDetailIdAsync(long detailId);
    }
}
