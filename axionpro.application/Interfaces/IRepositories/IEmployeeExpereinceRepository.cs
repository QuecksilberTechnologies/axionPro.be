

using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
 
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;

using axionpro.application.DTOS.Pagination;
 
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeExpereinceRepository
{


    #region Employee-Experience-info

    //    public Task<PagedResponseDTO<GetExperienceResponseDTO>> GetInfo(GetExperienceRequestDTO dto, long EmployeeId, long Id);
    //  public Task<PagedResponseDTO<GetExperienceResponseDTO>> CreateAsync(EmployeeExperience dto);

  
        Task<PagedResponseDTO<EmployeeExperience>> AddExperienceAsync(EmployeeExperience entity);

        Task<PagedResponseDTO<EmployeeExperienceDetail>> AddDetailAsync(List<EmployeeExperienceDetail> entities);

        Task<PagedResponseDTO<EmployeeExperiencePayslipUpload>> AddPayslipAsync(List<EmployeeExperiencePayslipUpload> entities);
    


    public Task<PagedResponseDTO<EmployeeExperience>> GetAllAsync(long employeeId);
    
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
    //public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 
    //public Task<bool> UpdateEmployeeFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById);

    



    #endregion



}

