// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Employee Expereince Repository.
// ================================================================

using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Defines the contract for EmployeeExperienceRepository.
/// </summary>
public interface IEmployeeExperienceRepository
{
    //  CREATE
    Task<GetEmployeeExperienceResponseDTO> AddAsync(EmployeeExperience entity);

    //  UPDATE
    Task<bool> UpdateAsync(EmployeeExperience entity);

    //  DELETE (Soft Delete)
    Task<bool> SoftDeleteAsync(EmployeeExperience entity);
    Task<bool> SoftDeleteDocAsync(EmployeeExperienceDocument entity);

    //  GET BY ID
    Task<EmployeeExperience?> GetByIdAsync(long id, long employeeid);

    //  GET LIST (WITH DETAILS)
    Task<PagedResponseDTO<GetEmployeeExperienceResponseDTO>> GetByEmployeeIdWithDocumentsAsync(long employeeId, GetExperienceRequestDTO employee);
}

