

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

public interface IEmployeeContactRepository
{



    #region Employee-Contact-info
    public Task<PagedResponseDTO<GetContactResponseDTO>> GetInfo(GetContactRequestDTO dto);  
    public Task<PagedResponseDTO<GetContactResponseDTO>> CreateAsync(EmployeeContact entity);
    public Task<PagedResponseDTO<GetContactResponseDTO>> AutoCreatedAsync(EmployeeContact entity);  
    public  Task<EmployeeContact?> GetSingleRecordAsync(long id, bool track = true);// Ensure this returns 
    public  Task<EmployeeContact?> GetPrimaryLocationAsync(long employeeId, bool isActive, bool track = true);// Ensure this returns 

    public Task<bool> UpdateContactAsync(EmployeeContact employeeContact);
    public Task<bool> DeleteAsync(EmployeeContact employeeContact);


    #endregion

}

