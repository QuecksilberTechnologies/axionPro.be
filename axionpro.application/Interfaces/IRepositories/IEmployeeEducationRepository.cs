

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
using SkiaSharp;
using System.Collections.Generic;



namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeEducationRepository
{



    #region Employee-Education-info
    public  Task<bool> DeleteAsync(EmployeeEducation employeeEducation);
    public Task<PagedResponseDTO<GetEducationResponseDTO>> GetInfo(GetEducationRequestDTO dto);
    public Task<PagedResponseDTO<GetEducationResponseDTO>> CreateAsync(EmployeeEducation entity);
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
    public Task<EmployeeEducation> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns    
    public   Task<bool> UpdateEmployeeFieldAsync(EmployeeEducation entity);
    #endregion




}

