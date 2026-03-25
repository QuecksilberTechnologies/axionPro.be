using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeExperienceDetailRepository
    {
        // 🔹 CREATE
        Task AddAsync(EmployeeExperienceDetail entity);
        Task AddRangeAsync(List<EmployeeExperienceDetail> entities);

        // 🔹 UPDATE
        Task<bool> UpdateAsync(EmployeeExperienceDetail entity);

        // 🔹 DELETE (Soft Delete)
        Task<bool> SoftDeleteAsync(EmployeeExperienceDetail entity);

        // 🔹 GET (By Parent)
        Task<List<EmployeeExperienceDetail>> GetByExperienceIdAsync(long experienceId);

        // 🔹 GET SINGLE (optional but useful)
        Task<EmployeeExperienceDetail?> GetByIdAsync(long id);
    }
}
