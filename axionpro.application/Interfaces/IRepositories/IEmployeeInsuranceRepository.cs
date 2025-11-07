

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

public interface IEmployeeInsuranceRepository
{

    public Task<PagedResponseDTO<GetExperienceResponseDTO>> GetInfo(GetExperienceRequestDTO dto, string tenantKey);
    public Task<PagedResponseDTO<GetExperienceResponseDTO>> CreateAsync(EmployeeExperience entity);
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
    public Task<EmployeeContact> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 



}

