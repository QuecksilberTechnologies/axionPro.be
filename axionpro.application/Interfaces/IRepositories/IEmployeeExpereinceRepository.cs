using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeExperienceRepository
{
    // 🔹 CREATE
    Task AddAsync(EmployeeExperience entity);

    // 🔹 UPDATE
    Task<bool> UpdateAsync(EmployeeExperience entity);

    // 🔹 DELETE (Soft Delete)
    Task<bool> SoftDeleteAsync(EmployeeExperience entity);

    // 🔹 GET BY ID
    Task<EmployeeExperience?> GetByIdAsync(long id, long tenantId);

    // 🔹 GET LIST (WITH DETAILS)
    Task<(List<EmployeeExperience> Items, int TotalCount)>
        GetListAsync(long employeeId, int pageNumber, int pageSize);
}

