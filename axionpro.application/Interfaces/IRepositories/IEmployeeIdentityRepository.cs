
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;

using axionpro.application.DTOS.Pagination;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeIdentityRepository
{


    #region Employee-Identity-info

   
   
 
    public Task<GetEmployeeIdentityResponseDTO> GetInfo(GetIdentityRequestDTO dto);
    public Task<GetIdentityResponseDTO> CreateAsync(EmployeePersonalDetail entity);
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
       public Task<EmployeePersonalDetail> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 
       public Task<bool> IsEmployeePersonalDetailExistsAsync(long Id, bool? IsActive);  // Ensure this returns 

    public Task<bool> UpdateIdentity(EmployeePersonalDetail employeePersonal);
    #endregion








}

