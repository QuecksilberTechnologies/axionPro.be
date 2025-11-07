
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;

using axionpro.application.DTOS.Pagination;
 
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeIdentityRepository
{


    #region Employee-Identity-info


    public Task<PagedResponseDTO<GetIdentityResponseDTO>> GetInfo(GetIdentityRequestDTO dto, long employeeId,long Id);
    public Task<GetIdentityResponseDTO> CreateAsync(EmployeePersonalDetail entity);
    //  public Task<PagedResponseDTO<GetDependentResponseDTO>> AutoCreatedAsync(EmployeeContact entity);
    public Task<GetIdentityResponseDTO> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 


    #endregion








}

