

using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.DTOS.Employee.Contact;

using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.DTOS.EmployeeLeavePolicyMap;
using axionpro.application.DTOS.Pagination;

using axionpro.domain.Entity;
using MediatR;
using System.Collections.Generic;

namespace axionpro.application.Interfaces.IRepositories;

public interface IBaseEmployeeRepository
{

    #region Employee-Base-info

    public Task<bool> UpdateVerifyEditStatusAsync(
      string sectionType,
      long EmployeeId,
      bool? isVerified,
      bool? isEditAllowed,
      bool? isActive,
      long userId, bool? IsActive // NEW : logged in admin user ID
  );

    Task<GetLeaveBalanceToEmployeeResponseDTO> UpdateLeaveBalanceToEmployee(UpdateLeaveBalanceToEmployeeRequestDTO updateLeaveBalanceTo);  // Ensure this returns Task<Employee>   

     Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> GetInfo(GetBaseEmployeeRequestDTO dto, long  decryptedTenantId, long id);
     Task<PagedResponseDTO<GetAllEmployeeInfoResponseDTO>> GetAllInfo(GetAllEmployeeInfoRequestDTO dto, long decryptedTenantId);

    public Task<PagedResponseDTO<GetBaseEmployeeResponseDTO>> CreateAsync(Employee entity);
    public Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> AddImageAsync(EmployeeImage entity);
    
    public Task<GetMinimalEmployeeResponseDTO> GetSingleRecordAsync(long Id, bool IsActive);  // Ensure this returns 
    
      public Task<bool> DeleteAsync(long id);
      public  Task<long> AutoCreated(Employee entity);
     
      public Task<bool> UpdateEmployeeFieldAsync(long Id, string entity, string fieldName, object? fieldValue, long updatedById);
      public Task<PagedResponseDTO<GetEmployeeImageReponseDTO>> GetImage(GetEmployeeImageRequestDTO dto, long decryptedTenantId);
    public Task<List<CompletionSectionDTO>> GetEmployeeCompletionAsync(long employeeId);
    #endregion





}

