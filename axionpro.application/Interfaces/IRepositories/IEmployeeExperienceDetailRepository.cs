using axionpro.domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.Interfaces.IRepositories
{
    public interface IEmployeeExperienceDetailRepository
    {
        // 🔹 CREATE
        Task AddAsync(EmployeeExperience entity);
        Task AddRangeAsync(List<EmployeeExperience> entities);

        // 🔹 UPDATE
        Task<bool> UpdateAsync(EmployeeExperience entity);

        // 🔹 DELETE (Soft Delete)
        Task<bool> SoftDeleteAsync(EmployeeExperience entity);

        // 🔹 GET (By Parent)
        Task<List<EmployeeExperience>> GetByExperienceIdAsync(long experienceId);

        // 🔹 GET SINGLE (optional but useful)
        Task<EmployeeExperience?> GetByIdAsync(long id);
    }
}
