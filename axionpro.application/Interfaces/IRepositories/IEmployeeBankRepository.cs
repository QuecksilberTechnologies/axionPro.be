

using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;
 
 
using axionpro.application.DTOS.Employee.Type;

using axionpro.application.DTOS.Pagination;
 
using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IEmployeeBankRepository
{

    #region Bank-info
    public Task<bool> UpdateEditStatus(long EmployeeId, long UserId, bool Status);
    public Task<bool> UpdateVerificationStatus(long EmployeeId, long UserId, bool Status);
    public Task<PagedResponseDTO<GetBankResponseDTO>> AddCreatedAsync(EmployeeBankDetail entity); // Ensure this returns 
    public Task< PagedResponseDTO<GetBankResponseDTO>> CreateAsync(EmployeeBankDetail entity);  // Ensure this returns 
    public Task<EmployeeBankDetail> GetSingleRecordAsync(int Id ,bool IsActive);  // Ensure this returns    
    public  Task<PagedResponseDTO<GetBankResponseDTO>> GetInfoAsync(GetBankReqestDTO dto);  
    public Task<bool> ResetPrimaryAccountAsync(long employeeId, long byUserId);
    public Task<bool> DeleteAsync(EmployeeBankDetail employeeBankDetail);
    public Task<bool> UpdateAsync(UpdateBankReqestDTO entity);



    #endregion

}

