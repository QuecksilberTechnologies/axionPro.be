// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the repository contract for  Employee Education Repository.
// ================================================================

using axionpro.application.DTOS.Employee.Education;

using axionpro.application.DTOS.Pagination;


using axionpro.domain.Entity;


namespace axionpro.application.Interfaces.IRepositories;

/// <summary>
/// Defines the contract for EmployeeEducationRepository.
/// </summary>
public interface IEmployeeEducationRepository
{



    #region Employee-Education-info
    public  Task<bool> DeleteAsync(EmployeeEducation employeeEducation);
    public Task<PagedResponseDTO<GetEducationResponseDTO>> GetInfo(long employeeId, GetEducationRequestDTO dto);
    public Task<PagedResponseDTO<GetEducationResponseDTO>> CreateAsync(EmployeeEducation entity);
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
    public Task<EmployeeEducation> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns    
    public   Task<bool> UpdateEmployeeFieldAsync(EmployeeEducation entity);
    #endregion




}

