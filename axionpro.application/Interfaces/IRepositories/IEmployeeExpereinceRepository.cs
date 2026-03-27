using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeExperienceRepository
{
    // 🔹 CREATE
    Task  AddAsync(EmployeeExperience entity);

    // 🔹 UPDATE
    Task<bool> UpdateAsync(EmployeeExperience entity);

    // 🔹 DELETE (Soft Delete)
    Task<bool> SoftDeleteAsync(EmployeeExperience entity);

    // 🔹 GET BY ID
    Task<EmployeeExperience?> GetByIdAsync(long id, long employeeid);

    // 🔹 GET LIST (WITH DETAILS)
    Task<List<EmployeeExperience>> GetByEmployeeIdWithDocumentsAsync(long employeeId);
}

